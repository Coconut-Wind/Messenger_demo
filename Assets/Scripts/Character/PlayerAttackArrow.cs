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

    private void OnMouseUp() 
    {
        if (Input.GetMouseButtonUp(0)) //弹起的是否是左键
            this.player.AttackEnemy(GameManager.instance.enemiesManager.GetEnemyByPos(to)); //发起攻击
    }
}
