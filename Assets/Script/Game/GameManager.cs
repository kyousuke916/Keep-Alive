using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    private MoveControlManager mMoveManger;
    private EnemyManager mEnemyManager;

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
        //if (Input.GetKeyDown(KeyCode.Space)) DynamicGI.UpdateEnvironment();

        if (isServer)
        {
            if (Input.GetKeyDown(KeyCode.Space)) mEnemyManager.SpawnEnemy();
        }
    }

    private void InitComponent()
    {
        mEnemyManager = gameObject.AddComponent<EnemyManager>();
    }

    public override void OnStartServer()
    {
        //base.OnStartServer();
        //StartGame();
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
