using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Settings")]
    public GameObject zombiePrefab;            // Prefab del zombie con NavMesh, AI, Ragdoll
    public int cantidadZombies = 5;
    public float radioSpawn = 20f;
    public Transform jugador;

    [Header("Tiempo entre oleadas")]
    public float tiempoEntreSpawns = 5f;

    private float tiempoProximoSpawn;

    void Start()
    {
        tiempoProximoSpawn = Time.time + 1f;
    }

    void Update()
    {
        if (Time.time >= tiempoProximoSpawn)
        {
            SpawnZombies();
            tiempoProximoSpawn = Time.time + tiempoEntreSpawns;
        }
    }

    void SpawnZombies()
    {
        for (int i = 0; i < cantidadZombies; i++)
        {
            Vector3 posicionAleatoria = transform.position + Random.insideUnitSphere * radioSpawn;
            posicionAleatoria.y = transform.position.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(posicionAleatoria, out hit, 2.0f, NavMesh.AllAreas))
            {
                GameObject zombie = Instantiate(zombiePrefab, hit.position, Quaternion.identity);

                // Asignar objetivo (jugador)
                ZombieAI ai = zombie.GetComponent<ZombieAI>();
                if (ai != null)
                {
                    ai.target = jugador;
                }
            }
        }
    }
}
