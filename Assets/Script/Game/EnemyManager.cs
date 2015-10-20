using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyManager : MonoBehaviour
{

    public void BeginSpawnEnemy()
    {
        StartCoroutine(LoopSpawnEnemy());
    }

    private IEnumerator LoopSpawnEnemy()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f);
        }
    }

    public void SpawnEnemy()
    {
        GameObject prefab = Resources.Load<GameObject>("Game/Enemy/TestEnemy");
        GameObject enemyGo = Instantiate<GameObject>(prefab);
        enemyGo.transform.position = new Vector3(0f, 1f, 0f);

        NetworkServer.Spawn(enemyGo);
    }
}
