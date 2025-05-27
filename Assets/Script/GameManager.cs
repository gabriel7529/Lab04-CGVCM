using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] bool m_playerWins = false;
    public GameObject[] exits;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MasterLoop());
    }

    IEnumerator MasterLoop()
    {
        yield return StartCoroutine(BeginingOfGame());
        yield return StartCoroutine(MainPartOfGame());
        yield return StartCoroutine(EndOfGame());
    }

    IEnumerator BeginingOfGame()
    {
        Debug.Log("Inicio del Juego ");
        foreach (GameObject exit in exits)
        {
            if (exit != null)
                exit.SetActive(false);
        }

        // Activar uno aleatorio
        int randomIndex = Random.Range(0, exits.Length);
        if (exits[randomIndex] != null)
            exits[randomIndex].SetActive(true);

        yield return null;
    }

    IEnumerator MainPartOfGame()
    {
        Debug.Log("Busca la zona de extracción");

        while (!m_playerWins)
        {
            yield return null;
        }

    }

    IEnumerator EndOfGame()
    {
        //TODO: Ir al siguiente escenario
        yield return null;
    }

    public void PlayerReacheExit()
    {
        m_playerWins = true;
    }
}
