using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using Networking.Network;
using System.Collections;

/// <summary>測試用</summary>
public class BombManager : NetworkBehaviour
{
    public static BombManager Instance { get; private set; }

    private const short MSG_TYPE = 1000;

    private NetworkClient mClient;

    void Awake()
    {
        Instance = this;

        mClient = LobbyManager.s_Singleton.client;

        //NetworkServer.RegisterHandler(MSG_TYPE, OnServerPostChatMessage);
    }

    void OnDestroy()
    {
        Instance = null;
    }

    void OnServerPostChatMessage(NetworkMessage netMsg)
    {
        RpcPutBomb(netMsg.ReadMessage<StringMessage>().value);
    }

    [Client]
    public void PutBomb(string bombID)
    {
        mClient.Send(MSG_TYPE, new StringMessage(bombID));
    }
    
    [ClientRpc]
    private void RpcPutBomb(string bombID)
    {
        //UIChat.Instance.InsertDialog(string.Format("put bomb:{0}", bombID));
    }
}
