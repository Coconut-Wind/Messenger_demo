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
    public GameObject enemyStateHolder;
    //public GameObject itemsHolder;
    public GameObject gameoverCanvas;
    public GameObject propertyCanvas;
    public DragCamera cameraController;
    public TextMeshProUGUI gameoverTitle;
    public TextMeshProUGUI topBar_text;
    public GameObject topBar_skipButton;


    private void Awake()
    {
        if(instance != this && instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
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

    public EnemyStatesUI GetEnemyStateHolder()
    {
        return enemyStateHolder.GetComponent<EnemyStatesUI>();
    }

    /*public ItemsHolder GetItemsHolder()
    {
        return itemsHolder.GetComponent<ItemsHolder>();
    }*/

    public void SetGameOverTitleText(string title)
    {
        gameoverTitle.text = title;
    }

    public void ShowInfoBars(string type = "all")
    {
        if (type == "all" || type == "player")
            playerStateHolder.SetActive(true);
        if (type == "all" || type == "enemy")
            enemyStateHolder.SetActive(true);
    }

    public void HideInfoBars(string type = "all")
    {
        if (type == "all" || type == "player")
            playerStateHolder.SetActive(false);
        if (type == "all" || type == "enemy")
            enemyStateHolder.SetActive(false);
    }

    public void ShowEnemyInfo(Enemy e)
    {
        if (e != null)
        {
            enemyStateHolder.SetActive(true);
            GetEnemyStateHolder().enemy = e;

        }
        else
        {
            GetEnemyStateHolder().enemy = null;
            enemyStateHolder.SetActive(false);
        }
    }
}
