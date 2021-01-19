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
    public static float distanceP = 3.25f;

    // Array de salas predefinidas
    public List<GameObject> predefinedRooms = new List<GameObject>();

    // Nº de salas
    public int roomsToGenerate = 2;


    void Start()
    {
        SetPredefinedRooms();

        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        GenerateDungeonLineal();
    }

    public void GenerateDungeonLineal()
    {
        //La primera sala no puede ser del tipo P, que es el ultimo en el array, de modo que generamos un random hasta el .count - 1
        //La generamos en el (0,0,0)
        GameObject lastRoom = null;
        var currentRoom = predefinedRooms[Random.Range(0, predefinedRooms.Count - 1)];
        currentRoom = Instantiate(currentRoom, new Vector3(0, 0, 0), currentRoom.transform.rotation);
        //currentRoom.GetComponent<Room>().SetDoors();
        roomsToGenerate--;
        Debug.Log($"Acabamos de generar la primera room (tipo {currentRoom.GetComponent<Room>().type.ToString()})");
        bool isClose = false;
        //Ahora generamos el resto de rooms
        while (!isClose)
        {
            Debug.Log($"Quedan {roomsToGenerate} por generar");
            // Antes de nada necesitamos saber que salas son compatibles con cada puerta
            //Debug.Log($"La room actual tiene {currentRoom.GetComponent<Room>().doors.Count} puertas");
            bool firstRoomGenerated = false; //primera room conectada a otra

            if (roomsToGenerate == 0)//esto indica que este sera el ultimo loop y que solo generaremos p
            {
                Debug.Log($"Ya no se generara nada que no sea de tipo P");
                isClose = true; //para cuando acabe el loop estara cerrado el circuito
                firstRoomGenerated = true;
            }

            for (int j = 0; j < currentRoom.GetComponent<Room>().doors.Count; j++)
            {
                Debug.Log($"Puertas posibles = {currentRoom.GetComponent<Room>().doors.Count}");
                if (firstRoomGenerated)
                {
                    //Las tipo P no cuentan como room, realmente no puedes entrar
                    GameObject roomToInstantiate = predefinedRooms[predefinedRooms.Count - 1]; //Si no quedan rooms completamos las aperturas con rooms de tipo P
                    Instantiate(roomToInstantiate, GeneratePosition(currentRoom.transform, currentRoom.GetComponent<Room>().doors[j], distanceP), roomToInstantiate.transform.rotation);
                }
                else
                {
                    //Debug.Log($"Vamos a buscar rooms compatibles con la puerta {currentRoom.GetComponent<Room>().doors[j].ToString()}");
                    List<GameObject> compatibleRooms = GetCompatibleRooms(currentRoom.GetComponent<Room>().doors[j]);
                    //Debug.Log($"Hemos obtenido {compatibleRooms.Count} rooms compatibles.");
                    if (compatibleRooms.Count != 0) //esto se va a dar siempre realmente
                    {
                        //Seleccionamos una de ellas de forma aleatoria
                        GameObject roomToInstantiate = compatibleRooms[Random.Range(0, compatibleRooms.Count)];
                        lastRoom = Instantiate(roomToInstantiate, GeneratePosition(currentRoom.transform, currentRoom.GetComponent<Room>().doors[j], distance), roomToInstantiate.transform.rotation);
                        lastRoom.GetComponent<Room>().DisableDoor(Room.GetComplementaryDoor(currentRoom.GetComponent<Room>().doors[j]));
                        Debug.Log($"Hemos instanciado una room de tipo {roomToInstantiate.GetComponent<Room>().type.ToString()}");//, con una rotacion de {roomToInstantiate.transform.rotation.x}, {roomToInstantiate.transform.rotation.y}, {roomToInstantiate.transform.rotation.z}");
                        //Debug.Log($"Desactivamos la puerta {Room.GetComplementaryDoor(currentRoom.GetComponent<Room>().doors[j])} de la nueva room");
                        firstRoomGenerated = true;
                        roomsToGenerate--;
                    }
                    else
                    {
                        Debug.Log($"La puerta {currentRoom.GetComponent<Room>().doors[j]} no es compatible con nada");
                    }
                }
                
            }
            //Cuando acabamos con las puertas de una room, vamos con las de la siguiente
            currentRoom = lastRoom;
            Debug.Log($"Next room = {currentRoom.GetComponent<Room>().type}");
        }

        Debug.Log($"Sale del bucle con isClose = {isClose} y roomsToGenerate = {roomsToGenerate}");
    }

    public void GenerateDungeonBase()
    {
        //La primera sala no puede ser del tipo P, que es el ultimo en el array, de modo que generamos un random hasta el .count - 1
        //La generamos en el (0,0,0)
        var currentRoom = predefinedRooms[Random.Range(0, predefinedRooms.Count - 1)];
        currentRoom = Instantiate(currentRoom, new Vector3(0, 0, 0), currentRoom.transform.rotation);
        //currentRoom.GetComponent<Room>().SetDoors();
        roomsToGenerate--;
        Debug.Log($"Acabamos de generar la primera room (tipo {currentRoom.GetComponent<Room>().type.ToString()})");

        //Ahora generamos el resto de rooms
        while(roomsToGenerate > 0)
        {
            Debug.Log($"Quedan {roomsToGenerate} por generar");
            // Antes de nada necesitamos saber que salas son compatibles con cada puerta
            Debug.Log($"La room actual tiene {currentRoom.GetComponent<Room>().doors.Count} puertas");

            for (int j = 0; j < currentRoom.GetComponent<Room>().doors.Count; j++)
            {
                if (roomsToGenerate == 0)
                {
                    GameObject roomToInstantiate = predefinedRooms[predefinedRooms.Count - 1]; //Si no quedan rooms completamos las aperturas con rooms de tipo P
                    Instantiate(roomToInstantiate, GeneratePosition(currentRoom.transform, currentRoom.GetComponent<Room>().doors[j], distanceP), roomToInstantiate.transform.rotation);
                }
                else
                {
                    Debug.Log($"Vamos a buscar rooms compatibles con la puerta {currentRoom.GetComponent<Room>().doors[j].ToString()}");
                    List<GameObject> compatibleRooms = GetCompatibleRooms(currentRoom.GetComponent<Room>().doors[j]);
                    //Debug.Log($"Hemos obtenido {compatibleRooms.Count} rooms compatibles.");
                    if (compatibleRooms.Count != 0) //esto se va a dar siempre realmente
                    {
                        //Seleccionamos una de ellas de forma aleatoria
                        GameObject roomToInstantiate = compatibleRooms[Random.Range(0, compatibleRooms.Count)];
                        Instantiate(roomToInstantiate, GeneratePosition(currentRoom.transform, currentRoom.GetComponent<Room>().doors[j], distance), roomToInstantiate.transform.rotation);
                        Debug.Log($"Hemos instanciado una room de tipo {roomToInstantiate.GetComponent<Room>().type.ToString()}");//, con una rotacion de {roomToInstantiate.transform.rotation.x}, {roomToInstantiate.transform.rotation.y}, {roomToInstantiate.transform.rotation.z}");
                    }
                }
                roomsToGenerate--;
            }
        }
    }

    //Devolverá una lista de rooms compatibles con una puerta de la room ya creada
    public List<GameObject> GetCompatibleRooms(Door door)
    {
        List<GameObject> compatibleRooms = new List<GameObject>();

        for (int i = 0; i < predefinedRooms.Count; i++)
        {
            
            if (predefinedRooms[i].GetComponent<Room>().IsCompatible(door))
            {
                //Debug.Log($"La room {predefinedRooms[i].GetComponent<Room>().type} es compatible");
                compatibleRooms.Add(predefinedRooms[i]);
            }
        }

        return compatibleRooms;
    }

    public void SetPredefinedRooms()
    {
        foreach (var item in predefinedRooms)
        {
            item.GetComponent<Room>().ClearDoors();
            item.GetComponent<Room>().SetDoors();
        }
    }

    public Vector3 GeneratePosition(Transform parentTransform, Door door, float distance)
    {
        Vector3 position = Vector3.zero;
        switch (door)
        {
            case Door.Right:
                return new Vector3(parentTransform.position.x, parentTransform.position.y, parentTransform.position.z + distance);
            case Door.Down:
                return new Vector3(parentTransform.position.x + distance, parentTransform.position.y, parentTransform.position.z);
            case Door.Left:
                return new Vector3(parentTransform.position.x, parentTransform.position.y, parentTransform.position.z - distance);
            case Door.Up:
                return new Vector3(parentTransform.position.x - distance, parentTransform.position.y, parentTransform.position.z);
        }


        return position;
    }
}
