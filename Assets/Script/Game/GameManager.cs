using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Networking.Network;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private UIManager m_UIManager;
    public UIManager UIManager { get { return m_UIManager; } }

    private MoveControlManager mMoveManger;

    /// <summary>怪物管理類別</summary>
    private EnemyManager mEnemyManager;

    /// <summary>記綠玩家資訊</summary>
    private List<PlayerManager> mPlayerList = new List<PlayerManager>();

    /// <summary>自身玩家 ID</summary>
    public string LocalPlayerKey { get; private set; }

    void Awake()
    {
        Instance = this;

        InitComponent();
    }

    void OnDestroy()
    {
        Instance = null;
    }

    void Update()
    {
        if (isServer)
        {
            if (Input.GetKeyDown(KeyCode.Space)) mEnemyManager.SpawnEnemy();
        }

        //if (Input.GetKeyDown(KeyCode.Space)) DynamicGI.UpdateEnvironment();
        //if (Input.GetKeyDown(KeyCode.Alpha1)) BombManager.Instance.PutBomb(netId.ToString());
    }

    private void InitComponent()
    {
        mEnemyManager = gameObject.AddComponent<EnemyManager>();
    }

    public override void OnStartServer()
    {
        //base.OnStartServer();

        //StartGame();

        //NetworkServer.Spawn(Instantiate<GameObject>(Resources.Load<GameObject>("Game/BombManager")));
    }

    public static void AddPlayer(GameObject playerGo, LobbyPlayer lobbyPlayer, LobbyPlayerParam param)
    {
        if (Instance == null)
            return;

        NetworkIdentity networkIdentity = playerGo.GetComponent<NetworkIdentity>();

        Debug.LogFormat("netId:{0} netId:{1}", networkIdentity.netId, lobbyPlayer.isLocalPlayer);

        Instance.AddPlayer(new PlayerManager(playerGo, lobbyPlayer.slot, lobbyPlayer.playerControllerId, networkIdentity, param.RoleName, param.RoleColor));
    }

    private void AddPlayer(PlayerManager pm)
    {
        mPlayerList.Add(pm);
    }

    public static void SetLocalPlayer(GameObject playerGo)
    {
        if (Instance == null)
            return;

        foreach (var pm in Instance.mPlayerList)
        {
            if (pm.PlayerGo == playerGo)
            {
                Instance.LocalPlayerKey = pm.Key;

                Debug.Log("本機玩家編號:" + Instance.LocalPlayerKey);

                pm.SetLocalPlayer();
                break;
            }
        }
    }

    public static PlayerManager GetPlayerManager(string id)
    {
        if (Instance == null)
            return null;

        foreach (var pm in Instance.mPlayerList)
        {
            if (pm.Key == id)
                return pm;
        }

        return null;
    }

    /// <summary>遊戲開始</summary>
    private void StartGame()
    {
        mEnemyManager.BeginSpawnEnemy();
    }

    /*private IEnumerator UpdateEnvironment()
    {
        yield return new WaitForSeconds(1f);

        DynamicGI.UpdateEnvironment();
    }*/
}
