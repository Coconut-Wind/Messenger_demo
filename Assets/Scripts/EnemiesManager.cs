using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{

    private void Update()
    {
        //如果不是玩家回合，则开始行动
        if (!GameManager.GM.IsPlayersTurn())
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Enemy>().ChasePlayer();
            }
            
            GameManager.GM.NextTurn();
        }
    }

    public List<Vector2Int> GetEnemiesPositions()
    {
        List<Vector2Int> res = new List<Vector2Int>();
        for (int i = 0; i < transform.childCount; i++)
        {
            res.Add(transform.GetChild(i).GetComponent<Enemy>().GetIndex());
        }
        return res;
    }

    public Enemy GetEnemyByPos(Vector2Int pos)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Enemy tmp = transform.GetChild(i).GetComponent<Enemy>();
            if (tmp.GetIndex() == pos)
            {
                return tmp;
            }
        }
        return null;
    }
}
