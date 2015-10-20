using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;

//www.gabrielgambetta.com/fpm1.html

[NetworkSettings(channel = 0, sendInterval = 0.033f)]
//[NetworkSettings(channel = 0, sendInterval = 1f)]
//[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class Player_SyncPosition : NetworkBehaviour
{
    private const float SYNC_POS_THRESHOLD = 0.1f;
    private const int SYNC_POS_SAFETY_NUM = 10;
    private const float CLOSE_ENOUGH = 0.1f;

    [SyncVar(hook = "SyncPositionValues")]
    private Vector3 mSyncPos;

    private float mLerpRate;
    private float mNormalLerpRate = 16f;
    private float mFastLerpRate = 27f;

    private Transform mTs;

    private Vector3 mLastPos;

    //private NetworkClient nClient;

    private List<Vector3> mSyncPosList = new List<Vector3>();

    private bool mUseHistoricalLerping = false;

    void Awake()
    {
        mTs = transform;

        //nClient = Networking.Network.LobbyManager.s_Singleton.client;
        mLerpRate = mNormalLerpRate;
    }

    void FixedUpdate()
    {
        TransmitPosition();
    }

    void Update()
    {
        LerpPosition();
    }

    /// <summary>同步位置給其他人</summary>
    [ClientCallback]
    private void TransmitPosition()
    {
        if (isLocalPlayer && Vector3.Distance(mTs.position, mLastPos) > SYNC_POS_THRESHOLD)
        {
            CmdProvidePositionToServer(mTs.position);
            mLastPos = mTs.position;
        }
    }

    /// <summary>平滑位置</summary>
    private void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            if (mUseHistoricalLerping)
                HistoricalLerping();
            else
                OrdinaryLerping();
        }
    }

    [Command]
    private void CmdProvidePositionToServer(Vector3 pos)
    {
        mSyncPos = pos;
    }

    [Client]
    private void SyncPositionValues(Vector3 latestPos)
    {
        mSyncPos = latestPos;
        mSyncPosList.Add(mSyncPos);
    }

    private void OrdinaryLerping()
    {
        mTs.position = Vector3.Lerp(mTs.position, mSyncPos, Time.deltaTime * mLerpRate);
    }

    private void HistoricalLerping()
    {
        if (mSyncPosList.Count > 0)
        {
            mTs.position = Vector3.Lerp(mTs.position, mSyncPosList[0], Time.deltaTime * mLerpRate);
            
            if (Vector3.Distance(mTs.position, mSyncPosList[0]) < CLOSE_ENOUGH)
                mSyncPosList.RemoveAt(0);

            if (mSyncPosList.Count > SYNC_POS_SAFETY_NUM)
                mLerpRate = mFastLerpRate;
            else
                mLerpRate = mNormalLerpRate;
        }
    }

    /*void ShowLatency()
    {
        if (isLocalPlayer)
        {
            //latency = nClient.GetRTT();
            //latencyText.text = latency.ToString();
        }
    }*/
}
