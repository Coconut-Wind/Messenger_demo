using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GM，用于全局数据交换
public class GameManager : MonoBehaviour
{
    public static GameManager GM; //静态唯一实例

    private Vector2Int playerPosition; //玩家所在点位
    private List<Vector2Int> targetPositions; //目标点位
    private bool isPlayersTurn = true; //回合判断
    private bool isFinishedGoal = false; //任务完成判断
    private bool isGameOver = false;

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
        if (isGameOver || isFinishedGoal){
            return;
        }
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

    public void PostTargetPositions(List<Vector2Int> targetList)
    {
        targetPositions = targetList;
    }

    public bool IsOnTarget(Vector2Int pos)
    {
        return targetPositions.Contains(pos);
    }

    //向GM索取玩家位置
    public Vector2Int GetPlayerPosition()
    {
        return playerPosition;
    }

    public void SetIsFinishedGoal(bool finish)
    {
        isFinishedGoal = finish;
        if (finish)
        {
            isGameOver = true;
            Debug.Log("任务完成");
        }
        
    }

    public bool IsFinishedGoal()
    {
        return isFinishedGoal;
    }

    public void SetGameOver(bool over)
    {
        Debug.Log("GG");
        isGameOver = over;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}
