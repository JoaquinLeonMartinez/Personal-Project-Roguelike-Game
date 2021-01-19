using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    //Singleton
    private static DungeonManager instance = null;
    public static DungeonManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    //Distancia entre centros de salas
    public static float distance = 4.5f;

    // Array de salas predefinidas
    public List<GameObject> predefinedRooms = new List<GameObject>();

    List<Door> posibleDoors = new List<Door>();
    
    void Start()
    {
        posibleDoors.Add(Door.Right);
        posibleDoors.Add(Door.Down);
        posibleDoors.Add(Door.Left);
        posibleDoors.Add(Door.Up);

        GenerateDungeon();
    }

    void Update()
    {
        
    }

    public void GenerateDungeon()
    {
        // Necesitamos:
        // Nº de salas
        int roomsToGenerate = 3; //TODO: Esto debe leerse del menu


        //La primera sala no puede ser del tipo P, que es el ultimo en el array, de modo que generamos un random hasta el .count - 1
        //La generamos en el (0,0,0)
        var currentRoom = predefinedRooms[Random.Range(0, predefinedRooms.Count - 1)];
        Instantiate(currentRoom, new Vector3(0, 0, 0), currentRoom.transform.rotation);
        roomsToGenerate--;

        //Ahora generamos el resto de rooms
        for (int i = 0; i < roomsToGenerate; i++)
        {
            // Antes de nada necesitamos saber que salas son compatibles con cada puerta
            //Si nuestra room tiene puerta derecha
            for (int j = 0; j < posibleDoors.Count; j++)
            {
                if (DoorExists(currentRoom.GetComponent<Room>().type, posibleDoors[j]))
                {
                    //TODO
                    //En caso de llegar aqui buscamos una habitación compatible
                    //Comprobar tambien cuantas quedan por generar
                }
            }

            //currentRoom = predefinedRooms[Random.Range(0, predefinedRooms.Count)];
            //Instantiate(currentRoom, new Vector3(0, 0, 4.5f * i), currentRoom.transform.rotation);
        }
    }

    //Devolverá una room compatible con una puerta de la room ya creada
    public GameObject GetCompatibleRoom(Door door)
    {
        //TODO
        return predefinedRooms[0];
    }

    //Indica si una room tiene una puerta en un lugar determinado
    public bool DoorExists(RoomType roomType, Door door)
    {
        if (door == Door.Right && (roomType == RoomType.A || roomType == RoomType.B || roomType == RoomType.C || roomType == RoomType.G || roomType == RoomType.F || roomType == RoomType.I || roomType == RoomType.K || roomType == RoomType.N))
        {
            return true;
        }

        if (door == Door.Down && (roomType == RoomType.A || roomType == RoomType.B || roomType == RoomType.C || roomType == RoomType.D || roomType == RoomType.F || roomType == RoomType.G || roomType == RoomType.L || roomType == RoomType.O))
        {
            return true;
        }

        if (door == Door.Left && (roomType == RoomType.A || roomType == RoomType.C || roomType == RoomType.D || roomType == RoomType.E || roomType == RoomType.G || roomType == RoomType.H || roomType == RoomType.M || roomType == RoomType.N))
        {
            return true;
        }

        if (door == Door.Up && (roomType == RoomType.A || roomType == RoomType.B || roomType == RoomType.D || roomType == RoomType.E || roomType == RoomType.H || roomType == RoomType.I || roomType == RoomType.J || roomType == RoomType.O))
        {
            return true;
        }

        return false;
    }
}
