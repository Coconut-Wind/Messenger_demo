using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 全局UI管理
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

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

    // 注意UIManager下孩子的顺序
    public GameObject GetEnemyHealthBar()
    {
        return transform.GetChild(0).gameObject;
    }
}
