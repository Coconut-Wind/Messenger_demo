using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Movement
{
    public int maxHealth = 3;
    public int currentHealth; // 当前生命值
    public int letterNum = 2;
    public float maxSelectDistance = 0.5f; //鼠标与主角的最大选择距离
    public float maxArrowSelectDistance = 1.5f;
    [SerializeField] private GameObject attackArrow; //攻击指示箭头
    [SerializeField] private Transform arrowHolder; //存放攻击箭头的父节点
    //[SerializeField] private GameObject circle;

    private bool isMouseDown = false; //是否正在主角处按下鼠标
    private bool isMouseOver = false; //是否鼠标悬停在上方
    private bool isShowingArrow = false; //是否正在显示攻击箭头

    private bool runOnce = false; //控制该回合只执行一次的代码

    private Vector2Int lastPosition, nextPosition;
    private List<Cell> highLightCellList; // 高亮点位列表
    [Header("---- 受伤效果 ----")]
    [SerializeField] private Color originColor;
    [SerializeField] private Color flashColor;
    [SerializeField] private float flashTime;
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
    }

    public override void Init(Map map, Vector2Int index)
    {
        base.Init(map, index);
        UIManager.instance.GetPlayerStateHolder().SetPlayer(this);
        GameManager.instance.PostPlayer(this);

        //设置信的个数
        letterNum = GameManager.instance.GetTargetCount();
        UIManager.instance.GetPlayerStateHolder().SetLetterNumber(letterNum);
    }

    public void SetCurrentHealth(int health)
    {
        currentHealth = (int)Mathf.Clamp(health, 0, maxHealth);
        Debug.Log("玩家受到攻击");

        //检测死亡
        if (currentHealth == 0)
        {
            GameManager.instance.SetGameOver(true);
        }
    }

    public void CreateDamage(int damege)
    {
        FlashColor(); // 受伤效果
        int h = (int)Mathf.Clamp(currentHealth - damege, 0, maxHealth);
        SetCurrentHealth(h);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetRunOnce(bool once)
    {
        runOnce = once;
    }

    private void Update()
    {
        //如果游戏结束或者非玩家回合则不执行下面
        if (GameManager.instance.IsGameOver() || !GameManager.instance.IsPlayersTurn()) 
            return;

        if (!runOnce)
        {
            //Debug.Log("一次");
            runOnce = true;

            // 每个回合开始初始化高亮列表
            GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(false, highLightCellList);
            HideArrow();
            
            UpdateHighLightCellList();
            ShowArrow();
            GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(true, highLightCellList);
        }

        //鼠标事件
        //CheckMouseOver();
        //CheckMouseClick();
        //CheckMouseDrag();
    }

    //处理点击事件
    private void CheckMouseClick()
    {
        
        if (Input.GetMouseButtonDown(0)) //左键按下
        {
            if (isMouseOver)
            {
                isMouseDown = true;
                lastPosition = GetPosition();
            }
        }
        else if (Input.GetMouseButtonUp(0)) //左键弹起
        {
            if (isMouseDown)
            {
                isMouseDown = false;
                
                transform.position = GameManager.instance.GetCurrentMap().AdjustPosition(nextPosition);
                Debug.Log("last: " + lastPosition + ", next: " + nextPosition);
                if (nextPosition != lastPosition)
                {
                    //发生移动,下一回合
                    //吸附点位
                    SetPosition(nextPosition.x, nextPosition.y);
                    
                    //目标检测
                    CheckCellType();
                    GameManager.instance.NextTurn();
                }
            }
        }
    }


    //处理拖动事件
    private void CheckMouseDrag()
    {
        if (isMouseDown)
        {
            //可达点位高亮
            GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(true, highLightCellList);
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition;
            
            nextPosition = GetNearestPosition(mousePosition);
            
            //拖动主角时，不显示攻击箭头
            HideArrow();
        }
    }

    //检测鼠标是否至于置于主角上
    private void CheckMouseOver()
    {
        //将屏幕坐标转换为世界坐标
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 self = transform.position; //Camera.main.ViewportToWorldPoint(transform.position);
        float dis = Vector2.Distance(mouse, self);
        
        isMouseOver = false;

        //根据距离判断是否悬停在Player上
        //如果鼠标进入 主角选择 范围
        if (dis <= maxSelectDistance && !isMouseDown)
        {
            if (GameManager.instance.IsPlayersTurn())
            {
                isMouseOver = true;
                ShowArrow();
                //设置可达点位高亮
                //GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(true, highLightCellList);

            }
        }

        //如果鼠标不在 箭头选择 范围内
        if (dis > maxArrowSelectDistance)
            HideArrow();

        //如果没有检测到悬停，则不显示点位光圈
        //if (!isMouseOver)
            //GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(false, highLightCellList);

    }

    //获取离鼠标坐标最近的点位
    private Vector2Int GetNearestPosition(Vector2 mousePosition)
    {
        List<Cell> list = highLightCellList; //onCell.GetAdjCellList();
        //list.Add(onCell);
        float minDis = int.MaxValue;
        Vector2Int targetPos = lastPosition;
        foreach (Cell c in list)
        {
            //计算距离,保留距离最小的点位
            Vector2Int cPos = c.GetPosition();
            Vector2 cPosReal = GameManager.instance.GetCurrentMap().AdjustPosition(c.GetPosition());

            float disSq = (mousePosition.x - cPosReal.x) * (mousePosition.x - cPosReal.x) 
                        + (mousePosition.y - cPosReal.y) * (mousePosition.y - cPosReal.y);
            if (disSq < minDis)
            {
                minDis = disSq;
                targetPos = cPos;
            }
        }

        return targetPos;
    }

    

    //显示附近所有敌人的箭头
    public void ShowArrow()
    {
        if (isShowingArrow)
            return;
        isShowingArrow = true;

        List<Cell> cellAdjustList = onCell.GetAdjCellList();

        for (int i = 0; i < cellAdjustList.Count; i++)
        {
            Vector2Int target = cellAdjustList[i].GetPosition();
            GameObject a = null;

            //根据目标点位旋转箭头
            Vector2 e = (Vector2)(target - GetPosition());
            e = new Vector2(e.y, e.x);

            e = e.normalized;
            float angle = Vector2.Angle(Vector2.right, e);
            if (e.y > 0)
                angle = -angle;
    
            //如果那个点位是有敌人
            if (GameManager.instance.isEnemy(target))
            {
                Debug.Log(target + ", " + GetPosition() + ", " + e + ", " + angle);
                //根据相对位置显示相应箭头
                a = Instantiate(attackArrow, this.transform.position, Quaternion.identity);
                a.GetComponent<PlayerAttackArrow>().Init(this, target);
                //旋转到对应方向
                a.transform.SetParent(arrowHolder);
                a.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                a.transform.GetChild(0).rotation = Quaternion.identity;
            }
        }
    }

    //删除所有箭头
    public void HideArrow()
    {
        if (!isShowingArrow)
            return;
        isShowingArrow = false;
        //circle.SetActive(false);
        for (int i = 0; i < arrowHolder.childCount; i++)
        {
            Destroy(arrowHolder.GetChild(i).gameObject, 0f);
        }
    }

    //攻击敌人
    public void AttackEnemy(Enemy enemy, int damage = 1)
    {
        
        if (enemy.GetCurrentHealth() - damage <= 0)
        {//由于敌人死亡后立即被Destory，要先预判下死亡的状态
            HideArrow();
            UpdateHighLightCellList();
            GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(true, highLightCellList);
            ShowArrow();
        }
        //给敌人扣血
        enemy.CreateDamage(damage);

        //没有动画，先延时顶替一下
        //先隐藏攻击箭头
        HideArrow();
        GameManager.instance.Delay(delegate(){
            ShowArrow(); //等待完成后重新显示
            //轮到敌方回合
            if (!GameManager.instance.IsGameOver())
                GameManager.instance.NextTurn();
        }, 1f);
        
    }

    // 初始化高亮点位列表
    public void UpdateHighLightCellList()
    {
            
        highLightCellList = new List<Cell>(onCell.GetAdjCellList());
        
        // 去除列表中有敌人的点位
        List<Vector2Int> enemiesPos = GameManager.instance.enemiesManager.GetComponent<EnemiesManager>().GetEnemiesPositions();
        for(int i = 0; i<highLightCellList.Count; i++)
        {
            if(enemiesPos.Contains(highLightCellList[i].GetPosition()))
            {
                highLightCellList.Remove(highLightCellList[i]);
                i--;
            }
        }
        highLightCellList.Add(onCell);
    }

    public List<Cell> GetHeightLightCellList()
    {
        return new List<Cell>(highLightCellList);
    }



    //移动
    public override void WalkTo(Vector2Int to)
    {
        Debug.Log("Walk to " + to);
        targetPos = GameManager.instance.GetCurrentMap().AdjustPosition(to);
        SetPosition(to.x, to.y);
        Vector2 substract = targetPos - (Vector2)transform.position;
        Vector2 targetPosDir = (substract).normalized;
        
        //isMoving = true;
        GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(false, highLightCellList);
        HideArrow();
        StartCoroutine(Walk(//开启协程
            //lambda, 当任务完成时执行
            delegate ()
            {
                //判断是否到达终点
                //Debug.Log("检测: ");
                CheckCellType();
                //isMoving = false;
                GameManager.instance.NextTurn();
                GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(true, highLightCellList);
                ShowArrow();
                return 0;
            }
        ));

    }

    private bool CheckCellType()
    {
        string type = GameManager.instance.GetCurrentMap().GetCellByIndex(GetPosition()).GetCellType();
        if (type == "TargetCell")
        {
            
            Debug.Log("到达目标");

            TargetCell tc = (TargetCell)GameManager.instance.GetCurrentMap().GetCellByIndex(GetPosition());
            if (!tc.isTriggered && letterNum > 0)
            {
                letterNum--;
                UIManager.instance.GetPlayerStateHolder().SetLetterNumber(letterNum); //修改ui显示

                tc.isTriggered = true;
                if (letterNum == 0)
                {
                    //任务完成
                    GameManager.instance.SetIsFinishedGoal(true);
                }
                return true;
            }
        }
        else if (type == "PosiCell")
        {
            Debug.Log("到达增益点位");

            Cell c = (Cell)GameManager.instance.GetCurrentMap().GetCellByIndex(GetPosition());
            if (!c.isTriggered)
            {
                SetCurrentHealth(GetCurrentHealth() + 1);
            }
        }
        else if (type == "NegaCell")
        {
            Debug.Log("到达减益点位");

            Cell c = (Cell)GameManager.instance.GetCurrentMap().GetCellByIndex(GetPosition());
            if (!c.isTriggered)
            {
                SetCurrentHealth(GetCurrentHealth() - 1);
            }
        }
        return false;
    }

    // 受伤效果
    public void FlashColor()
    {
        spriteRenderer.color = flashColor;
        Invoke("ResetColor", flashTime);
    }
    public void ResetColor()
    {
        spriteRenderer.color = originColor;
    }
}