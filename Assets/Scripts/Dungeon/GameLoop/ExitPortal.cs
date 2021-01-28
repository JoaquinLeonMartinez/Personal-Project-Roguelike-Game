using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    bool enable;
    void Start()
    {
        enable = true;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Ha entrado en el portal algo con la etiqueta {other.tag}");
        if (other.tag == "Player")
        {
            //Change dungeon
            enable = false;
            StartCoroutine(GameManager.Instance.OnChangeStage());
        }
    }
}
