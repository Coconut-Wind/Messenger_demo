using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackArrow : MonoBehaviour
{
    private Player player; 
    private Vector2Int to; //所指向的目标位置
    private bool doubleAttack = false;

    public void Init(Player player, Vector2Int to)
    {
        this.player = player;
        this.to = to;
    }

    private void Update()
    {
        // 如果面板打开了，禁止攻击
        if(PropertyManager.instance.isOpenedPanel || EventManager.instance.isEventPanelOpen)
        {
            return;
        }

        //处理双刃剑效果: 前提是攻击图标还存在
        if (doubleAttack)
        {
            doubleAttack = false;
            //延迟0.5秒再攻击一次
            Enemy enemy = GameManager.instance.enemiesManager.GetComponent<EnemiesManager>().GetEnemyByPos(to);
            GameManager.instance.Delay(delegate{
                player.AttackEnemy(enemy); //发起攻击
            }, 0.5f);
            return;
        }


        if (!Input.GetMouseButtonUp(0) || GameManager.instance.IsGameOver())
            return;
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = transform.GetChild(0).position;
        if (Vector2.Distance(pos, mousePos) < 0.7f)
        {
            Debug.Log("Attack at: " + to);
            Enemy enemy = GameManager.instance.enemiesManager.GetComponent<EnemiesManager>().GetEnemyByPos(to);
            player.AttackEnemy(enemy); //发起攻击

            if (player.isUsingDoubleSword)
            {
                float rand = UnityEngine.Random.value; // [0.0, 1.0]
                if (rand < 0.5f && enemy.currentHealth > 0) //附加条件：敌人还活着
                {
                    Debug.Log("双刃剑触发");
                    doubleAttack = true;
                }
                
            }
        }
    }
}
