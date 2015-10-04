using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using Networking.Network;

public class Chat : NetworkBehaviour
{
    public static Chat Instance { get; private set; }

    private const short MSG_TYPE = 1000;

    private SyncListString mChatLog = new SyncListString();

    private NetworkClient mClient;

    void Awake()
    {
        Instance = this;

        mClient = LobbyManager.s_Singleton.client;

        NetworkServer.RegisterHandler(MSG_TYPE, OnServerPostChatMessage);
    }

    public override void OnStartClient()
    {
        mChatLog.Callback = OnChatUpdated;
    }

    void OnDestroy()
    {
        Instance = null;

        NetworkServer.UnregisterHandler(MSG_TYPE);
    }

    [Client]
    public void PostChatMessage(string msg)
    {
        if (string.IsNullOrEmpty(msg))
            return;
       
        mClient.Send(MSG_TYPE, new StringMessage(msg));
    }

    [Server]
    void OnServerPostChatMessage(NetworkMessage netMsg)
    {
        mChatLog.Add(netMsg.ReadMessage<StringMessage>().value);
    }

    private void OnChatUpdated(SyncListString.Operation op, int index)
    {
        if (UIChat.Instance != null)
            UIChat.Instance.InsertDialog(mChatLog[mChatLog.Count - 1].ToString());
    }
}
