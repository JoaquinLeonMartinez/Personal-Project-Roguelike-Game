using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public int roomsToGenerate;
    public List<Room> predefinedRooms;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GenerateDungeon()
    {
        // Necesitamos:
        // Nº de salas
        // Array de salas predefinidas
        // Saber que salas son compatibles
        //Seguramente haya que hacer una clase room que contenga el tipo de room que es y la referencia al gameObject que la representa (serian prefabs no instanciados en la escena)
    }
}
