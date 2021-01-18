using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverseIllo : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        this.GetComponent<CharacterController>().Move(new Vector3(0,1 * Time.deltaTime,0));
    }
}
