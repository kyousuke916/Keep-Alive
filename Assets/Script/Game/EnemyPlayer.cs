using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyPlayer : NetworkBehaviour
{

    public NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding

    [SerializeField]
    private float m_StopDistance = 1f;
    [SerializeField]
    private float m_MoveSpeed = 3f;

    private Transform mTarget; // target to aim for

    // Use this for initialization
    private void Start()
    {
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.speed = m_MoveSpeed;
        agent.updateRotation = false;
        agent.updatePosition = true;

        SetTarget();
    }

    public void SetTarget()
    {
        mTarget = GetTarget();
    }

    // Update is called once per frame
    private void Update()
    {
        if (mTarget != null)
        {

            Vector3 dis = (mTarget.position - transform.position);

            if (dis.magnitude < m_StopDistance)
            {
                mTarget = GetTarget();
                return;
            }

            agent.SetDestination(mTarget.position);

        }
      
    }

    private Transform GetTarget()
    {
        var array = GameObject.FindGameObjectsWithTag(TagManager.SPAWN_POINT);
        if (array.Length == 0)
            return null;

        return array[UnityEngine.Random.Range(0, array.Length)].transform;
    }
    
}
