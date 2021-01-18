using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Singleton
    private static MainMenu instance = null;
    public static MainMenu Instance
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

    public bool active = true;

    void Start()
    {
        OpenMainMenu();
    }


    void Update()
    {
        
    }

    public void OpenMainMenu()
    {
        Time.timeScale = 0;
        this.gameObject.SetActive(true);
        active = false;
    }

    public void CloseMainMenu()
    {
        active = false;
        //Disable menu:
        this.gameObject.SetActive(false);
        //Active game:
        Time.timeScale = 1;
    }

    public void OnArenaSelected()
    {
        CloseMainMenu();
    }

    public void OnDungeonSelected()
    {
        //Change scene
        SceneManager.LoadScene("Dungeon");
        //Generate dungeon
        CloseMainMenu();

    }

}
