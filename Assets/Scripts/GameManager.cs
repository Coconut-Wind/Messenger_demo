using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//GM，用于全局数据交换
public class GameManager : MonoBehaviour
{
    public GameObject enemiesManager; //敌人管理器
    [HideInInspector] public Player player; //玩家
    public static GameManager instance; //静态唯一实例
    private Map currentMap;
    private Vector2Int playerPosition; //玩家所在点位
    private List<Vector2Int> targetPositions; //目标点位

    public enum TurnState
    {
        WaitngPlayer, //玩家移动或攻击之前
        PlayerMoving, //玩家移动或攻击之时
        EnemyMoving //玩家回合之后的敌方回合
    }
    public TurnState turnState = TurnState.WaitngPlayer;

    private bool isPlayersTurn = true; //回合判断
    private bool isFinishedGoal = false; //任务完成判断
    private bool isGameOver = false; //游戏结束判断 
    //((isGameOver && isFinishedGoal)视为通关， (isGameOver && !isFinishedGoal)视为失败)

    private int turnCount_ = 1;
    public int turnCount
    {
        set {turnCount_ = value; Debug.Log("回合"+value);}
        get { return turnCount_; }
    }

    //实现全局单例类
    private void Awake()
    {

        if (instance != this && instance != null)
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

    //----游戏流程控制---
    //跳过玩家回合
    public void SkipPlayerTurn()
    {
        if(!EventManager.instance.isEventPanelOpen && !PropertyManager.instance.isOpenedPanel)
            player.SkipTurn();
    }

    //切换到对方回合
    public void NextTurn()
    {
        //若游戏结束则不理会
        if (isGameOver)
        {
            return;
        }
        // isPlayersTurn = !isPlayersTurn;
        isPlayersTurn = !isPlayersTurn;
        if (isPlayersTurn)
        {
            player.SetRunOnce(false);

            turnCount_++;
            List<Property> plist = PropertyManager.instance.GetPlayerOwnedPropertyList();
            Debug.Log(plist.Count);
            foreach (Property p in plist)
            {
                if (p.isEveryPlayerTurn)
                {
                    GameManager.instance.player.UseProperty(new UsePropertyEventArgs(p.propertyID,  enemiesManager.GetComponent<EnemiesManager>().GetEnemyList()));
                }
            }
        }
        Debug.Log("现在是" + (isPlayersTurn ? "玩家" : "敌人") + "回合");
    }


    //询问是否为玩家回合
    public bool IsPlayersTurn()
    {
        return isPlayersTurn;
    }

    public void SetTopBar(bool isPlayer)
    {
        Debug.Log("TopBar: " + isPlayer);
        if (isPlayer)
        {
            //Debug.Log(1);
            UIManager.instance.topBar_text.text = "玩家回合";
            UIManager.instance.topBar_skipButton.SetActive(true);
        }
        else
        {
            UIManager.instance.topBar_text.text = "敌人回合";
            UIManager.instance.topBar_skipButton.SetActive(false);
        }
    }



    public void Replay()
    {
        Debug.Log("Replay");
        // UIManager.instance.gameoverCanvas.SetActive(false);
        UIManager.instance.PopHidePanel(UIManager.instance.gameoverCanvas.transform.GetChild(0).gameObject);

        player.StopAllCoroutines();
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        UIManager.instance.ClearAllEnemyHealthBar();
        isGameOver = false;
        isFinishedGoal = false;
        isPlayersTurn = true;
        AudioPlayer.instance.SetBgmPlaying(true); //重启bgm
    }

    public void NextLevel()
    {
        Debug.Log("NextLevel");
        // UIManager.instance.gameoverCanvas.SetActive(false);
        UIManager.instance.PopHidePanel(UIManager.instance.gameoverCanvas.transform.GetChild(0).gameObject);

        player.StopAllCoroutines();


        LevelManager.SetCurrentLevel(LevelManager.currentLevelId + 1);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        UIManager.instance.ClearAllEnemyHealthBar();
        isGameOver = false;
        isFinishedGoal = false;
        isPlayersTurn = true;
        AudioPlayer.instance.SetBgmPlaying(true);

    }

    public void OtherLevels()
    {
        Debug.Log("OtherLevels");
        // UIManager.instance.gameoverCanvas.SetActive(false);
        UIManager.instance.PopHidePanel(UIManager.instance.gameoverCanvas.transform.GetChild(0).gameObject);
        
        SceneManager.LoadScene(2);
        AudioPlayer.instance.ReplayBgm();
        isGameOver = false;
        isFinishedGoal = false;
        isPlayersTurn = true;
    }


    //--------------------

    //向GM提交目标点位
    public void PostTargetPositions(List<Vector2Int> targetList)
    {
        targetPositions = targetList;
    }

    public List<Vector2Int> GetTargetPositions()
    {
        return targetPositions;
    }

    public int GetTargetCount()
    {
        return targetPositions.Count;
    }
    //判断玩家是否在目标点位
    public bool IsOnTarget(Vector2Int pos)
    {
        return targetPositions.Contains(pos);
    }

    //判断所在位置是否有敌人
    public bool isEnemy(Vector2Int pos)
    {
        return enemiesManager.GetComponent<EnemiesManager>()
                .GetEnemiesPositions().Contains(pos);
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
            SetGameOver(true);
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

        // UIManager.instance.gameoverCanvas.SetActive(true);
        UIManager.instance.PopShowPanel(UIManager.instance.gameoverCanvas.transform.GetChild(0).gameObject);

        //判断输赢
        if (isGameOver)
        {
            //停止bgm
            AudioPlayer.instance.SetBgmPlaying(false);
            if (isFinishedGoal)
            {
                //win
                AudioPlayer.instance.Play("win");
                UIManager.instance.SetGameOverTitleText("Mission Completed");
            }
            else
            {
                //lose
                //游戏结束音效在Player处
                UIManager.instance.SetGameOverTitleText("You Died");
            }
        }
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
            //arr[playerPosition.x, playerPosition.y] = true;
            //将其他敌人所在点位设为true
            List<Vector2Int> list = enemiesManager.GetComponent<EnemiesManager>()
                                    .GetEnemiesPositions();
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

    //-------道具通信-----
    /*
    public void AddItem(string itemName, string description="", string usedDesc = "")
    {
        UIManager.instance.GetItemsHolder().AddItem(itemName, description, usedDesc);
    }

    public void UseItem(string itemName, string usedDesc)
    {
        Debug.Log("使用了道具" + itemName);

        if (itemName == "马")
        {
            Debug.Log(usedDesc);
            player.moveableTimes = 2;
        }
    }
    */
    //-------------------
}
