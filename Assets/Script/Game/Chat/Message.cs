using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class Message : NetworkBehaviour 
{
    public SyncListString chatMessages = new SyncListString();
    public Text chatMessageText;

    public class MyMsgType
    {
        public static short chatMessageType = MsgType.Highest + 1;
    };

    public class ChatMessage : MessageBase
    {
        public string text;
    }

    // This is called when server starts
    public void RegisterHandlers()
    {
        NetworkServer.RegisterHandler(MyMsgType.chatMessageType, OnChatMessageRecieved);
    }

    // This is sent to the server
    public void SendChatMessage(InputField input)
    {
        ChatMessage chatMessage = new ChatMessage();
        chatMessage.text = input.text;
        input.text = string.Empty;

        //MyNetworkManager.myClient.Send(MyMsgType.chatMessageType, chatMessage);
    }

    // The server recieves and adds to the SyncListString
    public void OnChatMessageRecieved(NetworkMessage netMsg)
    {
        ChatMessage chatMessage = netMsg.ReadMessage<ChatMessage>();
        chatMessages.Add(chatMessage.text);
    }

    // Dirty update to display all messages
    public void Update()
    {
        if (chatMessages.Count > 0)
        {
            string allMessages = string.Empty;

            for (int i = 0; i < chatMessages.Count; i++)
            {
                allMessages = allMessages + chatMessages[i] + "\n";
            }

            chatMessageText.text = allMessages;
        }
    }
}
