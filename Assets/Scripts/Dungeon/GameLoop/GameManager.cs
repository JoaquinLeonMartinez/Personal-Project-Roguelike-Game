using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Singleton
    private static GameManager instance = null;
    public static GameManager Instance
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

    public float playerY = 2;
    public int currentStage = 1;
    public GameObject playerPrefab;
    public GameObject player;
    public bool resetPlayer = false;
    public GameObject Camera;

    private void FixedUpdate()
    {
        if (resetPlayer)
        {
            Vector3 entryPosWorld = DungeonManager.Instance.GetEntryPosition();
            player.transform.position = new Vector3(entryPosWorld.x, entryPosWorld.y + playerY, entryPosWorld.z);
            player.transform.rotation = playerPrefab.transform.rotation;
            resetPlayer = false;
            Debug.LogError("Reset player");
        }
    }
    public void GenerateStage(int dungeonSize)
    {
        DungeonManager.Instance.ResetDungeon();

        DungeonManager.Instance.GenerateDungeon(dungeonSize);
    }
    public void OnChangeStage()
    {
        currentStage++;
        int dungeonSize = DungeonManager.Instance.dungeonSize + 2; //TODO: Esto en un futuro se hara en base a X parametro

        //Borramos la actual y generamos una nueva con el nuevo tamaño
        GenerateStage(dungeonSize);

        //Tenemos que mover la posicion del player
        SetPlayer();

        //Actualizamos la UI
        UI_DungeonManager.Instance.menuInGame.UpdateGameUI();
    }

    public void SetPlayer()
    {
        Vector3 entryPosWorld = DungeonManager.Instance.GetEntryPosition();
        if (player == null)
        {
            player = Instantiate(playerPrefab, new Vector3(entryPosWorld.x, entryPosWorld.y + playerY, entryPosWorld.z), playerPrefab.transform.rotation);
            PlayerFollow pf = Camera.GetComponent<PlayerFollow>();
            pf.PlayerTransform = player.transform;
            pf.OnInit();
            pf.enabled = true;
        }
        else
        {
            resetPlayer = true;
        }

    }
}
