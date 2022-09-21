using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    private Vector2Int playerPosition;
    private bool isPlayersTurn = true;

    //实现全局单例类
    private void Awake() {
        if (GM == null)
        {
            GM = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(GM != this)
        {
            Destroy(gameObject);
        }
    }
    
    //切换到对方回合
    public void NextTurn()
    {
        isPlayersTurn = !isPlayersTurn;
        Debug.Log("现在是" + (isPlayersTurn ? "玩家" : "敌人") + "回合");
    }

    //询问是否为玩家回合
    public bool IsPlayersTurn()
    {
        return isPlayersTurn;
    }

    //向GM提交玩家位置
    public void PostPlayerPosition(Vector2Int pos)
    {
        playerPosition = pos;
    }

    //向GM索取玩家位置
    public Vector2Int GetPlayerPosition()
    {
        return playerPosition;
    }
}
