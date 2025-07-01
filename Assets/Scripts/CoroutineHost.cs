using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHost : MonoBehaviour
{   
    public static CoroutineHost Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = (CoroutineHost)FindObjectOfType(typeof(CoroutineHost));
                if (m_Instance == null)
                {
                    GameObject go = new GameObject();
                    m_Instance = go.AddComponent<CoroutineHost>();
                }

                DontDestroyOnLoad(m_Instance.gameObject);
            }

            return m_Instance;
        }
    }

    private static CoroutineHost m_Instance = null;
}
