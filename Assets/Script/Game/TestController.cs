#define DavidTest

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>測試用同步操控角色類別</summary>
public class TestController : NetworkBehaviour
{
#if DavidTest

    public delegate void TakeDamageDelegate(int damage);
    public float speed = 10f;
    public float rotationSpeed = 100f;

    [SyncEvent(channel = 1)]
    public event TakeDamageDelegate EventTakeDamage;

    void Awake()
    {
        enabled = false;
    }

    void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var damage = Random.Range(10, 50);
            Debug.Log("CmdDoFire:damage:" + damage);
            CmdDoFire(damage);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var damage = Random.Range(10, 50);
            Debug.Log("RpcDoSomething:damage:" + damage);
            RpcDoSomething(damage);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var damage = Random.Range(10, 50);
            Debug.Log("TakeDamage:damage:" + damage);
            TakeDamage(damage);
        }
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

    private void Log(string data)
    {
        //Debug.LogWarning(data);
    }

#endif
}
