using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Player_SyncRotation : NetworkBehaviour
{
    private const float SYNC_ROT_THRESHOLD = 0.4f;
    private const float CLOSE_ENOUGH = 0.1f;

    [SyncVar(hook = "OnPlayerRotSynced")]
    private float mSyncPlayerRotation;

    [SyncVar(hook = "OnCamRotSynced")]
    private float mSyncCamRotation;

    private GamePlayer mGamePlayer;

    public RoleFixedPoint mRoleFixedPoint;

    private Transform mEyeTs;

    private float mLerpRate = 20f;

    private float mLastPlayerRot;
    private float mLastCamRot;

    private Transform mTs;

    private List<float> mSyncPlayerRotList = new List<float>();
    private List<float> mSyncCamRotList = new List<float>();

    private bool mUseHistoricalInterpolation = false;

    /// <summary>攝影機仰角</summary>
    private float CamAngleX { get { return PlayerCam.Instance.AngleX; } }

    void Awake()
    {
        mTs = transform;
        mGamePlayer = GetComponent<GamePlayer>();
        mRoleFixedPoint = mGamePlayer.RoleFixedPoint;
        mEyeTs = mRoleFixedPoint.EyeTs;
    }

    void FixedUpdate()
    {
        UpdateEyesRotation();

        TransmitRotation();
    }

    void Update()
    {
        LerpRotation();
    }

    [Client]
    private void TransmitRotation()
    {
        float camAngleX = mEyeTs.eulerAngles.x;

        if (isLocalPlayer && CheckIfBeyondThreshold(mTs.localEulerAngles.y, mLastPlayerRot) || CheckIfBeyondThreshold(camAngleX, mLastCamRot))
        {
            mLastPlayerRot = mTs.localEulerAngles.y;
            mLastCamRot = camAngleX;

            CmdProvideRotationToServer(mLastPlayerRot, mLastCamRot);
        }
    }

    private void LerpRotation()
    {
        if (!isLocalPlayer)
        {
            if (mUseHistoricalInterpolation)
                HistoricalLerping();
            else
                OrdinaryLerping();
        }
    }

    private void OrdinaryLerping()
    {
        LerpPlayerRotation(mSyncPlayerRotation);
        LerpEyesRotation(mSyncCamRotation);
    }

    private void HistoricalLerping()
    {
        if (mSyncPlayerRotList.Count > 0)
        {
            LerpPlayerRotation(mSyncPlayerRotList[0]);

            if (Mathf.Abs(mTs.localEulerAngles.y - mSyncPlayerRotList[0]) < CLOSE_ENOUGH)
                mSyncPlayerRotList.RemoveAt(0);
        }

        if (mSyncCamRotList.Count > 0)
        {
            LerpEyesRotation(mSyncCamRotList[0]);

            if (Mathf.Abs(mEyeTs.localEulerAngles.x - mSyncCamRotList[0]) < CLOSE_ENOUGH)
                mSyncCamRotList.RemoveAt(0);
        }
    }

    private void LerpPlayerRotation(float rotAngle)
    {
        var rot = Quaternion.Euler(new Vector3(0f, rotAngle, 0f));
        mTs.rotation = Quaternion.Lerp(mTs.rotation, rot, mLerpRate * Time.deltaTime);
    }

    private void LerpEyesRotation(float rotAngle)
    {
        var rot = Quaternion.Euler(new Vector3(rotAngle, 0f, 0f));
        mEyeTs.localRotation = Quaternion.Lerp(mEyeTs.localRotation, rot, mLerpRate * Time.deltaTime);
    }

    private void UpdateEyesRotation()
    {
        if (isLocalPlayer)
            mEyeTs.localRotation = Quaternion.Euler(new Vector3(CamAngleX, 0f, 0f));
    }

    private static bool CheckIfBeyondThreshold(float rot1, float rot2)
    {
        return Mathf.Abs(rot1 - rot2) > SYNC_ROT_THRESHOLD;
    }

    [Command]
    private void CmdProvideRotationToServer(float playerRot, float camRot)
    {
        mSyncPlayerRotation = playerRot;
        mSyncCamRotation = camRot;
    }

    [Client]
    void OnPlayerRotSynced(float latestPlayerRot)
    {
        mSyncPlayerRotation = latestPlayerRot;
        mSyncPlayerRotList.Add(mSyncPlayerRotation);
    }
    
    [Client]
    void OnCamRotSynced(float latestCamRot)
    {
        mSyncCamRotation = latestCamRot;
        mSyncCamRotList.Add(mSyncCamRotation);
    }
}
