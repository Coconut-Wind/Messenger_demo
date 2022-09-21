using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;
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
    
    public void NextTurn()
    {
        isPlayersTurn = !isPlayersTurn;
        Debug.Log("现在是" + (isPlayersTurn ? "玩家" : "敌人") + "回合");
    }

    public bool IsPlayersTurn()
    {
        return isPlayersTurn;
    }
}
