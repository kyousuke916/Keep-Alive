using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System;
using UnityEngine.Events;

namespace Networking.Network
{
    public class LobbyManager : NetworkLobbyManager
    {
        #region Event

        public event UnityAction OnUNetStartHost;
        public event UnityAction OnUNetClientConnect;
        
        public event UnityAction<NetworkConnection> OnUNetSceneChanged;

        #endregion

        #region Field

        public static LobbyManager s_Singleton;

        //[Tooltip("The minimum number of players in the lobby before player can be ready")]
        private int minPlayer = 1;

        private UInt64 _currentMatchID;
        public UInt64 CurrentMatchID { get { return _currentMatchID; } }

        public bool isMatchmaking = false;

        public bool _disconnectServer = false;

        #endregion

        #region Unity Event

        //繼承 NetworkLobbyManager 不能使用 Awake 會出現 No Lobby for LobbyPlayer
        //public void Awake()
        //{

        //}

        void Start()
        {
            s_Singleton = this;

            //_lobbyHooks = GetComponent<UnityStandardAssets.Network.LobbyHook>();
        }

        void OnDestroy()
        {
            OnUNetStartHost = null;
        }

        #endregion

        #region Override Server

        /// <summary>Called on the server when a client is ready</summary>
        public override void OnServerReady(NetworkConnection conn)
        {
            Debug.Log("== OnServerReady:" + conn);

            base.OnServerReady(conn);

            //NetworkServer.SetClientReady(conn);
        }

        /// <summary>This hook is invoked when a host is started.</summary>
        public override void OnStartHost()
        {
            Debug.Log("== OnStartHost");

            base.OnStartHost();

            if (OnUNetStartHost != null)
                OnUNetStartHost();
        }

        /// <summary>This is invoked when a match has been created.</summary>
        public override void OnMatchCreate(CreateMatchResponse matchInfo)
        {
            base.OnMatchCreate(matchInfo);

            Debug.Log("== OnMatchCreate");
            //Debug.Log("success:" + matchInfo.success);
            //Debug.Log("address:" + matchInfo.address);
            //Debug.Log("port:" + matchInfo.port);
            //Debug.Log("accessTokenString:" + matchInfo.accessTokenString);
            //Debug.Log("networkId:" + matchInfo.networkId);
            //Debug.Log("nodeId:" + matchInfo.nodeId);
            //Debug.Log("usingRelay:" + matchInfo.usingRelay);
            //Debug.Log("extendedInfo:" + matchInfo.extendedInfo);

            //NetworkID
            _currentMatchID = (System.UInt64)matchInfo.networkId;

            //Debug.Log("_currentMatchID:"+ _currentMatchID);
        }

        /// <summary>This allows customization of the creation of the lobby-player object on the server.</summary>
        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            //return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);

            Debug.Log("== OnLobbyServerCreateLobbyPlayer");
            //Debug.Log("conn:" + conn);
            //Debug.Log("playerControllerId:" + playerControllerId);
            Debug.Log("numPlayers:" + numPlayers);

            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

            LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();

            bool readyBtnEnable = numPlayers + 1 >= minPlayer;

            newPlayer.RpcToggleJoinButton(readyBtnEnable);

            for (int i = 0; i < numPlayers; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                    p.RpcToggleJoinButton(readyBtnEnable);
            }

            return obj;
        }

        /// <summary>This causes the server to switch scenes and sets the networkSceneId</summary>
        public override void ServerChangeScene(string sceneName)
        {
            Debug.Log("== ServerChangeScene:" + sceneName);

            base.ServerChangeScene(sceneName);
        }

        /// <summary>This is called on the server when a networked scene finishes loading</summary>
        public override void OnLobbyServerSceneChanged(string sceneName)
        {
            Debug.Log("== OnLobbyServerSceneChanged:" + sceneName);

            base.OnLobbyServerSceneChanged(sceneName);
        }

        /// <summary>This is called on the server when it is told that a client has finished switching from the lobby scene to a game player scene.</summary>
        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            Debug.Log("== OnLobbyServerSceneLoadedForPlayer =>" + lobbyPlayer + " : " + lobbyPlayer);

            return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
        }

        /// <summary>This is called on the server when a client disconnects</summary>
        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {
            for (int i = 0; i < numPlayers; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcToggleJoinButton(numPlayers >= minPlayer);
                }
            }
        }
        /// <summary>Called on the client when connected to a server</summary>
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            if (OnUNetClientConnect != null)
                OnUNetClientConnect();
        }

        /// <summary>Called on clients when a network error occurs.</summary>
        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            Debug.Log("== OnClientError");

            //ChangeTo(mainMenuPanel);
            //infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        }

        #endregion

        #region Override Client

        /// <summary>This is called on the client when the client is finished loading a new networked scene</summary>
        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            Debug.Log("== OnLobbyClientSceneChanged:" + conn);

            if (OnUNetSceneChanged != null)
                OnUNetSceneChanged(conn);
        }

        /// <summary>Called on clients when a servers tells the client it is no longer ready</summary>
        public override void OnClientNotReady(NetworkConnection conn)
        {
            Debug.Log("== OnClientNotReady:" + conn);

            base.OnClientNotReady(conn);
        }

        #endregion

        public void OnMatchDestroyed(BasicResponse resp)
        {
            if (_disconnectServer)
            {
                StopMatchMaker();
                StopHost();
            }
        }
    }
}

