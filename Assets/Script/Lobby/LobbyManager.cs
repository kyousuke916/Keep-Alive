using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System;
using UnityEngine.Events;

namespace UnityStandardAssets.Network
{
    public class LobbyManager : NetworkLobbyManager
    {
        public event UnityAction OnUNetStartHost;
        public event UnityAction OnUNetClientConnect;

        
        public event UnityAction<NetworkConnection> OnUNetSceneChanged;
        
        public static LobbyManager s_Singleton;

        [Tooltip("The minimum number of players in the lobby before player can be ready")]
        public int minPlayer;

        private UInt64 _currentMatchID;
        public UInt64 CurrentMatchID { get { return _currentMatchID; } }

        public bool isMatchmaking = false;

        public bool _disconnectServer = false;

        //繼承 NetworkLobbyManager 不能使用 Awake 會出現 No Lobby for LobbyPlayer
        //public void Awake()
        //{

        //}

        void Start()
        {
            s_Singleton = this;

            //_lobbyHooks = GetComponent<UnityStandardAssets.Network.LobbyHook>();

            //DontDestroyOnLoad(gameObject);
        }

        void OnDestroy()
        {
            OnUNetStartHost = null;
        }

        #region override

        public override void OnStartHost()
        {
            base.OnStartHost();

            if (OnUNetStartHost != null)
                OnUNetStartHost();
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            Debug.Log("OnLobbyClientSceneChanged:" + conn);

            if (OnUNetSceneChanged != null)
                OnUNetSceneChanged(conn);
        }

        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            //return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);

            Debug.Log("===== OnLobbyServerCreateLobbyPlayer =====");
            Debug.Log("conn:" + conn);
            Debug.Log("playerControllerId:" + playerControllerId);
            Debug.Log("numPlayers:" + numPlayers);

            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

            LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();

            newPlayer.RpcToggleJoinButton(numPlayers + 1 >= minPlayer);

            for (int i = 0; i < numPlayers; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcToggleJoinButton(numPlayers + 1 >= minPlayer);
                }
            }

            return obj;
        }

        public override void OnMatchCreate(CreateMatchResponse matchInfo)
        {
            base.OnMatchCreate(matchInfo);

            //NetworkID
            _currentMatchID = (System.UInt64)matchInfo.networkId;

            Debug.Log("_currentMatchID:"+ _currentMatchID);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            if (OnUNetClientConnect != null)
                OnUNetClientConnect();
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

