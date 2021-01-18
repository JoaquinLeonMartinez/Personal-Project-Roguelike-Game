using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { A, B, C, D, E, F, G, H, I, J, K, L, M};
public enum Door { Up, Down, Right, Left};
public class Room : MonoBehaviour
{
    public RoomType type;
    //Compatible rooms
    public GameObject roomPrefab;
    public int openedDoors;
    //TODO: Hacer un par de salas de ejemplo para aclararme

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
