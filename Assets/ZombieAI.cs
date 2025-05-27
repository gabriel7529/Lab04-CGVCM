
using Unity.AI;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{

    public float speed;
    NavMeshAgent agent;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;


        
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = target.position;
    }
}
