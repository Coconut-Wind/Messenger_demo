using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Movement
{
    public int health = 2;
    
    struct Node
    {
        public Vector2Int pos;
        public int step;
        public int last;

        public Node(Vector2Int pos, int step, int last)
        {
            this.pos = pos;
            this.step = step;
            this.last = last;
        }

    }

    //追赶玩家
    public void CheasePlayer()
    {
        Vector2Int nextPos = GetNextPositionTowardPlayer();
        Debug.Log("next position:" + nextPos);
        WalkTo(nextPos);
    }

    private Vector2Int GetNextPositionTowardPlayer()
    {
        Vector2Int pos = GetIndex();
        List<Node> list = new List<Node>();
        list.Add(new Node(pos, 0, 0));
        List<int> searchPointIds = new List<int>();
        bool isFound = false;
        int head = 0, tail = 1;

        while (head < tail && !isFound)
        {
            Node curr = list[head];
            Cell currCell = mapObject.GetCellByIndex(curr.pos);
            List<Cell> adjs = currCell.GetAdjCellList(); // 获取邻接表

            //枚举所有可行路径
            for (int i = 0; i < adjs.Count; i++)
            {
                //当前位置是否检索过？
                Vector2Int npos = adjs[i].GetIndex();
                int id = npos.x + npos.y * mapObject.GetMapShape().y;
                if (searchPointIds.Contains(id))
                {
                    continue;
                }
                else
                {
                    list.Add(new Node(npos, curr.step + 1, head));
                    tail++;
                
                    //比对玩家位置
                    if (GameManager.GM.GetPlayerPosition() == npos)
                    {
                        Debug.Log("find player, dis:"+ (curr.step + 1) + ", pos:" + npos);
                        isFound = true;
                        break;
                    }

                    //添加搜索记录
                    searchPointIds.Add(id);
                }
                
            }
            head++;
        }
        Node node = list[tail - 1];
        //回溯，找到第二个点
        while (node.last != 0)
        {
            node = list[node.last];
        }
        return node.pos;
    }

    public override void WalkTo(Vector2Int to)
    {
        Debug.Log("Walk to " + to);
        targetPos = mapObject.AdjustPosition(to.y, to.x);
        SetIndex(to.x, to.y);
        StartCoroutine(Walk());
        
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
