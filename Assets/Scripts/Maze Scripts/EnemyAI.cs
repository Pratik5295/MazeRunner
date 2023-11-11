using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Transform target;

    private Vector3 startingPosition;

    [SerializeField] public bool testFlag = false;

    [SerializeField] private Vector3 targetPosition;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        startingPosition = transform.position;

        
    }

    private void Update()
    {
        if (Vector3.Distance(this.transform.position, target.transform.position) < 0.5f)
        {
            //Go back to starting
            agent.SetDestination(startingPosition);
            targetPosition = startingPosition;
            testFlag = false;
        }
        else if (Vector3.Distance(this.transform.position, startingPosition) < 0.5f)
        {
            agent.SetDestination(target.transform.position);
            targetPosition = target.position;
            testFlag = true;
        }

        if (!agent.hasPath)
        {
            agent.SetDestination(target.position);
            targetPosition = target.position;
            testFlag = true;
        }
    }

    private void LateUpdate()
    {
        if(agent != null && agent.hasPath)
        {
            Vector3 nextPosition = agent.nextPosition;

            Vector3 direction = (nextPosition - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = rotation;
            }
        }
    }

    public void SetTarget(GameObject targetObject)
    {
        target = targetObject.transform;
        Debug.Log($"Target set to: {targetObject.name}", targetObject);
    }
}
