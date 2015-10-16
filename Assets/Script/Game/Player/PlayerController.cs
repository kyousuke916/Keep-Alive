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


        float translation = Input.GetAxis("Vertical") * 5;
        float rotation = Input.GetAxis("Horizontal") * 5;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);  

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