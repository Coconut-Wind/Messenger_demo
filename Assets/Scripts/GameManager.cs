using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GM，用于全局数据交换
public class GameManager : MonoBehaviour
{
    public EnemiesManager enemiesManager; //敌人管理器
    public Player player; //玩家
    public static GameManager GM; //静态唯一实例
    private Map currentMap;
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

    public Map GetCurrentMap()
    {
        return currentMap;
    }

    public void SetCurrentMap(Map mp) 
    {
        currentMap = mp;
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

    public void PostTargetPositions(List<Vector2Int> targetList)
    {
        targetPositions = targetList;
    }

    //判断玩家是否在目标点位
    public bool IsOnTarget(Vector2Int pos)
    {
        return targetPositions.Contains(pos);
    }

    public bool isEnemy(Vector2Int pos)
    {
        return enemiesManager.GetEnemiesPositions().Contains(pos);
    }

    //向GM提交player
    public void PostPlayer(Player player)
    {
        this.player = player;
    }

    //向GM索取玩家位置
    public Vector2Int GetPlayerPosition()
    {
        return player.GetIndex();
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

    //返回地图的状态图，true表示不可达，false表示可达
    //exceptCharas: 是否将角色所在点也设为不可达
    public bool[,] GetCurrentStateMap(bool exceptCharas)
    {
        Vector2Int shape = currentMap.GetMapShape();
        bool[,] arr = new bool[shape.y, shape.x];

        if (exceptCharas)
        {
            //将玩家所在点位设为true
            arr[playerPosition.x, playerPosition.y] = true;
            //将其他敌人所在点位设为true
            List<Vector2Int> list = enemiesManager.GetEnemiesPositions();
            foreach (Vector2Int pos in list)
            {
                arr[pos.x, pos.y] = true;
            }
        }


        return arr;
    }
}
