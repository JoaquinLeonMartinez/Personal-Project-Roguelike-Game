using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public TMP_InputField roomsToGenerateInput;
    public GameObject dungeonContainer;
    //La utilizaremos para ir guardando las roomsque generemos que no sean de tipo P
    public List<GameObject> generatedRooms = new List<GameObject>();

    int checkedRooms = 0;


    //New method:
    int ramificaciones = 1;


    //backtraking
    public List<RoomTypeBacktracking> grid;
    public int sizeX = 10;
    public int sizeY = 10;

    void Start()
    {
        SetPredefinedRooms();

        //GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        GenerateDungeonLinealImproved();
    }

    public void GenerateDungeonLinealImproved()
    {
        //generar todo menos las P
        //para determinar si hay un bloqueo debe encontrar obstaculos en todas sus puertas
        //si encuentra un bloqueo vuelve a empezar desde otra

        var currentRoom = predefinedRooms[Random.Range(0, predefinedRooms.Count - 1)];
        currentRoom = Instantiate(currentRoom, new Vector3(0, 0, 0), currentRoom.transform.rotation);
        currentRoom.transform.parent = dungeonContainer.transform;
        roomsToGenerate--;
        bool isClose = false;
        //Ahora generamos el resto de rooms
        while (!isClose)
        {

            for (int j = 0; j < currentRoom.GetComponent<Room>().doors.Count; j++)
            {
                if (!isBlockedSimplify(currentRoom, currentRoom.GetComponent<Room>().doors[j]))//si la puerta no esta bloqueada
                {
                    List<GameObject> compatibleRooms = GetCompatibleRooms(currentRoom.GetComponent<Room>().doors[j]);
                    GenerateRoomNormal(currentRoom, compatibleRooms[Random.Range(0, compatibleRooms.Count)], GeneratePosition(currentRoom.transform, currentRoom.GetComponent<Room>().doors[j], distance), j);
                    //j--; 
                }
                else
                {

                }
            }
        }

        if (roomsToGenerate <= 0)
        {
            isClose = true;
        }
        else if(currentRoom == generatedRooms[generatedRooms.Count - 1])
        {
            //bloqueo
            isClose = true;
        }
        else
        {
            currentRoom = generatedRooms[generatedRooms.Count - 1];
        }

    }


    public void InitGridTypes()
    {
        grid.Clear();
        for (int i = 0; i < (sizeX*sizeY); i++)
        {
            grid.Add(RoomTypeBacktracking.X);
        }
    }

    //TODO: probar la clase grid
    //(numeroDeCelda/tamañoY , numeroDeZelda%tamañoY)
    //la lista grid tiene que venir llena de X
    public bool GenerateDungeonBacktracking(List<RoomTypeBacktracking> grid, int position)
    {
        List<RoomTypeBacktracking> arrayPosiblesOpciones = GetCompatibles(grid, position);
        bool isOk = false;
        for (int i = 0; i < arrayPosiblesOpciones.Count && !isOk; i++)
        {
            grid[position] = arrayPosiblesOpciones[i];
            isOk = GenerateDungeonBacktracking(grid, position);//esto no es asi realmente, habria que incrementarlo bien
        }

        if (isOk)
        {
            Debug.Log($"La casilla ({position / sizeY},{position % sizeY}) se ha generado con una {grid[position]}");
            //Extra: generar aqui todas las P
            return true;
        }
        else
        {
            return false;
        }

    }

    public List<RoomTypeBacktracking> GetCompatibles(List<RoomTypeBacktracking> grid, int position)
    {
        List<RoomTypeBacktracking> compatibles = new List<RoomTypeBacktracking>();
        //si es la primera genera una si o si
        if (position == 0)
        {
            //Genera una si o si
        }

        //Cosas a tener en cuenta a la hora de chequear:

        //no hace falta llamar a todas, segun el tipo de room se llama a unas u a otras
        //Chequear arriba
        //Chequear derecha
        //Chequear abajo
        //Chequear izquierda
        //CheckDirection(List<RoomTypeBacktracking> grid, int currentPos, Door direction)

        return compatibles;
    }

    public void CheckDirection(List<RoomTypeBacktracking> grid, int currentPos, Door direction)
    {
        //Las opciones son 3: que arriba no haya nada (X), que arriba haya algo que bloquee (final del array o room sin puerta), que haya una room con puerta)
        DirectionState state = DirectionState.block;

        switch (direction)
        {
            case Door.Up:
                //Comprobamos la casilla de arriba, viniendo de abajo
                if ((currentPos - sizeY) > 0) 
                {
                    state = DirectionState.block; //no existe la posicion
                }
                else if( grid[currentPos - sizeY] == RoomTypeBacktracking.X)
                {
                    state = DirectionState.empty; //se podría utilizar
                }
                else //if(Room with open door)
                {
                    //indicar que la puerta de arriba tiene una conexion
                    state = DirectionState.open;
                }//else room with close door -- state = DirectionState.block;

                break;
            case Door.Right:
                break;
            case Door.Down:
                break;
            case Door.Left:
                break;
        }
    }


    public void GenerateDungeonLinealImprovedFail() //not working
    {
        //La primera sala no puede ser del tipo P, que es el ultimo en el array, de modo que generamos un random hasta el .count - 1
        //La generamos en el (0,0,0)
        GameObject lastRoom = null;
        var currentRoom = predefinedRooms[Random.Range(0, predefinedRooms.Count - 1)];
        currentRoom = Instantiate(currentRoom, new Vector3(0, 0, 0), currentRoom.transform.rotation);
        currentRoom.transform.parent = dungeonContainer.transform;
        //currentRoom.GetComponent<Room>().SetDoors();
        roomsToGenerate--;
        Debug.Log($"Acabamos de generar la primera room (tipo {currentRoom.GetComponent<Room>().type.ToString()})");
        bool isClose = false;
        //Ahora generamos el resto de rooms
        while (!isClose)
        {
            //Debug.Log($"Quedan {roomsToGenerate} por generar");
            // Antes de nada necesitamos saber que salas son compatibles con cada puerta
            //Debug.Log($"La room actual tiene {currentRoom.GetComponent<Room>().doors.Count} puertas");
            bool firstRoomGenerated = false; //primera room conectada a otra

            if (roomsToGenerate == 0)//esto indica que este sera el ultimo loop y que solo generaremos p
            {
                //Debug.Log($"Ya no se generara nada que no sea de tipo P");
                isClose = true; //para cuando acabe el loop estara cerrado el circuito
                firstRoomGenerated = true;
            }

            bool finished = false;

            for (int j = 0; j < currentRoom.GetComponent<Room>().doors.Count && !finished; j++)
            {
                //Debug.Log($"Puertas posibles = {currentRoom.GetComponent<Room>().doors.Count}");
                bool isWayBlocked = isBlocked(currentRoom, currentRoom.GetComponent<Room>().doors[j]);
                if (firstRoomGenerated || isWayBlocked)
                {
                    //Las tipo P no cuentan como room, realmente no puedes entrar
                    if (isWayBlocked)
                    {
                        //En caso de estar bloqueado aqui acaba todo, destruimos la current y la sustituimos por una room P 
                        GenerateEndRoom(currentRoom);
                        finished = true;
                        isClose = true;
                    }
                    else //si el camino no esta bloqueado instanciamos una room P y listo
                    {
                        GenerateRoomP(currentRoom, predefinedRooms[predefinedRooms.Count - 1], GeneratePosition(currentRoom.transform, currentRoom.GetComponent<Room>().doors[j], distanceP), j);
                    }
                }
                else
                {
                    //Debug.Log($"Vamos a buscar rooms compatibles con la puerta {currentRoom.GetComponent<Room>().doors[j].ToString()}");
                    List<GameObject> compatibleRooms = GetCompatibleRooms(currentRoom.GetComponent<Room>().doors[j]);
                    //Debug.Log($"Hemos obtenido {compatibleRooms.Count} rooms compatibles.");
                    if (compatibleRooms.Count != 0) //esto se va a dar siempre realmente
                    {
                        //Generamos la room
                        lastRoom = GenerateRoomNormal(currentRoom, compatibleRooms[Random.Range(0, compatibleRooms.Count)], GeneratePosition(currentRoom.transform, currentRoom.GetComponent<Room>().doors[j], distance), j);
                        //Debug.Log($"Desactivamos la puerta {Room.GetComplementaryDoor(currentRoom.GetComponent<Room>().doors[j])} de la nueva room");
                        firstRoomGenerated = true;
                        roomsToGenerate--;
                        j--; //como borramos una puerta hay que decrementar este valor
                    }
                    else
                    {
                        Debug.Log($"La puerta {currentRoom.GetComponent<Room>().doors[j]} no es compatible con nada");
                    }
                }

            }


            if (currentRoom != lastRoom && !isClose) //comprobamos que la ultima generada no sea la actual, eso significaria que no se ha hecho ninguna conexion (caso del else)
            {
                //Cuando acabamos con las puertas de una room, vamos con las de la siguiente
                currentRoom = lastRoom;
                //Debug.Log($"Next room = {currentRoom.GetComponent<Room>().type}");
            }
            else if (roomsToGenerate != 0)
            {
                //Si llegamos aqui significa que nos hemos bloqueado, no se puede seguir generando la mazmorra
                //Debug.Log($"Hemos llegado a un bloqueo, quedan {roomsToGenerate} rooms por generar");

                //En este caso buscaremos otra room compatible
                
                currentRoom = FindOtherPath();

                if (checkedRooms >= generatedRooms.Count || currentRoom == null)// si no quedan compatibles pues nos aguantamos
                {
                    Debug.Log($"Hemos llegado a un bloqueo critico, checked rooms = {checkedRooms} and generated rooms = {generatedRooms.Count}");
                    isClose = true;
                }
                else
                {
                    isClose = false;
                }
            }
        }

        Debug.Log($"Acaba con roomsToGenerate = {roomsToGenerate}");
    }

    public GameObject FindOtherPath()
    {
        GameObject roomToGenerate = null;
        bool enc = false;

        for (int i = checkedRooms; i < generatedRooms.Count && !enc; i++)
        {
            //si tiene alguna room P hijo la borra y devuelve true
            if (generatedRooms[i].GetComponent<Room>().DeleteRoomsP())
            {
                roomToGenerate = generatedRooms[i];
                Debug.LogError($"Hemos encontrado una nueva room por la que empezar de tipo {generatedRooms[i].GetComponent<Room>().type}");
                enc = true;
            }
            checkedRooms = i; //para no volver a buscar en la misma
        }

        return roomToGenerate;
    }

    public GameObject GenerateRoomNormal(GameObject currentRoom, GameObject roomToInstantiate, Vector3 position, int doorPosition)
    {
        var generatedRoom = Instantiate(roomToInstantiate, position, roomToInstantiate.transform.rotation);
        generatedRoom.transform.parent = dungeonContainer.transform;
        generatedRoom.GetComponent<Room>().SetUp(currentRoom, Room.GetComplementaryDoor(currentRoom.GetComponent<Room>().doors[doorPosition])); // no pasa anda porque lo llamemos con P, no hara anda  y ya esta
        currentRoom.GetComponent<Room>().connections.Add(generatedRoom);
        generatedRooms.Add(generatedRoom);
        Debug.Log($"Hemos instanciado una room de tipo {roomToInstantiate.GetComponent<Room>().type.ToString()} con parent tipo {currentRoom.GetComponent<Room>().type} en la puerta {currentRoom.GetComponent<Room>().doors[doorPosition]} (la cual vamos a desactivar), y la añadimos al array de generated");

        currentRoom.GetComponent<Room>().DisableDoor(currentRoom.GetComponent<Room>().doors[doorPosition]);//desactivamos la puerta en la que hemos generado al hijo
        roomsToGenerate--;

        return generatedRoom;
    }

    public void GenerateRoomP(GameObject currentRoom, GameObject roomToInstantiate, Vector3 position, int doorPosition)
    {
        currentRoom.GetComponent<Room>().connections.Add(Instantiate(roomToInstantiate,position , roomToInstantiate.transform.rotation));
        currentRoom.GetComponent<Room>().connections[currentRoom.GetComponent<Room>().connections.Count - 1].transform.parent = dungeonContainer.transform;
        currentRoom.GetComponent<Room>().connections[currentRoom.GetComponent<Room>().connections.Count - 1].GetComponent<Room>().SetParent(currentRoom);
        Debug.Log($"Hemos instanciado una room de tipo {roomToInstantiate.GetComponent<Room>().type.ToString()} con parent tipo {currentRoom.GetComponent<Room>().type} en la puerta {currentRoom.GetComponent<Room>().doors[doorPosition]}");
    }

    public void GenerateEndRoom(GameObject currentRoom)
    {
        //Antes de borrar la current room, habra que borrar todos sus hijos :')
        //Limpiamos las conexiones de la actual
        currentRoom.GetComponent<Room>().ClearConnections();
        //En la posicion de la que vamos a destruir, generamos una torre
        GameObject roomToInstantiate = predefinedRooms[predefinedRooms.Count - 1];
        var endRoom = Instantiate(roomToInstantiate, GeneratePosition(currentRoom.transform, currentRoom.GetComponent<Room>().entryDoor, distance - distanceP), roomToInstantiate.transform.rotation);
        endRoom.transform.parent = dungeonContainer.transform;
        endRoom.GetComponent<Room>().SetParent(currentRoom.GetComponent<Room>().parent);//le decimos a la room quien es su padre
        currentRoom.GetComponent<Room>().parent.GetComponent<Room>().connections.Add(endRoom); //le decimos al padre que tiene un nuevo hijo
        Debug.Log($"Hemos borrado la room {currentRoom.GetComponent<Room>().type} y finalizamos");
        //Destruimos la actual
        Destroy(currentRoom);
        generatedRooms.RemoveAt(generatedRooms.Count -1); // lo quitamos de la lista
        roomsToGenerate++; //hemos destruido una que ya habiamos restado
        Debug.Log($"Rooms generadas (sin P) antes del bloqueo: {generatedRooms.Count}, quedan {roomsToGenerate} por generar");

    }


    public void GenerateDungeonLineal()
    {
        //La primera sala no puede ser del tipo P, que es el ultimo en el array, de modo que generamos un random hasta el .count - 1
        //La generamos en el (0,0,0)
        GameObject lastRoom = null;
        var currentRoom = predefinedRooms[Random.Range(0, predefinedRooms.Count - 1)];
        currentRoom = Instantiate(currentRoom, new Vector3(0, 0, 0), currentRoom.transform.rotation);
        currentRoom.transform.parent = dungeonContainer.transform;
        //currentRoom.GetComponent<Room>().SetDoors();
        roomsToGenerate--;
        Debug.Log($"Acabamos de generar la primera room (tipo {currentRoom.GetComponent<Room>().type.ToString()})");
        bool isClose = false;
        //Ahora generamos el resto de rooms
        while (!isClose)
        {
            //Debug.Log($"Quedan {roomsToGenerate} por generar");
            // Antes de nada necesitamos saber que salas son compatibles con cada puerta
            //Debug.Log($"La room actual tiene {currentRoom.GetComponent<Room>().doors.Count} puertas");
            bool firstRoomGenerated = false; //primera room conectada a otra

            if (roomsToGenerate == 0)//esto indica que este sera el ultimo loop y que solo generaremos p
            {
                //Debug.Log($"Ya no se generara nada que no sea de tipo P");
                isClose = true; //para cuando acabe el loop estara cerrado el circuito
                firstRoomGenerated = true;
            }

            bool finished = false;

            for (int j = 0; j < currentRoom.GetComponent<Room>().doors.Count && !finished; j++)
            {
                //Debug.Log($"Puertas posibles = {currentRoom.GetComponent<Room>().doors.Count}");
                bool isWayBlocked = isBlocked(currentRoom, currentRoom.GetComponent<Room>().doors[j]);
                if (firstRoomGenerated || isWayBlocked)
                {
                    //Las tipo P no cuentan como room, realmente no puedes entrar
                    if (isWayBlocked)
                    {
                        //En caso de estar bloqueado aqui acaba todo, destruimos la current y la sustituimos por una room P 
                        //Antes de borrar la current room, habra que borrar todos sus hijos :')
                        currentRoom.GetComponent<Room>().ClearConnections();
                        //En la posicion de la que vamos a destruir, generamos una torre
                        GameObject roomToInstantiate = predefinedRooms[predefinedRooms.Count - 1];
                        var endRoom = Instantiate(roomToInstantiate, GeneratePosition(currentRoom.transform, currentRoom.GetComponent<Room>().entryDoor, distance - distanceP), roomToInstantiate.transform.rotation);
                        endRoom.transform.parent = dungeonContainer.transform;
                        endRoom.GetComponent<Room>().SetParent(currentRoom.GetComponent<Room>().parent);//le decimos a la room quien es su padre
                        currentRoom.GetComponent<Room>().parent.GetComponent<Room>().connections.Add(endRoom); //le decimos al padre que tiene un nuevo hijo
                        Debug.Log($"Hemos borrado la room {currentRoom.GetComponent<Room>().type} y finalizamos");
                        //Destruimos la actual
                        Destroy(currentRoom);
                        //salimos de este bucle, pero no del otro
                        finished = true;
                        isClose = true;


                    }
                    else //si el camino no esta bloqueado instanciamos una y listo
                    {
                        GameObject roomToInstantiate = predefinedRooms[predefinedRooms.Count - 1]; //Si no quedan rooms completamos las aperturas con rooms de tipo P
                        currentRoom.GetComponent<Room>().connections.Add(Instantiate(roomToInstantiate, GeneratePosition(currentRoom.transform, currentRoom.GetComponent<Room>().doors[j], distanceP), roomToInstantiate.transform.rotation));
                        currentRoom.GetComponent<Room>().connections[currentRoom.GetComponent<Room>().connections.Count - 1].transform.parent = dungeonContainer.transform;
                        currentRoom.GetComponent<Room>().connections[currentRoom.GetComponent<Room>().connections.Count - 1].GetComponent<Room>().SetParent(currentRoom);
                        Debug.Log($"Hemos instanciado una room de tipo {roomToInstantiate.GetComponent<Room>().type.ToString()} con parent tipo {currentRoom.GetComponent<Room>().type} en la puerta {currentRoom.GetComponent<Room>().doors[j]}");
                    }
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
                        lastRoom.transform.parent = dungeonContainer.transform;
                        lastRoom.GetComponent<Room>().SetUp(currentRoom, Room.GetComplementaryDoor(currentRoom.GetComponent<Room>().doors[j]));
                        currentRoom.GetComponent<Room>().connections.Add(lastRoom);
                        Debug.Log($"Hemos instanciado una room de tipo {roomToInstantiate.GetComponent<Room>().type.ToString()} con parent tipo {currentRoom.GetComponent<Room>().type} en la puerta {currentRoom.GetComponent<Room>().doors[j]}");//, con una rotacion de {roomToInstantiate.transform.rotation.x}, {roomToInstantiate.transform.rotation.y}, {roomToInstantiate.transform.rotation.z}");
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
            
            if (currentRoom != lastRoom)
            {
                //Cuando acabamos con las puertas de una room, vamos con las de la siguiente
                currentRoom = lastRoom;
                //Debug.Log($"Next room = {currentRoom.GetComponent<Room>().type}");
            }
            else if(roomsToGenerate != 0)
            {
                //Si llegamos aqui significa que nos hemos bloqueado, no se puede seguir generando la mazmorra
                Debug.Log($"Hemos llegado a un bloqueo, quedan {roomsToGenerate} rooms por generar");
                isClose = true;
            } 
        }

        //Debug.Log($"Sale del bucle con isClose = {isClose} y roomsToGenerate = {roomsToGenerate}");
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

    public void OnClickGenerate()
    {
        if (!string.IsNullOrEmpty(roomsToGenerateInput.text))
        {
            roomsToGenerate = int.Parse(roomsToGenerateInput.text.ToString());
        }

        ResetDungeon();

        GenerateDungeon();
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
            item.GetComponent<Room>().ClearConnections();
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

    public bool isBlockedSimplify(GameObject currentRoom, Door door) //Devuelve true si choca con algo
    {
        Vector3 origin = Vector3.zero;
        Vector3 target1 = Vector3.zero;
        float maxDistance = 10f; ;
        float offsetY = 3f;
        float offset = 0f; //a cuanta distancia del centro se lanza el raycast
        float offsetTarget = distance / 2;

        switch (door)
        {
            case Door.Right:
                origin = new Vector3(currentRoom.transform.position.x, currentRoom.transform.position.y + offsetY, currentRoom.transform.position.z + offset);
                target1 = new Vector3(currentRoom.transform.position.x, currentRoom.transform.position.y, currentRoom.transform.position.z + distance * 2);
                break;
            case Door.Down:
                origin = new Vector3(currentRoom.transform.position.x + offset, currentRoom.transform.position.y + offsetY, currentRoom.transform.position.z);
                target1 = new Vector3(currentRoom.transform.position.x + distance * 2, currentRoom.transform.position.y, currentRoom.transform.position.z);
                break;
            case Door.Left:
                origin = new Vector3(currentRoom.transform.position.x, currentRoom.transform.position.y + offsetY, currentRoom.transform.position.z - offset);
                target1 = new Vector3(currentRoom.transform.position.x, currentRoom.transform.position.y, currentRoom.transform.position.z - distance * 2);
                break;
            case Door.Up:
                origin = new Vector3(currentRoom.transform.position.x - offset, currentRoom.transform.position.y + offsetY, currentRoom.transform.position.z);
                target1 = new Vector3(currentRoom.transform.position.x - distance * 2, currentRoom.transform.position.y, currentRoom.transform.position.z);
                break;
        }

        //Debug.Log($"Vamos a comprobar si esta bloqueada la puerta {door} de la room {currentRoom.GetComponent<Room>().type}");
        //En un futuro podria ser util si queremos ver con que elementos choca
        RaycastHit hit;
        if (Physics.Raycast(origin, (target1 - origin).normalized, out hit, maxDistance))
        {
            //Ha colisionado con algo
            Debug.Log($"(Raycast 1) La puerta {door} de la room {currentRoom.GetComponent<Room>().type} esta bloqueada con la room {hit.collider.GetComponent<Room>().type}");
            return true;
        }

        return false;
    }

    public bool isBlocked(GameObject currentRoom, Door door) //Devuelve true si choca con algo
    {
        Vector3 origin = Vector3.zero;
        Vector3 target1 = Vector3.zero; //hacemos tres raycasts
        Vector3 target2 = Vector3.zero;
        Vector3 target3 = Vector3.zero;
        float maxDistance = 10f; ;
        float offsetY = 3f;
        float offset = 0f; //a cuanta distancia del centro se lanza el raycast
        float offsetTarget = distance / 2;

        switch (door)
        {
            case Door.Right:
                origin = new Vector3(currentRoom.transform.position.x, currentRoom.transform.position.y + offsetY, currentRoom.transform.position.z + offset);
                target1 = new Vector3(currentRoom.transform.position.x, currentRoom.transform.position.y, currentRoom.transform.position.z + distance * 2);
                target2 = new Vector3(currentRoom.transform.position.x + offsetTarget, currentRoom.transform.position.y, currentRoom.transform.position.z + distance * 2);
                target3 = new Vector3(currentRoom.transform.position.x - offsetTarget, currentRoom.transform.position.y, currentRoom.transform.position.z + distance * 2);
                break;
            case Door.Down:
                origin = new Vector3(currentRoom.transform.position.x + offset, currentRoom.transform.position.y + offsetY, currentRoom.transform.position.z);
                target1 = new Vector3(currentRoom.transform.position.x + distance * 2, currentRoom.transform.position.y, currentRoom.transform.position.z);
                target2 = new Vector3(currentRoom.transform.position.x + distance * 2, currentRoom.transform.position.y, currentRoom.transform.position.z + offsetTarget);
                target3 = new Vector3(currentRoom.transform.position.x + distance * 2, currentRoom.transform.position.y, currentRoom.transform.position.z - offsetTarget);
                break;
            case Door.Left:
                origin = new Vector3(currentRoom.transform.position.x, currentRoom.transform.position.y + offsetY, currentRoom.transform.position.z - offset);
                target1 = new Vector3(currentRoom.transform.position.x, currentRoom.transform.position.y, currentRoom.transform.position.z - distance * 2);
                target2 = new Vector3(currentRoom.transform.position.x + offsetTarget, currentRoom.transform.position.y, currentRoom.transform.position.z - distance * 2);
                target3 = new Vector3(currentRoom.transform.position.x - offsetTarget, currentRoom.transform.position.y, currentRoom.transform.position.z - distance * 2);
                break;
            case Door.Up:
                origin = new Vector3(currentRoom.transform.position.x - offset, currentRoom.transform.position.y + offsetY, currentRoom.transform.position.z);
                target1 = new Vector3(currentRoom.transform.position.x - distance * 2, currentRoom.transform.position.y, currentRoom.transform.position.z);
                target2 = new Vector3(currentRoom.transform.position.x - distance * 2, currentRoom.transform.position.y, currentRoom.transform.position.z + offsetTarget);
                target3 = new Vector3(currentRoom.transform.position.x - distance * 2, currentRoom.transform.position.y, currentRoom.transform.position.z - offsetTarget);
                break;
        }

        //Debug.Log($"Vamos a comprobar si esta bloqueada la puerta {door} de la room {currentRoom.GetComponent<Room>().type}");
        //En un futuro podria ser util si queremos ver con que elementos choca
        RaycastHit hit;
        if (Physics.Raycast(origin, (target1 - origin).normalized, out hit, maxDistance))
        {
            //Ha colisionado con algo
            Debug.Log($"(Raycast 1) La puerta {door} de la room {currentRoom.GetComponent<Room>().type} esta bloqueada con la room {hit.collider.GetComponent<Room>().type}");
        }
        else if (Physics.Raycast(origin, (target2 - origin).normalized, out hit, maxDistance))
        {
            Debug.Log($"(Raycast 2) La puerta {door} de la room {currentRoom.GetComponent<Room>().type} esta bloqueada con la room {hit.collider.GetComponent<Room>().type}");
        }
        else if (Physics.Raycast(origin, (target3 - origin).normalized, out hit, maxDistance))
        {
            Debug.Log($"(Raycast 3) La puerta {door} de la room {currentRoom.GetComponent<Room>().type} esta bloqueada con la room {hit.collider.GetComponent<Room>().type}");
        }

        //TODO: Devolver la distancia del que choca con algo, si es menor de una torre pequeña no pasa nada
        return Physics.Raycast(origin, (target3 - origin).normalized, out hit, maxDistance) || Physics.Raycast(origin, (target2 - origin).normalized, out hit, maxDistance) || Physics.Raycast(origin, (target1 - origin).normalized, out hit, maxDistance);
    }

    public void ResetDungeon()
    {
        foreach (Transform child in dungeonContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        generatedRooms.Clear();
    }
}
