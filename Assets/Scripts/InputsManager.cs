using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputsManager : MonoBehaviour
{
    //Singleton
    private static InputsManager instance = null;
    public static InputsManager Instance
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
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) //Menu
        {
            MainMenu.Instance.OpenMainMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) //Finish
        {
            Application.Quit();
        }

    }
}
