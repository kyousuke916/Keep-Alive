using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


public class PlayerController : NetworkBehaviour
{
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

    private GamePlayer mGamePlayer;

    private float mX = 0f;

    private float mY = 0f;

    public bool Listencontroll = false;

    void Awake()
    {
        enabled = false;

        mGamePlayer = GetComponent<GamePlayer>();
    }

    void Update()
    {
        float translation = mY * 10f;

        float rotation = mX * 80f;

        translation *= Time.deltaTime;

        rotation *= Time.deltaTime;

        transform.Translate(0, 0, translation);

        transform.Rotate(0, rotation, 0);
    }

    public void Move(float x, float y)
    {
        mX = x;
        mY = y;
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