using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敌人管理器，挂在敌人的父节点上
public class EnemiesManager : MonoBehaviour
{
    private int unreachEnemyCount_ = 0; //未走到目标点的敌人数量
    public int unreachEnemyCount
    {
        set
        {
            unreachEnemyCount_ = value;
            Debug.Log("unreach: " + value);
            if (unreachEnemyCount == 0)
            {
                GameManager.instance.SetTopBar(true); //将topbar改成玩家回合
                GameManager.instance.turnState = GameManager.TurnState.WaitngPlayer;
            }
        }
        get { return unreachEnemyCount_; }
    }
    public Enemy enemyShowingAdjustCell = null;

    private void Start()
    {
        //场景重载时这个gameobject会被销毁，然后new一个新的
        //这使得单例类GameManager中的enemiesManager丢失
        //因此需要手动设置一下
        GameManager.instance.enemiesManager = this.gameObject;

        unreachEnemyCount = 0;
    }
    private void Update()
    {

        //如果不是玩家回合，且游戏没有结束，则开始行动
        if (GameManager.instance.IsGameOver())
            return;
        if (!GameManager.instance.IsPlayersTurn())
        {
            unreachEnemyCount = transform.childCount;
            
            if (transform.childCount > 0)
            {
                GameManager.instance.turnState = GameManager.TurnState.EnemyMoving;
                
                GameManager.instance.SetTopBar(false); //将topbar改成敌人回合

                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetComponent<Enemy>().ChasePlayer();
                }
                GameManager.instance.NextTurn();
            }
            else
            {
                GameManager.instance.NextTurn();
                GameManager.instance.turnState = GameManager.TurnState.WaitngPlayer;
                GameManager.instance.SetTopBar(true);
            }
            
        }
        else
        {
            //点击敌人之后触发
            if (Input.GetMouseButtonUp(0))
            {
                ClearHightLightEnemyReachablePoint();

                bool find = false;

                Vector2 mpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform et = transform.GetChild(i);
                    Enemy script = et.GetComponent<Enemy>();

                    if (Vector2.Distance(mpos, et.position) <= 1f)
                    {
                        Debug.Log("from:" + this + ", " + Vector2.Distance(mpos, et.position));
                        if (!UIManager.instance.enemyStateHolder.activeSelf
                            || UIManager.instance.GetEnemyStateHolder().enemy != script)
                        {
                            UIManager.instance.ShowEnemyInfo(script);

                            GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(false,
                                GameManager.instance.player.GetHeightLightCellList());

                            GameManager.instance.GetCurrentMap().SetHightLightEnemyReachablePoint(true, script);

                            enemyShowingAdjustCell = script;
                        }
                        else
                        {
                            UIManager.instance.ShowEnemyInfo(null);
                            GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(true,
                                GameManager.instance.player.GetHeightLightCellList());
                        }
                        find = true;

                        break; //检测到一个就立马退出
                    }
                }
                if (!find)
                {
                    UIManager.instance.ShowEnemyInfo(null);
                    GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(true,
                        GameManager.instance.player.GetHeightLightCellList());
                }

            }
        }
    }

    //获取所有敌人的位置的List
    public List<Vector2Int> GetEnemiesPositions()
    {
        List<Vector2Int> res = new List<Vector2Int>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Enemy e = transform.GetChild(i).GetComponent<Enemy>();
            if (e.gameObject.activeSelf) //由于Destroy不会立即执行，可以先设置active，根据active判断敌人是否存在
                res.Add(e.GetPosition());
        }
        return res;
    }

    // 拿到所有的敌人List
    public List<Enemy> GetEnemyList()
    {
        List<Enemy> enemies = new List<Enemy>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Enemy e = transform.GetChild(i).GetComponent<Enemy>();
            if (e.gameObject.activeSelf)
            {
                enemies.Add(e);
            }
        }
        return enemies;
    }

    //根据网格坐标获取敌人脚本
    public Enemy GetEnemyByPos(Vector2Int pos)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Enemy tmp = transform.GetChild(i).GetComponent<Enemy>();
            if (tmp.GetPosition() == pos)
            {
                return tmp;
            }
        }
        return null;
    }

    public void ClearHightLightEnemyReachablePoint()
    {
        if (enemyShowingAdjustCell != null)
        {
            Debug.Log(enemyShowingAdjustCell.GetPosition());
            GameManager.instance.GetCurrentMap().SetHightLightEnemyReachablePoint(false, enemyShowingAdjustCell);
            enemyShowingAdjustCell = null;
        }
    }

}
