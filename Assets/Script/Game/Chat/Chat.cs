using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using Networking.Network;
using System;

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

        //mClient.Send(MSG_TYPE, new StringMessage(msg));
        NetworkInstanceId netID = GameManager.Instance == null ? NetworkInstanceId.Invalid : GameManager.GetLocalNetID;

        ChatMessage cm = new ChatMessage
        {
            NetID = netID,
            Msg = msg
        };

        mClient.Send(MSG_TYPE, cm);
    }

    [Server]
    void OnServerPostChatMessage(NetworkMessage netMsg)
    {
        var cm = netMsg.ReadMessage<ChatMessage>();

        string roleName = GameManager.GetRoleName(cm.NetID);

        string msg = cm.Msg;

        string data = string.Format("<color=#FFFF00>[{0}]</color><color=#00FF00>[{1}]</color>:{2}", DateTime.Now.ToString("HH:mm:ss"), roleName, msg);

        mChatLog.Add(data);

        //mChatLog.Add(netMsg.ReadMessage<StringMessage>().value);
    }

    private void OnChatUpdated(SyncListString.Operation op, int index)
    {
        if (UIChat.Instance != null)
            UIChat.Instance.InsertDialog(mChatLog[mChatLog.Count - 1].ToString());
    }

    public class ChatMessage : MessageBase
    {
        public NetworkInstanceId NetID = NetworkInstanceId.Invalid;
        public string Msg = string.Empty;
    }
}
