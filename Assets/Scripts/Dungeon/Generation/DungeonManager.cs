using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonManager : MonoBehaviour
{
    #region Singleton
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

    #endregion

    #region Attributes
    //Distance between two rooms (center to center)
    public static float distance = 4.5f;

    //Distance between two rooms if one of them is the "end chamber (type P chamber)"
    public static float distanceP = 3.25f;

    //Predefined rooms array
    public List<GameObject> predefinedRooms = new List<GameObject>();

    //Prefab reference
    public GameObject exitPortalPrefab;

    //Nunmber of rooms to generate
    [System.NonSerialized]
    public int roomsToGenerate = 2;

    //GameObject parent
    public GameObject dungeonContainer;

    //Rooms container (except type P rooms)
    List<GameObject> generatedRooms = new List<GameObject>();

    //2D grid attributes
    public List<Room> grid;
    int sizeX = 4;
    int sizeY = 4;
    int entryPos = 0;
    public int dungeonSize = 0;

    [System.NonSerialized]
    public bool generated = false;

    #endregion

    #region Methods

   /*
    * This method will be invoked to generate the dungeon according to the given parameters.
    */
    public void GenerateDungeon(int roomsToGenerate)
    {
        GenerateDungeonFinalManager(roomsToGenerate);
    }

    /*
     * This method will be in charge of generating the dungeon in 2D and then transferring it to 3D.
     */
    public void GenerateDungeonFinalManager(int _roomsToGenerate)
    {
        roomsToGenerate = _roomsToGenerate;
        dungeonSize = roomsToGenerate;
        AdaptGrid();
        InitGrid();
        PrintGrid();
        Debug.Log($"Dungeon size = {roomsToGenerate}");

        //"Backtracking"
        if (!GenerateDungeonFinal(grid, roomsToGenerate))
        {
            Debug.LogError("No solution");
        }
        else
        {
            //Generate random entry and exit portals
            GenerateEntryAndExit(grid);
            //Close all open paths with "P rooms"
            CloseOpenPaths(grid);
            //Export all dungeon from 2D to 2D world
            GridToDungeonExport(grid);
            generated = true;
        }
    }

    /*
     * Method for generating the 2D matrix
     */
    public bool GenerateDungeonFinal(List<Room> grid, int roomsToGenerate)
    {
        for (int i = 0; i < grid.Count && roomsToGenerate > 0; i++)
        {
            List<Room> arrayPosiblesOpciones = GetCompatibles(grid, i); //get a list with the compatible rooms

            if (arrayPosiblesOpciones.Count > 0)
            {
                grid[i] = arrayPosiblesOpciones[0]; //We choose the first option
                if (isNormalRoomType(grid[i].type)) //If is not an end room
                {
                    roomsToGenerate--; 
                }
            }

            if (i == grid.Count - 1 && roomsToGenerate > 0) //If we are at the end of the grid but we still not finish we have to repeat the process
            {
                i = 0;
            }
        }

        if (roomsToGenerate <= 0)
        {
            PrintGrid();
            return true;
        }
        else
        {
            Debug.LogError("No solution, please try again");
            return false;
        }
    }

    /*
     * We have to check all open paths after generate 2D matrix and close them
     */
    public void CloseOpenPaths(List<Room> grid)
    {
        for (int i = 0; i < grid.Count; i++)
        {
            if (grid[i].type == RoomType.X)
            {
                Dictionary<Door, DirectionState> dictionaryDirections = new Dictionary<Door, DirectionState>();

                //Check up
                dictionaryDirections.Add(Door.Up, CheckDirection(grid, i, Door.Up));
                //Check right
                dictionaryDirections.Add(Door.Right, CheckDirection(grid, i, Door.Right));
                //Check down
                dictionaryDirections.Add(Door.Down, CheckDirection(grid, i, Door.Down));
                //Check left
                dictionaryDirections.Add(Door.Left, CheckDirection(grid, i, Door.Left));

                if (dictionaryDirections[Door.Left] == DirectionState.open && dictionaryDirections[Door.Up] == DirectionState.open && dictionaryDirections[Door.Down] == DirectionState.open)
                {
                    grid[i] = new Room(RoomType.D);
                }
                else if (dictionaryDirections[Door.Left] == DirectionState.open && dictionaryDirections[Door.Right] == DirectionState.open && dictionaryDirections[Door.Up] == DirectionState.open)
                {
                    grid[i] = new Room(RoomType.E);
                }
                else if (dictionaryDirections[Door.Left] == DirectionState.open && dictionaryDirections[Door.Right] == DirectionState.open && dictionaryDirections[Door.Down] == DirectionState.open)
                {
                    grid[i] = new Room(RoomType.C);
                }
                else if (dictionaryDirections[Door.Up] == DirectionState.open && dictionaryDirections[Door.Down] == DirectionState.open && dictionaryDirections[Door.Right] == DirectionState.open)
                {
                    grid[i] = new Room(RoomType.B);
                }
                else if (dictionaryDirections[Door.Up] == DirectionState.open && dictionaryDirections[Door.Down] == DirectionState.open)
                {
                    grid[i] = new Room(RoomType.O);
                }
                else if (dictionaryDirections[Door.Left] == DirectionState.open && dictionaryDirections[Door.Right] == DirectionState.open)
                {
                    grid[i] = new Room(RoomType.N);
                } 
                else if (dictionaryDirections[Door.Up] == DirectionState.open && dictionaryDirections[Door.Left] == DirectionState.open)
                {
                    grid[i] = new Room(RoomType.H);
                }
                else if (dictionaryDirections[Door.Up] == DirectionState.open && dictionaryDirections[Door.Right] == DirectionState.open)
                {
                    grid[i] = new Room(RoomType.I);
                }
                else if (dictionaryDirections[Door.Down] == DirectionState.open && dictionaryDirections[Door.Right] == DirectionState.open)
                {
                    grid[i] = new Room(RoomType.F);
                }
                else if (dictionaryDirections[Door.Down] == DirectionState.open && dictionaryDirections[Door.Left] == DirectionState.open)
                {
                    grid[i] = new Room(RoomType.G);
                }
                else if (dictionaryDirections[Door.Up] == DirectionState.open)
                {
                    //J room
                    grid[i] = new Room(RoomType.J);
                }
                else if (dictionaryDirections[Door.Right] == DirectionState.open)
                {
                    //K room
                    grid[i] = new Room(RoomType.K);
                }
                else if (dictionaryDirections[Door.Down] == DirectionState.open)
                {
                    //L room
                    grid[i] = new Room(RoomType.L);
                }
                else if (dictionaryDirections[Door.Left] == DirectionState.open)
                {
                    //M room
                    grid[i] = new Room(RoomType.M);
                }
            }
        }
        PrintGrid();
    }

    /*
     * This method is in charge of adjusting the size of the matrix to the most suitable one depending on the size of the dungeon. If it is too big it will lower the efficiency, if it is too small it might not find a solution.
     */
    public void AdaptGrid()
    {
        if (roomsToGenerate > (sizeX * sizeY) / 2)
        {
            sizeY = (int)Mathf.Sqrt(roomsToGenerate * 2) + 1;
            sizeX = sizeY;
        }
    }

    /*
     * Set all values to X, X == empty space
     */
    public void InitGrid()
    {
        grid.Clear();
        for (int i = 0; i < (sizeX * sizeY); i++)
        {
            grid.Add(new Room(RoomType.X));
        }
    }

    /*
     * Print method in debug console
     */
    public void PrintGrid()
    {
        string printedGrid = "Grid: ";

        for (int i = 0; i < grid.Count; i++)
        {
            if (i % sizeY == 0)
            {
                printedGrid += "\n " + grid[i].type;
            }
            else
            {
                printedGrid += " " + grid[i].type;
            }
        }
        Debug.Log($" {printedGrid}");
    }

    /*
     * Exporter of the 2D grid to the 3D world
     */
    public void GridToDungeonExport(List<Room> grid)
    {
        for (int i = 0; i < grid.Count; i++)
        {
            if (grid[i].type != RoomType.X)
            {
                var roomToInstantiate = GetRoomPrefab(grid[i].type);
                var currentRoom = Instantiate(roomToInstantiate, GetPositionWorld(i), roomToInstantiate.transform.rotation);
                currentRoom.transform.parent = dungeonContainer.transform;

                if (grid[i].rol == Rol.Exit)
                {
                    //Intantiate exit portal
                    var gameplayObj = Instantiate(exitPortalPrefab, GetPositionWorld(i), exitPortalPrefab.transform.rotation);
                    gameplayObj.transform.parent = currentRoom.transform;
                }
            }
        }
    }

    /*
     * Change from position in the theoretical matrix to position in space.
     */
    public Vector3 GetPositionWorld(int positionGrid)
    {
        return new Vector3((positionGrid / sizeY) * distance, 0, (positionGrid % sizeY) * distance);
    }

    /*
     * Given the room type returns the prefab for that room.
     */
    public GameObject GetRoomPrefab(RoomType type)
    {
        GameObject target = null;
        bool enc = false;

        for (int i = 0; i < predefinedRooms.Count && !enc; i++)
        {
            if (predefinedRooms[i].GetComponent<Room>().type == type)
            {
                enc = true;
                target = predefinedRooms[i];
            }
        }

        return target;
    }

    /*
     * Check if the indicated type is not a "end room"
     */
    public bool isNormalRoomType(RoomType type)
    {
        bool isValid = true;
        //Si en un futuro tengo las salas J,K, L, M se quitarian del IF porque pasariamos a contarlos
        if (type == RoomType.X || type == RoomType.J || type == RoomType.K || type == RoomType.L || type == RoomType.M)
        {
            isValid = false;
        }

        return isValid;
    }

    /*
     * Return a list with compatible rooms in one specific position
     */
    public List<Room> GetCompatibles(List<Room> grid, int position)
    {
        List<Room> compatibles = new List<Room>();
        Dictionary<Door, DirectionState> dictionaryDirections = new Dictionary<Door, DirectionState>();

        //UP
        dictionaryDirections.Add(Door.Up, CheckDirection(grid, position, Door.Up));
        //RIGHT
        dictionaryDirections.Add(Door.Right, CheckDirection(grid, position, Door.Right));
        //DOWN
        dictionaryDirections.Add(Door.Down, CheckDirection(grid, position, Door.Down));
        //LEFT
        dictionaryDirections.Add(Door.Left, CheckDirection(grid, position, Door.Left));

        //Check if the connection with previous room exists (and it is not the case 0), in case of not having connection it does not even try to look for compatible (default X value)
        if (ValidRoom(position, dictionaryDirections))
        {
            compatibles = GenerateOptions(dictionaryDirections);
        }
        else
        {
            compatibles.Add(new Room(RoomType.X));
        }

        return compatibles;
    }

    /*
     * Check if it is a valid room
     */
    public bool ValidRoom(int currentPos, Dictionary<Door, DirectionState> dictionaryDirections)
    {
        if (currentPos == 0)
        {
            return true;
        }

        if (dictionaryDirections[Door.Up] == DirectionState.open || dictionaryDirections[Door.Left] == DirectionState.open)
        {
            return true;
        }

        return false; ;
    }

    /*
     * Check direction in the indicated room
     */
    public DirectionState CheckDirection(List<Room> grid, int currentPos, Door direction)
    {
        //There are 3 options:
        // - There is nothing above (X)
        // - Something blocking at the top (end of the array or room with no door)
        // - There is a room with a door

        DirectionState state = DirectionState.block;

        int posToCheck = 0;
        int currentRow = 0;
        int posToCheckRow = 0;

        switch (direction)
        {
            case Door.Up:
                //Up direction from down
                posToCheck = currentPos - sizeY;
                state = CheckPosition(grid, posToCheck, Door.Down);
                break;

            case Door.Right:
                posToCheck = currentPos + 1;
                currentRow = currentPos / sizeY;
                posToCheckRow = posToCheck / sizeY;

                if (currentRow == posToCheckRow)
                {
                    state = CheckPosition(grid, posToCheck, Door.Left);
                }
                else
                {
                    state = DirectionState.block;
                }
                break;

            case Door.Down:
                posToCheck = currentPos + sizeY;
                state = CheckPosition(grid, posToCheck, Door.Up);
                break;

            case Door.Left:
                posToCheck = currentPos - 1;
                currentRow = currentPos / sizeY;
                posToCheckRow = posToCheck / sizeY;

                if (currentRow == posToCheckRow)
                {
                    state = CheckPosition(grid, posToCheck, Door.Right);
                }
                else
                {
                    state = DirectionState.block;
                }
                break;
        }

        return state;
    }

    /*
     * Check position
     */
    public DirectionState CheckPosition(List<Room> grid, int posToCheck, Door door)
    {
        DirectionState state = DirectionState.block;

        if (posToCheck < 0 || posToCheck >= grid.Count)//Check up and down
        {
            state = DirectionState.block; //out of range
        }
        else if (grid[posToCheck].type == RoomType.X)
        {
            state = DirectionState.empty; //empty space
        }
        else if (grid[posToCheck].doorDictionary[door]) 
        {
            state = DirectionState.open; //open room
        }
        else if (!grid[posToCheck].doorDictionary[door])
        {
            state = DirectionState.block; //block room
        }

        return state;
    }

    /*
     * Generate room options based on the direction of the doors
     */
    public List<Room> GenerateOptions(Dictionary<Door, DirectionState> dictionaryDirections)
    {
        List<Room> disponibleRooms = new List<Room>();

        disponibleRooms.Add(new Room(RoomType.A)); 
        disponibleRooms.Add(new Room(RoomType.B));
        disponibleRooms.Add(new Room(RoomType.C));
        disponibleRooms.Add(new Room(RoomType.D));
        disponibleRooms.Add(new Room(RoomType.E));
        disponibleRooms.Add(new Room(RoomType.F));
        disponibleRooms.Add(new Room(RoomType.G));
        disponibleRooms.Add(new Room(RoomType.H));
        disponibleRooms.Add(new Room(RoomType.I));

        disponibleRooms.Add(new Room(RoomType.N));
        disponibleRooms.Add(new Room(RoomType.O));

        List<Room> compatibles = new List<Room>();

        compatibles = Filter(dictionaryDirections, disponibleRooms);

        //Randomize list
        ListOperations.Shuffle<Room>(compatibles);

        return compatibles;
    }

    /*
     * Filter compatible rooms after generate opcions
     */
    public List<Room> Filter(Dictionary<Door, DirectionState> dictionaryDirections, List<Room> candidates)
    {
        for (int i = candidates.Count - 1; i >= 0; i--)
        {
            if (!candidates[i].isValid(dictionaryDirections[Door.Up], Door.Up))
            {
                candidates.RemoveAt(i);
            }
            else if (!candidates[i].isValid(dictionaryDirections[Door.Right], Door.Right))
            {
                candidates.RemoveAt(i);
            }
            else if (!candidates[i].isValid(dictionaryDirections[Door.Down], Door.Down))
            {
                candidates.RemoveAt(i);
            }
            else if (!candidates[i].isValid(dictionaryDirections[Door.Left], Door.Left))
            {
                candidates.RemoveAt(i);
            }
        }

        return candidates;
    }

    /*
     * Destroy generated dungeon
     */
    public void ResetDungeon()
    {
        foreach (Transform child in dungeonContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        entryPos = 0;

        generatedRooms.Clear();
    }

    /*
     * Return the position of the player at the begining of each level
     */
    public Vector3 GetEntryPosition()
    {
        return GetPositionWorld(entryPos);
    }

    /*
     * Generator of entry and exit portals in random rooms
    */
    public void GenerateEntryAndExit(List<Room> grid)
    {
        bool entryEnc = false;
        bool exitEnc = false;

        while (!entryEnc || !exitEnc)
        {
            int rndPos = Random.Range(0, grid.Count);
            if (grid[rndPos].type != RoomType.X)
            {
                if (!entryEnc)
                {
                    grid[rndPos].rol = Rol.Entry;
                    entryEnc = true;
                    Debug.Log($"Entry position {rndPos}");
                    entryPos = rndPos;
                }
                else if (rndPos != entryPos)
                {
                    grid[rndPos].rol = Rol.Exit;
                    exitEnc = true;
                    Debug.Log($"Exit position {rndPos}");
                }
            }
        }
    }


    #endregion
}
