using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    void Start()
    {
        
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
            GameManager.Instance.OnChangeStage();
        }
    }
}
