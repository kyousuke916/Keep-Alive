using UnityEngine;
using System.Collections;

public class EnemyPlayer : MonoBehaviour {

    public NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding
    public Transform target; // target to aim for

    // Use this for initialization
    private void Start()
    {
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updatePosition = true;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    // Update is called once per frame
    private void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);

        }
        else
        {

        }

    }


    
}
