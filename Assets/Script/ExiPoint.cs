using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExiPoint : MonoBehaviour
{

    [SerializeField]GameManager m_gameManager;
    bool m_wasEnable = false;
    void enable()
    {
       
        m_wasEnable=true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if(!m_wasEnable) enable();

        if(other.TryGetComponent<Movement>(out Movement movement))
        {
            m_gameManager.PlayerReacheExit();
        }
    }
}
