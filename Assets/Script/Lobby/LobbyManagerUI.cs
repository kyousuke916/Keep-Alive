using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.Network;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;

public class LobbyManagerUI : MonoBehaviour
{
    #region Field

    [SerializeField]
    private RectTransform mainMenuPanel;

    [SerializeField]
    private RectTransform lobbyPanel;

    [SerializeField]
    private RectTransform lobbyServerList;

    //public LobbyInfoPanel infoPanel;

    [SerializeField]
    private RectTransform currentPanel;

    [SerializeField]
    private InputField m_RoonField;

    [SerializeField]
    private Button m_CreateBtn;

    [SerializeField]
    private Button m_ServerListBtn;

    [SerializeField]
    private LobbyManager m_LobbyManager;

    [SerializeField]
    private LobbyTopPanel m_LobbyTopPanel;

    #endregion

    #region Unity Event

    void Awake()
    {
        currentPanel = mainMenuPanel;

        //backButton.gameObject.SetActive(false);
        //GetComponent<Canvas>().enabled = true;

        SetServerInfo("Offline", "None");

        m_CreateBtn.onClick.AddListener(OnClickCreateMatchmakingGame);
        m_ServerListBtn.onClick.AddListener(OnClickOpenServerList);

        m_LobbyManager.OnUNetStartHost += OnUNetStartHost;
        m_LobbyManager.OnUNetClientConnect += OnUNetClientConnect;
        
        m_LobbyManager.OnUNetSceneChanged += OnUNetSceneChanged;
    }

    void OnDestroy()
    {
        m_CreateBtn.onClick.RemoveListener(OnClickCreateMatchmakingGame);
        m_ServerListBtn.onClick.RemoveListener(OnClickOpenServerList);

        m_LobbyManager.OnUNetStartHost -= OnUNetStartHost;
        m_LobbyManager.OnUNetClientConnect -= OnUNetClientConnect;

        m_LobbyManager.OnUNetSceneChanged -= OnUNetSceneChanged;
    }

    #endregion

    private void OnClickCreateMatchmakingGame()
    {
        m_LobbyManager.StartMatchMaker();

        string matchName = m_RoonField.text;
        uint matchSize = 3;
        bool matchAdvertise = true;
        string matchPassword = string.Empty;

        m_LobbyManager.matchMaker.CreateMatch(matchName, matchSize, matchAdvertise, matchPassword, m_LobbyManager.OnMatchCreate);

        //m_LobbyManager.backDelegate = lobbyManager.StopHost;
        //m_LobbyManager.isMatchmaking = true;
        //m_LobbyManager.DisplayIsConnecting();

        SetServerInfo("Matchmaker Host", m_LobbyManager.matchHost);
    }


    private void OnUNetStartHost()
    {
        Debug.Log("OnUNetStartHost");

        ChangeTo(lobbyPanel);
        //backDelegate = StopHostClbk;
        SetServerInfo("Hosting", m_LobbyManager.networkAddress);
    }

    private void OnUNetClientConnect()
    {
        m_LobbyTopPanel.gameObject.SetActive(false);

        if (!NetworkServer.active)
        {
            //only to do on pure client (not self hosting client)
            ChangeTo(lobbyPanel);
            //backDelegate = StopClientClbk;
            SetServerInfo("Client", m_LobbyManager.networkAddress);
        }
    }

    private void OnUNetSceneChanged(NetworkConnection conn)
    {
        if (!conn.playerControllers[0].unetView.isLocalPlayer)
            return;

        if (Application.loadedLevelName == m_LobbyManager.lobbyScene)
        {
            //if (m_LobbyTopPanel.isInGame)
            if (true)
            {
                ChangeTo(lobbyPanel);

                if (m_LobbyManager.isMatchmaking)
                {
                    if (conn.playerControllers[0].unetView.isServer)
                    {
                        //backDelegate = StopHostClbk;
                    }
                    else
                    {
                        //backDelegate = StopClientClbk;
                    }
                }
                else
                {
                    if (conn.playerControllers[0].unetView.isClient)
                    {
                        //backDelegate = StopHostClbk;
                    }
                    else
                    {
                        //backDelegate = StopClientClbk;
                    }
                }
            }
            else
            {
                ChangeTo(mainMenuPanel);
            }

            m_LobbyTopPanel.ToggleVisibility(true);
            m_LobbyTopPanel.isInGame = false;
        }
        else
        {
            ChangeTo(null);

            Destroy(GameObject.Find("MainMenuUI(Clone)"));

            //backDelegate = StopGameClbk;
            m_LobbyTopPanel.isInGame = true;
            m_LobbyTopPanel.ToggleVisibility(false);
        }
    }




    public void ChangeTo(RectTransform newPanel)
    {
        if (currentPanel != null)
        {
            currentPanel.gameObject.SetActive(false);
        }

        if (newPanel != null)
        {
            newPanel.gameObject.SetActive(true);
        }

        currentPanel = newPanel;

        if (currentPanel != mainMenuPanel)
        {
            //backButton.gameObject.SetActive(true);
        }
        else
        {
            //backButton.gameObject.SetActive(false);
            SetServerInfo("Offline", "None");
            m_LobbyManager.isMatchmaking = false;
        }
    }

    public void SetServerInfo(string status, string host)
    {
        Debug.Log(string.Format("SetServerInfo {0}:{1}", status, host));

        m_LobbyTopPanel.StatusInfo(status);
        m_LobbyTopPanel.HostInfo(host);
    }


    // ----------------- Server management

    public void SimpleBackClbk()
    {
        ChangeTo(mainMenuPanel);
    }

    public void StopHostClbk()
    {
        if (m_LobbyManager.isMatchmaking)
        {
            m_LobbyManager.matchMaker.DestroyMatch((NetworkID)m_LobbyManager.CurrentMatchID, OnMatchDestroyed);
            m_LobbyManager._disconnectServer = true;
        }
        else
        {
            m_LobbyManager.StopHost();
        }

        ChangeTo(mainMenuPanel);
    }

    public void OnMatchDestroyed(BasicResponse resp)
    {
        if (m_LobbyManager._disconnectServer)
        {
            m_LobbyManager.StopMatchMaker();
            m_LobbyManager.StopHost();
        }
    }

    public void StopClientClbk()
    {
        m_LobbyManager.StopClient();

        if (m_LobbyManager.isMatchmaking)
        {
            m_LobbyManager.StopMatchMaker();
        }

        ChangeTo(mainMenuPanel);
    }

    public void StopServerClbk()
    {
        m_LobbyManager.StopServer();

        ChangeTo(mainMenuPanel);
    }

    public void StopGameClbk()
    {
        m_LobbyManager.SendReturnToLobby();

        ChangeTo(lobbyPanel);
    }


    private void OnClickOpenServerList()
    {
        m_LobbyManager.StartMatchMaker();
        //m_LobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
        ChangeTo(lobbyServerList);
    }
}
