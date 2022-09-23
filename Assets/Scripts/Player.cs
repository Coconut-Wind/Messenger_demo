using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Movement
{
    public int maxHealth = 3;
    public int currentHealth; // 当前生命值
    public int letterNum = 2;
    public float maxSelectDistance = 1.5f; //鼠标与主角的最大选择距离
    [SerializeField] private GameObject arrow; //指示箭头
    [SerializeField] private GameObject attackArrow; //攻击指示箭头
    [SerializeField] private Transform arrowHolder;
    [SerializeField] private GameObject circle;

    private bool isShowingArrows = false; //是否正在显示箭头

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public override void init(Map map, Vector2Int index)
    {
        base.init(map, index);
        UIManager.instance.GetPlayerStateHolder().SetPlayer(this);
        GameManager.GM.PostPlayer(this);
    }

    public void SetCurrentHealth(int health)
    {
        currentHealth = (int)Mathf.Clamp(health, 0, maxHealth);
        Debug.Log("玩家受到攻击");
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void Update()
    {
        if (GameManager.GM.IsGameOver()) //如果游戏结束则不执行下面
        {
            //return;
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
            Vector2Int target = cellAdjustList[i].GetIndex();
            GameObject a = null;

            //根据目标点位旋转箭头
            Vector2 e = (Vector2)(target - GetIndex());
            e = new Vector2(e.y, e.x);

            e = e.normalized;
            float angle = Vector2.Angle(Vector2.right, e);
            if (e.y > 0)
                angle = -angle;
    
            //如果那个点位是有敌人
            if (GameManager.GM.isEnemy(target))
            {
                //TODO:攻击箭头
                Debug.Log(target + ", " + GetIndex() + ", " + e + ", " + angle);
                //根据相对位置显示相应箭头
                a = Instantiate(attackArrow, this.transform.position, Quaternion.identity);
                a.GetComponent<PlayerAttackArrow>().init(this, target);
            }
            else
            {
                Debug.Log(target + ", " + GetIndex() + ", " + e + ", " + angle);
                //根据相对位置显示相应箭头
                a = Instantiate(arrow, this.transform.position, Quaternion.identity);
                a.GetComponent<PlayerMoveArrow>().init(this, target);
            }
            
            //旋转到对应方向
            a.transform.SetParent(arrowHolder);
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
            delegate ()
            {
                //判断是否到达终点
                //Debug.Log("检测: ");
                if (GameManager.GM.IsOnTarget(GetIndex()))
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
                    }
                }
                //轮到敌方回合
                if (!GameManager.GM.IsGameOver())
                    GameManager.GM.NextTurn();
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

        for (int i = 0; i < num - 1; i++)
        {
            transform.position = transform.position + (Vector3)per;
            yield return new WaitForSeconds(Time.deltaTime * 15);
        }
        transform.position = targetPos;



        //执行回调函数
        finish();
        yield return null;
    }

    public void AttackEnemy(Enemy enemy)
    {
        enemy.SetCurrentHealth(enemy.GetCurrentHealth()-1);
        //轮到敌方回合
        if (!GameManager.GM.IsGameOver())
            GameManager.GM.NextTurn();
    }
}