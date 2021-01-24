using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_DungeonManager : MonoBehaviour
{
    public TMP_InputField roomsToGenerateInput;

    public GameObject playerPrefab;

    public Camera GlobalCamera;

    public Camera PlayerCamera;

    private void Start()
    {
        GlobalCamera.enabled = true;
        PlayerCamera.enabled = false;
    }

    public void OnClickGenerate()
    {
        if (!string.IsNullOrEmpty(roomsToGenerateInput.text))
        {
            DungeonManager.Instance.roomsToGenerate = int.Parse(roomsToGenerateInput.text.ToString());
        }

        DungeonManager.Instance.ResetDungeon();

        DungeonManager.Instance.GenerateDungeon();
    }

    public void OnClickClear()
    {
        DungeonManager.Instance.ResetDungeon();
    }

    public void OnClickStart()
    {
        //comprobar que se haya generado la muralla
        if (DungeonManager.Instance.generated)
        {
            //Si se ha generado instanciar al player en la posicion 0 (temporal)
            PlayerCamera.GetComponent<PlayerFollow>().PlayerTransform = Instantiate(playerPrefab, new Vector3(0, 4, 0), playerPrefab.transform.rotation).transform;
            PlayerCamera.GetComponent<PlayerFollow>().SetOffset();

            //Change camera
            ChangeCameras();

            //Desactivar UI
            this.gameObject.SetActive(false);
        }
    }

    public void ChangeCameras()
    {
        GlobalCamera.enabled = !GlobalCamera.enabled;
        PlayerCamera.enabled = !PlayerCamera.enabled;
    }
}
