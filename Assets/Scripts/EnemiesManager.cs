using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{

    void Update()
    {
        //如果不是玩家回合，则开始行动
        if (!GameManager.GM.IsPlayersTurn())
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Enemy>().CheasePlayer();
            }
            
            GameManager.GM.NextTurn();
        }
    }
}
