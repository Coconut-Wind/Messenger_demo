using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 全局UI管理
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject enemyHealthBarHolder;
    public GameObject playerStateHolder;
    public GameObject gameoverCanvas;
    public TextMeshProUGUI gameoverTitle;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetEnemyHealthBar()
    {
        return enemyHealthBarHolder;
    }

    //清除所有敌人血条
    public void ClearAllEnemyHealthBar()
    {
        for (int i = 0; i < enemyHealthBarHolder.transform.childCount; i++)
        {
            Destroy(enemyHealthBarHolder.transform.GetChild(i).gameObject);
        }
    }

    public PlayerStatesUI GetPlayerStateHolder()
    {
        return playerStateHolder.GetComponent<PlayerStatesUI>();
    }

    public void SetGameOverTitleText(string title)
    {
        gameoverTitle.text = title;
    }

    
}
