using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_DungeonManager : MonoBehaviour
{
    //Singleton
    private static UI_DungeonManager instance = null;
    public static UI_DungeonManager Instance
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

    public TMP_InputField roomsToGenerateInput;
    public GameObject menuInit;
    public UI_InGame menuInGame;

    private void Start()
    {
    }

    public void OnClickGenerate()
    {
        int dungeonSize = 4;

        if (!string.IsNullOrEmpty(roomsToGenerateInput.text))
        {
            dungeonSize = DungeonManager.Instance.roomsToGenerate = int.Parse(roomsToGenerateInput.text.ToString());
        }

        GameManager.Instance.GenerateStage(dungeonSize);
    }

    public void OnClickClear()
    {
        DungeonManager.Instance.ResetDungeon();
    }

    public void OnClickStart()
    {
        //Comprobar que se haya generado la muralla
        if (DungeonManager.Instance.generated)
        {
            //Si se ha generado instanciar al player en la posicion 0 (temporal)
            GameManager.Instance.SetPlayer();

            //Desactivar UI de inicio y activar la UI in game
            menuInit.gameObject.SetActive(false);
            menuInGame.gameObject.SetActive(true);
            menuInGame.UpdateGameUI();
        }
    }

}
