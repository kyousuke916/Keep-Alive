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

        #region For Lobby

        #region Override Server

        /// <summary>Called on the server when a client is ready</summary>
        public override void OnServerReady(NetworkConnection conn)
        {
            Log("OnServerReady:{0}", conn);

            base.OnServerReady(conn);

            //NetworkServer.SetClientReady(conn);
        }

        /// <summary>This hook is invoked when a host is started.</summary>
        public override void OnStartHost()
        {
            Log("OnStartHost");

            base.OnStartHost();

            if (OnUNetStartHost != null)
                OnUNetStartHost();
        }

        /// <summary>This is invoked when a match has been created.</summary>
        public override void OnMatchCreate(CreateMatchResponse matchInfo)
        {
            Log("OnMatchCreate");

            base.OnMatchCreate(matchInfo);

            //Log("success:{0}", matchInfo.success);
            //Log("address:{0}", matchInfo.address);
            //Log("port:{0}", matchInfo.port);
            //Log("accessTokenString:{0}" + matchInfo.accessTokenString);
            //Log("networkId:{0}", matchInfo.networkId);
            //Log("nodeId:{0}", matchInfo.nodeId);
            //Log("usingRelay:{0}", matchInfo.usingRelay);
            //Log("extendedInfo:{0}", matchInfo.extendedInfo);

            //NetworkID
            _currentMatchID = (System.UInt64)matchInfo.networkId;

            //Debug.Log("_currentMatchID:"+ _currentMatchID);
        }

        /// <summary>This allows customization of the creation of the lobby-player object on the server.</summary>
        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            //return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);

            Log("OnLobbyServerCreateLobbyPlayer");

            //return null;
            
            //Log("conn:{0}", conn);
            //Log("playerControllerId:{0}", playerControllerId);
            //Log("numPlayers:{0}", numPlayers);

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
            Log("ServerChangeScene:{0}", sceneName);

            base.ServerChangeScene(sceneName);
        }

        /// <summary>This is called on the server when a networked scene finishes loading</summary>
        public override void OnLobbyServerSceneChanged(string sceneName)
        {
            Log("OnLobbyServerSceneChanged:{0}", sceneName);

            base.OnLobbyServerSceneChanged(sceneName);
        }

        /// <summary>This is called on the server when it is told that a client has finished switching from the lobby scene to a game player scene.</summary>
        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayerGo, GameObject gamePlayerGo)
        {
            Log("OnLobbyServerSceneLoadedForPlayer =>" + lobbyPlayerGo + " : " + gamePlayerGo);

            LobbyPlayer lobbyPlayer = lobbyPlayerGo.GetComponent<LobbyPlayer>();
            LobbyPlayerParam param = lobbyPlayer.Param;
            
            GameManager.AddPlayer(gamePlayerGo, lobbyPlayer, param);

            return true;

            //return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayerGo, gamePlayerGo);
        }
        
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            base.OnServerAddPlayer(conn, playerControllerId);

            Log("OnServerAddPlayer");
            //Log("connectionId:{0}", conn.connectionId);
            //Log("logNetworkMessages:{0}", conn.logNetworkMessages);
        }
        
        /// <summary>This is called on the server when a client disconnects</summary>
        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {
            Log("OnLobbyServerDisconnect:{0}", conn);

            for (int i = 0; i < numPlayers; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                    p.RpcToggleJoinButton(numPlayers >= minPlayer);
            }
        }

        /// <summary>Called on the client when connected to a server</summary>
        public override void OnClientConnect(NetworkConnection conn)
        {
            Log("OnClientConnect:{0}", conn);

            base.OnClientConnect(conn);

            if (OnUNetClientConnect != null)
                OnUNetClientConnect();
        }

        /// <summary>Called on clients when a network error occurs.</summary>
        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            Log("OnClientError");

            //ChangeTo(mainMenuPanel);
            //infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        }

        #endregion

        #region Override Client

        public override void OnLobbyClientEnter()
        {
            Log("OnLobbyClientEnter");
            base.OnLobbyClientEnter();
        }

        /// <summary>This is called on the client when the client is finished loading a new networked scene</summary>
        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            Log("OnLobbyClientSceneChanged:{0}", conn);

            if (OnUNetSceneChanged != null)
                OnUNetSceneChanged(conn);
        }

        /// <summary>Called on clients when a servers tells the client it is no longer ready</summary>
        public override void OnClientNotReady(NetworkConnection conn)
        {
            Log("OnClientNotReady:{0}", conn);

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

        #endregion

        #region For Game

        public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
        {
            Log("OnLobbyServerCreateGamePlayer");
            Log("connectionId:{0}", conn.connectionId);
            Log("playerControllerId:{0}", playerControllerId);

            return Instantiate<GameObject>(gamePlayerPrefab);
        }
        
        #endregion

        #region Debug

        //private static void Log(string data)
        //{
        //    Debug.Log("L == " + data);
        //}

        private static void Log(string format, params object[] args)
        {
            Debug.LogFormat(string.Format("LM:{0}", format), args);
        }

        #endregion

    }
}

