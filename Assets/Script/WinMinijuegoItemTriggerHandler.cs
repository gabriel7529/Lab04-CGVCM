using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMinijuegoItemTriggerHandler : MonoBehaviour
{
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private bool already = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!already && other.CompareTag("Player"))
        {
            already = true;
            transform.parent.transform.localScale = Vector3.zero;
            //* Destroy(transform.parent.gameObject);
            StartCoroutine(GoNextScene());
        }
    }

    IEnumerator GoNextScene()
    {
        audioManager.PlaySFX(audioManager.winMinijuegoSound);
        yield return new WaitForSeconds(6f);

        SceneManager.LoadScene("SampleScene");
    }
}
