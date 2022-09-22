using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Movement
{
    public int health = 3;
    public int letterNum = 2;
    public float maxSelectDistance = 1.5f; //鼠标与主角的最大选择距离
    [SerializeField]private GameObject arrow; //指示箭头
    [SerializeField]private Transform arrowHolder;
    [SerializeField]private GameObject circle;
    
    private bool isShowingArrows = false; //是否正在显示箭头

    public override void SetIndex(int _x, int _y)
    {
        base.SetIndex(_x, _y);
        //向GM提交当前坐标
        GameManager.GM.PostPlayerPosition(new Vector2Int(_x, _y));
    }

    private void Update()
    {
        if (GameManager.GM.IsGameOver())
        {
            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            //将屏幕坐标转换为世界坐标
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //将人物坐标转换为
            Vector2 self = transform.position; //Camera.main.ViewportToWorldPoint(transform.position);
            float dis = Vector2.Distance(mouse, self);
            //根据距离判断是否点到Player
            if (dis <= maxSelectDistance && !isShowingArrows)
            {
                if (GameManager.GM.IsPlayersTurn())
                {
                    isShowingArrows = true;
                    ShowArrow();
                }
            }
            else
            {
                if (isShowingArrows)
                {
                    isShowingArrows = false;
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

    //移动
    public override void WalkTo(Vector2Int to)
    {
        Debug.Log("Walk to " + to);
        targetPos = mapObject.AdjustPosition(to.y, to.x);
        SetIndex(to.x, to.y);
        Vector2 substract = targetPos - (Vector2)transform.position;
        Vector2 targetPosDir = (substract).normalized;
        //开启协程
        StartCoroutine(Walk(
            //lambda, 当任务完成时执行
            delegate(){
                //判断是否到达终点
                //Debug.Log("检测: ");
                if (GameManager.GM.isOnTarget(GetIndex()))
                {
                    Debug.Log("到达目标");
                    
                    TargetCell tc = (TargetCell)mapObject.GetCellByIndex(GetIndex());
                    if (!tc.isReached && letterNum > 0)
                    {
                        letterNum--;
                        tc.isReached = true;
                        if (letterNum == 0)
                        {
                            //任务完成
                            GameManager.GM.SetIsFinishedGoal(true);
                        }
                        
                        //轮到敌方回合
                        GameManager.GM.NextTurn();
                    }
                }    
                return 0;
            }
        ));
        
    }
    
    //人物运动的协程
    IEnumerator Walk(System.Func<int> finish)
    {
        Vector2 substract = targetPos - (Vector2)transform.position;
        Vector2 targetPosDir = (substract).normalized;
        float dis = Vector2.Distance(transform.position, targetPos);
        float num = dis / speed;
        Vector2 per = substract / num;
        
        Debug.Log(targetPosDir);

        for (int i = 0; i < num-1; i++)
        {
            transform.position = transform.position + (Vector3)per;
            yield return new WaitForSeconds(Time.deltaTime * 15);
        }
        transform.position = targetPos;

        
        
        //执行回调函数
        finish();
        yield return null;
    }


}
