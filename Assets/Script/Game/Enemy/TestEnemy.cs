using UnityEngine;
using UnityEngine.Networking;

/// <summary>測試用敵人</summary>
public class TestEnemy : NetworkBehaviour
{
    [SerializeField]
    private float m_StopDistance = 1f;
    [SerializeField]
    private float m_MoveSpeed = 3f;

    private Transform mTargetTs;

    void Start()
    {
        enabled = isServer;
    }

    void Update()
    {
        if (mTargetTs == null)
            mTargetTs = GetTarget();

        Vector3 dis = (mTargetTs.position - transform.position);

        if (dis.magnitude < m_StopDistance)
            return;

        Vector3 dir = dis.normalized;
        dir.y = 0f;

        transform.forward = dir;
        transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime, Space.Self);
    }

    private Transform GetTarget()
    {
        var array = GameObject.FindGameObjectsWithTag(TagManager.PLAYER);
        if (array.Length == 0)
            return null;

        return array[Random.Range(0, array.Length)].transform;
    }
}
