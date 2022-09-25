using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackArrow : MonoBehaviour
{
    private Player player; 
    private Vector2Int to; //所指向的目标位置

    public void Init(Player player, Vector2Int to)
    {
        this.player = player;
        this.to = to;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonUp(0) || GameManager.instance.IsGameOver())
            return;
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = transform.GetChild(0).position;
        if (Vector2.Distance(pos, mousePos) < 0.7f)
        {
            Debug.Log("Attack at: " + to);
            this.player.AttackEnemy(GameManager.instance.enemiesManager.GetComponent<EnemiesManager>().GetEnemyByPos(to)); //发起攻击
        }
    }
}
