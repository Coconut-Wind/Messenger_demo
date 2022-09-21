using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Movement
{
    public int health = 3;
    public float chooseMaxDistance = 1.5f;
    [SerializeField]private GameObject arrow;
    [SerializeField]private Transform arrowHolder;
    [SerializeField]private GameObject circle;
    
    private bool isUnderMouse = false;

    public override void SetIndex(int _x, int _y)
    {
        base.SetIndex(_x, _y);
        GameManager.GM.PostPlayerPosition(new Vector2Int(_x, _y));
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //将屏幕坐标转换为世界坐标
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float dis = Vector2.Distance(mouse, this.transform.position);
            //根据距离判断是否点到Player
            if (dis <= chooseMaxDistance && !isUnderMouse)
            {
                if (GameManager.GM.IsPlayersTurn())
                {
                    isUnderMouse = true;
                    ShowArrow();
                }
            }
            else
            {
                if (isUnderMouse)
                {
                    isUnderMouse = false;
                    HideArrow();
                }
            }
        }
            
    }

    //显示圆圈和所有可达箭头
    private void ShowArrow()
    {
        //Debug.Log("show");
        List<Cell> cellAdjustList = onCell.GetAdjCellList();
        circle.SetActive(true);
        for (int i = 0; i < cellAdjustList.Count; i++)
        {
            //根据相对位置显示相应箭头
            GameObject a = Instantiate(arrow, this.transform.position, Quaternion.identity);
            a.transform.SetParent(arrowHolder);
            a.GetComponent<PlayerMoveArrow>().init(this, cellAdjustList[i].GetIndex());
            
            //根据目标点位旋转箭头
            Vector2 e = (Vector2)(cellAdjustList[i].GetIndex() - GetIndex());
            e = new Vector2(e.y, e.x);
            
            e = e.normalized;
            float angle = Vector2.Angle(Vector2.right, e);
            if (e.y > 0)
            {
                angle = -angle;
            }
            Debug.Log(cellAdjustList[i].GetIndex() +", "+ GetIndex()+", "+e+", "+angle);
            a.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    //删除所有箭头，隐藏圆圈
    private void HideArrow()
    {
        circle.SetActive(false);
        for (int i = 0; i < arrowHolder.childCount; i++)
        {
            Destroy(arrowHolder.GetChild(i).gameObject, 0f);
        }
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
        GameManager.GM.NextTurn();
        yield return new WaitForSeconds(0.05f);
    }


}
