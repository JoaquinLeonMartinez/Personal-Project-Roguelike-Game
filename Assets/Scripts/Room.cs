using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P};
public enum Door { Up, Down, Right, Left};
public class Room : MonoBehaviour
{
    public RoomType type;
    //Compatible rooms
    //public GameObject roomPrefab;
    //public int openedDoors;
    //TODO: Hacer un par de salas de ejemplo para aclararme

    void Start()
    {
        //En el start, segun el tipo podemos generar un array con las puertas que tiene directamente (de esta forma no habira que meterlo a mano cada vez que hacemos una sala)
    }

    void Update()
    {
        
    }
}
