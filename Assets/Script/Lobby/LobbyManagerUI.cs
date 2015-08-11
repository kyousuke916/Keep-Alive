using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Networking.Network;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;

public class LobbyManagerUI : MonoBehaviour
{
    #region Field

    public static LobbyManagerUI Instance;

    [SerializeField]
    private RectTransform mainMenuPanel;

    [SerializeField]
    private RectTransform lobbyPanel;

    [SerializeField]
    private RectTransform lobbyServerList;

    public LobbyInfoPanel infoPanel;

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

    public delegate void BackButtonDelegate();
    public BackButtonDelegate backDelegate;

    #endregion

    #region Unity Event

    void Awake()
    {
        Instance = this;

        currentPanel = mainMenuPanel;
        
        SetServerInfo("Offline", "None");

        m_CreateBtn.onClick.AddListener(OnClickCreateMatchmakingGame);
        m_ServerListBtn.onClick.AddListener(OnClickOpenServerList);

        m_LobbyManager.OnUNetStartHost += OnUNetStartHost;
        m_LobbyManager.OnUNetClientConnect += OnUNetClientConnect;
        
        m_LobbyManager.OnUNetSceneChanged += OnUNetSceneChanged;
    }

    void OnDestroy()
    {
        Instance = null;

        m_CreateBtn.onClick.RemoveListener(OnClickCreateMatchmakingGame);
        m_ServerListBtn.onClick.RemoveListener(OnClickOpenServerList);

        m_LobbyManager.OnUNetStartHost -= OnUNetStartHost;
        m_LobbyManager.OnUNetClientConnect -= OnUNetClientConnect;

        m_LobbyManager.OnUNetSceneChanged -= OnUNetSceneChanged;
    }

    #endregion

    #region Event - Button

    private void OnClickCreateMatchmakingGame()
    {
        m_LobbyManager.StartMatchMaker();

        NetworkMatch match = m_LobbyManager.matchMaker;

        if (match == null)
        {
            Debug.LogError("m_LobbyManager.matchMaker == null");
            return;
        }

        var matchName = m_RoonField.text;
        var matchSize = 3u;
        var matchAdvertise = true;
        var matchPassword = string.Empty;

        var request = new CreateMatchRequest();
        request.name = matchName;
        request.size = matchSize;
        request.advertise = matchAdvertise;
        request.password = matchPassword;

        match.CreateMatch(request, m_LobbyManager.OnMatchCreate);
        //match.CreateMatch(matchName, matchSize, matchAdvertise, matchPassword, m_LobbyManager.OnMatchCreate);

        m_LobbyManager.isMatchmaking = true;

        DisplayIsConnecting();

        backDelegate = m_LobbyManager.StopHost;

        SetServerInfo("Matchmaker Host", m_LobbyManager.matchHost);
    }

    #endregion

    #region Event - LobbyManager

    private void OnUNetStartHost()
    {
        //Debug.Log("OnUNetStartHost");

        ChangeTo(lobbyPanel);
        backDelegate = StopHostClbk;
        SetServerInfo("Hosting", m_LobbyManager.networkAddress);
    }

    private void OnUNetClientConnect()
    {
        infoPanel.gameObject.SetActive(false);

        if (!NetworkServer.active)
        {
            //only to do on pure client (not self hosting client)
            ChangeTo(lobbyPanel);
            backDelegate = StopClientClbk;
            SetServerInfo("Client", m_LobbyManager.networkAddress);
        }
    }

    private void OnUNetSceneChanged(NetworkConnection conn)
    {
        if (!conn.playerControllers[0].unetView.isLocalPlayer)
            return;

        if (Application.loadedLevelName == m_LobbyManager.lobbyScene)
        {
            if (m_LobbyTopPanel.isInGame)
            {
                ChangeTo(lobbyPanel);

                if (m_LobbyManager.isMatchmaking)
                {
                    if (conn.playerControllers[0].unetView.isServer)
                    {
                        backDelegate = StopHostClbk;
                    }
                    else
                    {
                        backDelegate = StopClientClbk;
                    }
                }
                else
                {
                    if (conn.playerControllers[0].unetView.isClient)
                    {
                        backDelegate = StopHostClbk;
                    }
                    else
                    {
                        backDelegate = StopClientClbk;
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

            backDelegate = StopGameClbk;
            m_LobbyTopPanel.isInGame = true;
            m_LobbyTopPanel.ToggleVisibility(false);
        }
    }

    #endregion

    #region Other

    public void DisplayIsConnecting()
    {
        var _this = this;
        infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
    }

    public void ChangeTo(RectTransform newPanel)
    {
        if (currentPanel != null)
            currentPanel.gameObject.SetActive(false);

        if (newPanel != null)
            newPanel.gameObject.SetActive(true);

        currentPanel = newPanel;

        if (currentPanel != mainMenuPanel)
        {
            m_LobbyTopPanel.ShowBackBtn(true);
        }
        else
        {
            m_LobbyTopPanel.ShowBackBtn(false);

            SetServerInfo("Offline", "None");

            m_LobbyManager.isMatchmaking = false;
        }
    }

    public void SetServerInfo(string status, string host)
    {
        //Debug.Log(string.Format("SetServerInfo {0}:{1}", status, host));

        m_LobbyTopPanel.StatusInfo(status);
        m_LobbyTopPanel.HostInfo(host);
    }

    public void GoBackButton()
    {
        if (backDelegate != null)
            backDelegate();
    }

    #endregion

    #region Server Management

    private void SimpleBackClbk()
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

    /// <summary>停止配桌，返回主畫面</summary>
    public void StopClientClbk()
    {
        m_LobbyManager.StopClient();

        if (m_LobbyManager.isMatchmaking)
            m_LobbyManager.StopMatchMaker();

        ChangeTo(mainMenuPanel);
    }

    /// <summary>停止 Server，返回主畫面</summary>
    public void StopServerClbk()
    {
        m_LobbyManager.StopServer();

        ChangeTo(mainMenuPanel);
    }

    /// <summary>停止遊戲，返回大廳</summary>
    public void StopGameClbk()
    {
        m_LobbyManager.SendReturnToLobby();

        ChangeTo(lobbyPanel);
    }
    
    /// <summary>開啟房間列表</summary>
    private void OnClickOpenServerList()
    {
        m_LobbyManager.StartMatchMaker();

        backDelegate = SimpleBackClbk;

        ChangeTo(lobbyServerList);
    }

    #endregion
}
