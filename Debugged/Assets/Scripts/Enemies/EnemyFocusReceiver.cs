using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFocusReceiver : MonoBehaviour
{
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // If PriorityQueue has set a focus target, chase it
        if (PriorityQueue.FocusTarget != null)
        {
            agent.SetDestination(PriorityQueue.FocusTarget.position);
        }
        else
        {
            // TODO: your normal enemy AI behaviour here (patrol, chase nearest player, etc.)
        }
    }
}
