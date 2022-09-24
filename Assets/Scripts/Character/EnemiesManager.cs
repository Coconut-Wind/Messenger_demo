using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敌人管理器，挂在敌人的父节点上
public class EnemiesManager : MonoBehaviour
{
    private void Start() {
        //场景重载时这个gameobject会被销毁，然后new一个新的
        //这使得单例类GameManager中的enemiesManager丢失
        //因此需要手动设置一下
        GameManager.instance.enemiesManager = this.gameObject;
    }
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
            Enemy e = transform.GetChild(i).GetComponent<Enemy>();
            if (e.gameObject.activeSelf) //由于Destroy不会立即执行，可以先设置active，根据active判断敌人是否存在
                res.Add( e.GetPosition());
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
