using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Movement
{
    public int health = 2;

    //TODO: BFS / A*寻路
    public void ChacePlayer()
    {
        
    }

    IEnumerator Walk()
    {
        Vector2 substract = targetPos - (Vector2)transform.position;
        Vector2 targetPosDir = (substract).normalized;
        float dis = Vector2.Distance(transform.position, targetPos);
        float num = dis / speed;
        Vector2 per = substract / num;
        Debug.Log(dis + ", " + num);
        for (int i = 0; i < num-1; i++)
        {
            transform.position = transform.position + (Vector3)per;
            yield return new WaitForSeconds(0.05f);
        }
        transform.position = targetPos;

        //轮到敌方回合
        //GameManager.GM.nextTurn();
        yield return new WaitForSeconds(0.05f);
    }
}
