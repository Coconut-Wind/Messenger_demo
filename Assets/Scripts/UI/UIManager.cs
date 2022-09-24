using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 全局UI管理
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject enemyHealthBarHolder;
    public GameObject playerStateHolder;

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

    public PlayerStatesUI GetPlayerStateHolder()
    {
        return playerStateHolder.GetComponent<PlayerStatesUI>();
    }
}
