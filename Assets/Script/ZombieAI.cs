
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
        if (target != null)
        {
            agent.destination = target.position;
        }
    }


}
