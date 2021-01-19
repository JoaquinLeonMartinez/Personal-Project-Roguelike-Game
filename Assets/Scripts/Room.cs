using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P};
public enum Door { Up, Down, Right, Left};
public class Room : MonoBehaviour
{
    public RoomType type;
    public List<Door> doors = new List<Door>();

    //Compatible rooms
    //public GameObject roomPrefab;
    //public int openedDoors;
    //TODO: Hacer un par de salas de ejemplo para aclararme

    void Start()
    {
        //En el start, segun el tipo podemos generar un array con las puertas que tiene directamente (de esta forma no habira que meterlo a mano cada vez que hacemos una sala)
        //Debug.Log($"Se acaba de crear una room de tipo {type}, con {doors.Count} puertas");
    }

    void Update()
    {
        
    }

    //Indica si una room tiene una puerta en un lugar determinado
    public bool DoorExisits(Door door)
    {
        foreach (var item in doors)
        {
            if (item == door)
            {
                return true;
            }
        }

        return false;
    }

    //Indica si la room se podría conectar a la puerta indicada, es decir, devuelve true cuando tiene la puerta contraria
    public bool IsCompatible(Door door)
    {
        Door deshiredDoor = GetComplementaryDoor(door);

        foreach (var item in doors)
        {
            if (item == deshiredDoor)
            {
                //Debug.Log($"La puerta {door} es compatible con la puerta {item}");
                return true;
            }
            else
            {
                //Debug.Log($"La puerta {door} NO es compatible con la puerta {item}");
            }
        }
        return false;
    }

    public static Door GetComplementaryDoor(Door door)
    {
        switch (door)
        {
            case Door.Right:
                return Door.Left;
            case Door.Down:
                return Door.Up;
            case Door.Left:
                return Door.Right;
            case Door.Up:
                return Door.Down;
        }

        return Door.Down; //nunca deberia llegar a este punto
    }

    public void ClearDoors()
    {
        doors.Clear();
    }

    public void DisableDoor(Door door)
    {
        bool enc = false;
        int i = 0;
        for (i = 0; i < doors.Count && !enc; i++)
        {
            if (doors[i] == door)
            {
                enc = true;
                break;
            }
        }

        if (enc)
        {
            doors.RemoveAt(i);
        }
        
    }

    public void SetDoors()
    {
        switch (type)
        {
            case RoomType.A:
                doors.Add(Door.Right);
                doors.Add(Door.Down);
                doors.Add(Door.Left);
                doors.Add(Door.Up);
                break;
            case RoomType.B:
                doors.Add(Door.Right);
                doors.Add(Door.Down);
                doors.Add(Door.Up);
                break;
            case RoomType.C:
                doors.Add(Door.Right);
                doors.Add(Door.Down);
                doors.Add(Door.Left);
                break;
            case RoomType.D:
                doors.Add(Door.Down);
                doors.Add(Door.Left);
                doors.Add(Door.Up);
                break;
            case RoomType.E:
                doors.Add(Door.Right);
                doors.Add(Door.Left);
                doors.Add(Door.Up);
                break;
            case RoomType.F:
                doors.Add(Door.Right);
                doors.Add(Door.Down);
                break;
            case RoomType.G:
                doors.Add(Door.Down);
                doors.Add(Door.Left);
                break;
            case RoomType.H:
                doors.Add(Door.Left);
                doors.Add(Door.Up);
                break;
            case RoomType.I:
                doors.Add(Door.Right);
                doors.Add(Door.Up);
                break;
            case RoomType.J:
                doors.Add(Door.Up);
                break;
            case RoomType.K:
                doors.Add(Door.Right);
                break;
            case RoomType.L:
                doors.Add(Door.Down);
                break;
            case RoomType.M:
                doors.Add(Door.Left);
                break;
            case RoomType.N:
                doors.Add(Door.Right);
                doors.Add(Door.Left);
                break;
            case RoomType.O:
                doors.Add(Door.Down);
                doors.Add(Door.Up);
                break;
            case RoomType.P:
                break;
        }
    }
}
