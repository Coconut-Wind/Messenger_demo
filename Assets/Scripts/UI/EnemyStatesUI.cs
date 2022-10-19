using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyStatesUI : MonoBehaviour
{
    [HideInInspector]public Enemy enemy = null;
    public TextMeshProUGUI nameText, dataText;

    private void Awake()
    {

    }


    private void Update() {
        if (enemy)
            UpdateInfo();
    }

    private void UpdateInfo()
    {
        nameText.text = enemy.enemyName;
        dataText.text = "health:" + enemy.GetCurrentHealth() + "/" + enemy.maxHealth + "\n" +
                        "zoom size:" + enemy.maxZoomDistance + "\n"+
                        "chase distance:" + enemy.maxChaseDistance + "\n" + 
                        "watch diatance:" + enemy.maxWatchingDistance;
    }
}
