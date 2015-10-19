using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Networking.Network;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    /// <summary>記綠玩家資訊(For Server)</summary>
    private static List<PlayerManager> PLAYER_LIST = new List<PlayerManager>();

    [SerializeField]
    private UIManager m_UIManager;
    public UIManager UIManager { get { return m_UIManager; } }

    private MoveControlManager mMoveManger;

    /// <summary>怪物管理類別</summary>
    private EnemyManager mEnemyManager;

    public string LocalPlayerKey { get { return LocalPlayerSlot.ToString(); } }

    /// <summary>自身玩家 ID</summary>
    //public string LocalPlayerRoleName { get; private set; }

    /// <summary>自身玩家 NetID</summary>
    public NetworkInstanceId LocalPlayerNetID { get; private set; }

    /// <summary>自身玩家 Slot</summary>
    public int LocalPlayerSlot { get; private set; }

    void Awake()
    {
        Instance = this;

        InitComponent();
    }

    void OnDestroy()
    {
        Instance = null;
        PLAYER_LIST.Clear();
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

    #region For Server

    public static void AddPlayer(GameObject playerGo, LobbyPlayer lobbyPlayer, LobbyPlayerParam param)
    {
        UIDebug.Log("AddPlayer Slot:{0} isLocalPlayer:{1} RoleName:{2} Instance:{3}", param.Slot, param.isLocalPlayer, param.RoleName, Instance);

        PLAYER_LIST.Add(new PlayerManager(playerGo, param.Slot, param.RoleName, param.RoleColor));
    }

    //public void AAAA(int slot, string roleName)
    //{
    //    Instance.RpcAddPlayer(slot, roleName);
    //}

    //[ClientRpc]
    //private void RpcAddPlayer(int slot, string roleName)
    //{
    //    Debug.LogFormat("RpcAddPlayer slot:{0} roleName:{1}",slot, roleName);
    //    AddPlayer(new PlayerManager(null, slot, roleName, Color.white));
    //}

    public static void SetLocalPlayer(GameObject playerGo, NetworkInstanceId netId)
    {
        //Log("SetLocalPlayer netId:{0} Instance:{1}", netId, Instance);

        /*if (Instance == null)
            return;

        foreach (var pm in Instance.mPlayerList)
        {
            if (pm.PlayerGo == playerGo)
            {
                Instance.LocalPlayerKey = pm.Key;
                Instance.LocalPlayerNetID = netId;
                Instance.LocalPlayerSlot = pm.Slot;

                Log("本機玩家編號:{0}", Instance.LocalPlayerKey);
                Log("本機玩家 NetID:{0}", Instance.LocalPlayerNetID);
                Log("本機玩家 Slot:{0}", Instance.LocalPlayerSlot);

                pm.SetLocalPlayer();

                break;
            }
        }*/
    }

    #endregion

    public static void SetLocalParam(NetworkInstanceId netId)
    {
        Log("============== SetLocalParam:{0}", netId);
        if (Instance == null)
            return;
        
        Instance.LocalPlayerNetID = netId;
        //Instance.LocalPlayerSlot = slot;
    }

    public static PlayerManager GetPlayerManager(string id)
    {
        if (Instance == null)
            return null;

        foreach (var pm in PLAYER_LIST)
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

    #region GamePlayer Helper

    /// <summary>取得自己角色 NetID(目前是暴力找，待優化)</summary>
    public static NetworkInstanceId GetLocalNetID
    {
        get
        {
            foreach (var gamePlayer in GameObject.FindObjectsOfType<GamePlayer>())
            {
                if (gamePlayer.isLocalPlayer)
                    return gamePlayer.netId;
            }

            return NetworkInstanceId.Invalid;
        }
    }

    /// <summary>取得角色名稱</summary>
    public static string GetRoleName(NetworkInstanceId netID)
    {
        foreach (var item in PLAYER_LIST)
        {
            if (item.NetID.Equals(netID))
                return item.RoleName;
        }

        return "unknow";
    }

    #endregion

    #region Debug

    private static void Log(string format, params object[] args)
    {
        Debug.LogFormat(string.Format("GM:{0}", format), args);
    }

    #endregion
}
