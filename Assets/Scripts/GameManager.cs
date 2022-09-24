using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GM，用于全局数据交换
public class GameManager : MonoBehaviour
{
    public EnemiesManager enemiesManager; //敌人管理器
    public Player player; //玩家
    public static GameManager instance; //静态唯一实例
    private Map currentMap;
    private Vector2Int playerPosition; //玩家所在点位
    private List<Vector2Int> targetPositions; //目标点位
    
    private bool isPlayersTurn = true; //回合判断
    private bool isFinishedGoal = false; //任务完成判断
    private bool isGameOver = false; //游戏结束判断 
    //((isGameOver && isFinishedGoal)视为通关， (isGameOver && !isFinishedGoal)视为失败)

    //实现全局单例类
    private void Awake() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    //获取目前的地图
    public Map GetCurrentMap()
    {
        return currentMap;
    }

    //设置目前的地图
    public void SetCurrentMap(Map mp) 
    {
        currentMap = mp;
    }
    
    //切换到对方回合
    public void NextTurn()
    {
        //若游戏结束则不理会
        if (isGameOver){
            return;
        }
        isPlayersTurn = !isPlayersTurn;
        if (isPlayersTurn)
        {
            player.SetRunOnce(false);
        }
        Debug.Log("现在是" + (isPlayersTurn ? "玩家" : "敌人") + "回合");
        
        //更新玩家的攻击箭头
        
    }

    //询问是否为玩家回合
    public bool IsPlayersTurn()
    {
        return isPlayersTurn;
    }

    //向GM提交目标点位
    public void PostTargetPositions(List<Vector2Int> targetList)
    {
        targetPositions = targetList;
    }

    //判断玩家是否在目标点位
    public bool IsOnTarget(Vector2Int pos)
    {
        return targetPositions.Contains(pos);
    }

    //判断所在位置是否有敌人
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
        return player.GetPosition();
    }

    //设置是否完成目标，同时设置GameOver
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
    //主要用于获取点位是否可达
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

    //----延时工具----

    public void Delay(CustomVoid pDelegate, float time)
    {
        StartCoroutine(TimeWait(pDelegate, time));
    }

    protected IEnumerator TimeWait(CustomVoid pDelegate, float time)
    {
        yield return new WaitForSeconds(time);
        pDelegate.Invoke();
    }

    public delegate void CustomVoid();
    //----------------
}
