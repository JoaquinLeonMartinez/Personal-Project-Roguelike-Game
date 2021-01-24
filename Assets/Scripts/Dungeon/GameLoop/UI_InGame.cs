using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_InGame : MonoBehaviour
{
    public TextMeshProUGUI size;
    public TextMeshProUGUI stage;

    public void UpdateGameUI()
    {
        size.text = $"Size {DungeonManager.Instance.dungeonSize}";
        stage.text = $"Stage {GameManager.Instance.currentStage}";
    }
}
