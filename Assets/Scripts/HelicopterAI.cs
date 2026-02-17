using UnityEngine;
using UnityEngine.AI;

public class HelicopterAI : MonoBehaviour
{

    public Transform playerTarget;
    private bool playerInRange;
    public float fireRange = 50f;
    public NavMeshAgent agent;
    private NavMeshPath path;
    private float elapsed = 0f; // Timer for pathfinding updates


    void Start()
    {
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
        }
        agent = GetComponent<NavMeshAgent>();
        agent.areaMask = 1 << NavMesh.GetAreaFromName("Flyable");
        path = new NavMeshPath();
        agent.stoppingDistance = fireRange;
    }
    
    // Update is called once per frame
    void Update()
    {
        playerInRange = Physics.CheckSphere(transform.position, fireRange, LayerMask.GetMask("player"));
        if (playerInRange)
        {
            CirclePlayer();
            Shootplayer();
        }
        else
        {
            FindPlayer();
        }
        
    }

    void FindPlayer()
    {

        elapsed += Time.deltaTime;
            if (elapsed > 1.0f)
            {
                elapsed -= 1.0f;
                if (!NavMesh.SamplePosition(agent.transform.position, out NavMeshHit startHit, 2f, agent.areaMask))
                {
                    Debug.LogWarning("EnemyAI: agent is not on the NavMesh.", this);
                    return;
                }

                if (!NavMesh.SamplePosition(playerTarget.position, out NavMeshHit endHit, 20f, agent.areaMask))
                {
                    Debug.LogWarning("EnemyAI: target is not on the NavMesh.", this);
                    return;
                }
                NavMesh.CalculatePath(
                    startHit.position,
                    endHit.position,
                    new NavMeshQueryFilter
                    {
                        agentTypeID = agent.agentTypeID,
                        areaMask = agent.areaMask
                    },
                    path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetPath(path);
                }
                else
                {
                    Debug.LogWarning($"EnemyAI: path status {path.status}", this);
                }
            }
    }

    void CirclePlayer()
    {
            
        
    }

    void Shootplayer()
    {

    }
}
