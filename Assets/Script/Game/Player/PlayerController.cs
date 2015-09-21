using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


public class PlayerController : NetworkBehaviour
{
    /// <summary> 攝影機碰撞器</summary>
    public CollisionCounter CameraSensor;

    public Camera MainCamera;

    public GameObject Player;
    public Rigidbody PlayerRigidBody;

    public Transform CameraPoint;

    public float Maxdistance;

    public float MoveSpeed;

    private bool mIsHoriMove;
    private bool mIsVertiMove;

    private Vector3 mHoriVelocity;
    private Vector3 mVertiVelocity;

    private float mTime;

    private float mCameraPosYValue = 0f;

    private HitControlManager mHitManger;
    private MoveControlManager mMoveManger;

    private GamePlayer mGamePlayer;

    public bool Listencontroll = false;

    void Awake()
    {
        enabled = false;

        mGamePlayer = GetComponent<GamePlayer>();

        MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        CameraSensor = MainCamera.GetComponent<CollisionCounter>();
        
    }
    public void uiSet()
    {
        var mmm = GameObject.Instantiate(Resources.Load("Game/UI/UI_Prefab")) as GameObject;
        mMoveManger = mmm.GetComponentInChildren<MoveControlManager>();
        mHitManger = mmm.GetComponentInChildren<HitControlManager>();

        mHitManger.OnHitAction += PlayerHitAction;
        mHitManger.OnDragAction += FixPlayerCam;
    }

    private void PlayerHitAction(int action)
    {
        mGamePlayer.UseSkill(action);
    }

    private void FixPlayerCam(Vector2 value)
    {
        mCameraPosYValue = value.y;
    }

    void Update()
    {
        if (!Listencontroll)
            return;

        float vertical = mMoveManger.Y;
        float horizontal = mMoveManger.X;

        Vector3 forward = MainCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 distance = (MainCamera.transform.position - Player.transform.position);
        distance.y = 0;

        Vector3 right = MainCamera.transform.right;
        right.y = 0;
        right.Normalize();

        if (vertical > 0)
        {
            mVertiVelocity = forward * MoveSpeed;
            if (distance.magnitude > Maxdistance)
            {
                /*
                float originY = CameraSensorObj.transform.position.y;
                Vector3 pos = Player.transform.position + distance.normalized * Maxdistance;
                pos.y = originY;
                CameraSensorObj.transform.position = pos;
                */
                float originY = MainCamera.transform.position.y;
                Vector3 pos = Player.transform.position + distance.normalized * Maxdistance;
                pos.y = originY;
                MainCamera.transform.position = pos;
            }

            mIsVertiMove = true;
        }
        else if (vertical < 0)
        {
            mVertiVelocity = -forward * MoveSpeed;
            if (CameraSensor.Counter <= 0)
            {
                /*
                float originY = CameraSensorObj.transform.position.y;
                Vector3 pos = Player.transform.position + distance.normalized * Maxdistance;
                pos.y = originY;
                CameraSensorObj.transform.position = pos;
                */

                float originY = MainCamera.transform.position.y;
                Vector3 pos = Player.transform.position + distance.normalized * Maxdistance;
                pos.y = originY;
                MainCamera.transform.position = pos;
            }

            mIsVertiMove = true;
        }
        else
        {
            mIsVertiMove = false;
        }

        if (horizontal > 0)
        {
            mHoriVelocity = right * MoveSpeed;
            mIsHoriMove = true;
        }
        else if (horizontal < 0)
        {
            mHoriVelocity = -right * MoveSpeed;
            mIsHoriMove = true;
        }
        else
        {
            mIsHoriMove = false;
        }

        if (mIsHoriMove && mIsVertiMove)
        {
            PlayerRigidBody.velocity = mHoriVelocity + mVertiVelocity;
        }
        else if (mIsHoriMove)
        {
            PlayerRigidBody.velocity = mHoriVelocity;
        }
        else if (mIsVertiMove)
        {
            PlayerRigidBody.velocity = mVertiVelocity;
        }

        if (mIsHoriMove || mIsVertiMove)
        {
            float rotate = Mathf.Atan2(PlayerRigidBody.velocity.x, PlayerRigidBody.velocity.z);
            Player.transform.rotation = Quaternion.Euler(0, rotate / Mathf.PI * 180, 0);
            mTime = 0.1f;
        }
        if (mTime >= 0)
        {
            mTime -= Time.deltaTime;
        }
        else
        {
            PlayerRigidBody.velocity = Vector3.zero;
        }

        mGamePlayer.SetCameraAngleX(mCameraPosYValue);
    }

    public override void OnStartLocalPlayer()
    {
        Log("== OnStartLocalPlayer:" + netId);

        enabled = isLocalPlayer;
    }

    public override void OnStartServer()
    {
        Log("== OnStartServer:" + netId);
    }

    public override void OnStartClient()
    {
        name = string.Format("Player[{0}]", netId);

        Log("== OnStartClient:" + netId);
    }

    public override bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
    {
        Log("== OnRebuildObservers:" + name);

        return base.OnRebuildObservers(observers, initialize);
    }

    public override void OnSetLocalVisibility(bool vis)
    {
        Log("== OnSetLocalVisibility:" + vis);

        base.OnSetLocalVisibility(vis);
    }

    public override bool OnCheckObserver(NetworkConnection conn)
    {
        Log("== OnCheckObserver:" + conn);

        return base.OnCheckObserver(conn);
    }
    /*
    [Command]
    private void CmdDoFire(int damage)
    {
        Debug.Log("DoFire:" + damage);
    }

    [ClientRpc]
    private void RpcDoSomething(int damage)
    {
        Debug.Log("RpcDoSomething:" + damage);
    }

    [Server]
    void TakeDamage(int damage)
    {
        if (EventTakeDamage != null)
            EventTakeDamage(damage);
    }
    */
    private void Log(string data)
    {
        //Debug.LogWarning(data);
    }
}