using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] bool m_playerWins = false;
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
        Debug.Log("Funcion1");
        yield return null;
    }

    IEnumerator MainPartOfGame()
    {
        Debug.Log("Funcion2");

        while (!m_playerWins)
        {
            yield return null;
        }

    }

    IEnumerator EndOfGame()
    {
        Debug.Log("Funcion3");
        yield return null;
    }
}
