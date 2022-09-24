using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敌人管理器，挂在敌人的父节点上
public class EnemiesManager : MonoBehaviour
{

    private void Update()
    {
        //如果不是玩家回合，且游戏没有结束，则开始行动
        if (GameManager.instance.IsGameOver())
            return;
        if (!GameManager.instance.IsPlayersTurn())
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Enemy>().ChasePlayer();
            }
            
            GameManager.instance.NextTurn();
        }
    }

    //获取所有敌人的位置的List
    public List<Vector2Int> GetEnemiesPositions()
    {
        List<Vector2Int> res = new List<Vector2Int>();
        for (int i = 0; i < transform.childCount; i++)
        {
            res.Add(transform.GetChild(i).GetComponent<Enemy>().GetPosition());
        }
        return res;
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
}
