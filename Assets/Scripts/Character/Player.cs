using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : Movement
{
    public int attackDMG = 1;
    public int maxHealth = 3;
    public int currentHealth; // 当前生命值
    public int letterNum = 2;
    public float maxSelectDistance = 0.5f; //鼠标与主角的最大选择距离
    public float maxArrowSelectDistance = 1.5f;
    
    public int moveableTimes = 2; //可移动次数
    private int moveTime = 1; //目前的移动次数

    [SerializeField] private GameObject attackArrow; //攻击指示箭头
    [SerializeField] private Transform arrowHolder; //存放攻击箭头的父节点
    //[SerializeField] private GameObject circle;
    
    private bool isMouseDown = false; //是否正在主角处按下鼠标
    private bool isMouseOver = false; //是否鼠标悬停在上方
    private bool isShowingArrow = false; //是否正在显示攻击箭头

    public bool isMoving = false; //玩家是否正在移动

    private bool runOnce = false; //控制该回合只执行一次的代码

    private Vector2Int lastPosition, nextPosition;
    private List<Cell> highLightCellList; // 高亮点位列表
    [Header("---- 受伤效果 ----")]
    [SerializeField] private Color originColor;
    [SerializeField] private Color flashColor;
    [SerializeField] private float flashTime;
    private SpriteRenderer spriteRenderer;

    /// 使用道具事件，事件响应者：Property类；事件处理器：Perperty.PropertyAbility
    public event Action<Player, UsePropertyEventArgs> OnUseProperty; // 使用道具事件


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
    }

    private void Start() {
        //测试用：添加道具
        PropertyManager.instance.GenerateProperty(0);
        PropertyManager.instance.GenerateProperty(1);
    }

    private void Update()
    {
        //如果游戏结束或者非玩家回合则不执行下面
        if (GameManager.instance.IsGameOver() || !GameManager.instance.IsPlayersTurn()) 
            return;

        if (!runOnce) //每回合或一小步仅执行一次
        {
            runOnce = true;

            UIManager.instance.cameraController.LocateToPlayer(); // 镜头移动到玩家

            // 每个回合开始初始化高亮列表
            GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(false, highLightCellList);
            HideArrow();
            
            UpdateHighLightCellList();
            
            ShowArrow();
            GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(true, highLightCellList);
        }
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
        Debug.Log("设置玩家生命：" + health);
        //检测死亡
        if (currentHealth == 0)
        {
            GameManager.instance.SetGameOver(true);
        }
    }

    public void CreateDamage(int damege)
    {
        Debug.Log("玩家受到攻击");
        FlashColor(); // 受伤效果
        int h = (int)Mathf.Clamp(currentHealth - damege, 0, maxHealth);
        SetCurrentHealth(h);
        if (h > 0) //检测死亡
            AudioPlayer.instance.Play("negative"); //播放受伤音效
        else
            AudioPlayer.instance.Play("gameover"); //播放游戏结束音效
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetRunOnce(bool once)
    {
        runOnce = once;
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

    /// <summary>检测鼠标是否至于置于主角上 </summary>
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

    /// <summary> 获取离鼠标坐标最近的点位 </summary>
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
    public void AttackEnemy(Enemy enemy)
    {
        
        if (enemy.GetCurrentHealth() - attackDMG <= 0)
        {//由于敌人死亡后立即被Destory，要先预判下死亡的状态
            HideArrow();
            UpdateHighLightCellList();
            GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(true, highLightCellList);
            ShowArrow();
        }
        //给敌人扣血
        enemy.CreateDamage(attackDMG);

        //没有动画，先延时顶替一下
        //先隐藏攻击箭头
        HideArrow();
        GameManager.instance.Delay(delegate(){
            ShowArrow(); //等待完成后重新显示
            //下一步操作
            if (!GameManager.instance.IsGameOver())
            {
                NextStep();
            }
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
        AudioPlayer.instance.Play("Reach"); // 音效
        base.WalkTo(to);
        GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(false, highLightCellList);
        
        HideArrow();

        isMoving = true;

        //移动时会隐藏ui， 先记录下状态方便之后复原
        //isShowingEnemyStateHolder = UIManager.instance.enemyStateHolder.activeSelf;
        //UIManager.instance.HideInfoBars();
    }

    protected override int OnReachCell()
    {
        //判断是否到达终点
        
        isMoving = false; //取消移动状态

        CheckCellType();
        //isMoving = false;
        /*UIManager.instance.ShowInfoBars("player");
        if (isShowingEnemyStateHolder)
        {
            isShowingEnemyStateHolder = false;
            UIManager.instance.ShowInfoBars("enemy");
        }*/
        
        NextStep();
        //GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(true, highLightCellList);
        ShowArrow();
        return base.OnReachCell();
    }

    private void NextStep()
    {
        if (moveTime < moveableTimes)
        {
            moveTime++;
            runOnce = false;
        }
        else
        {
            moveTime = 1;
            GameManager.instance.NextTurn();
        }
        GameManager.instance.GetCurrentMap().SetHightLightAvailablePoint(false,  highLightCellList); //显示点位光圈
    }

    public void SkipTurn()
    {
        moveTime = moveableTimes;
        NextStep();
    }

    // TODO:这个方法需要修改，以配合使用事件系统
    private bool CheckCellType()
    {
        Cell cell = GameManager.instance.GetCurrentMap().GetCellByIndex(GetPosition());
        string type = cell.GetCellType();
        Debug.Log(type);
        if (type == "TargetCell")
        {
            
            Debug.Log("到达目标");

            TargetCell tc = (TargetCell)cell;
            if (!tc.isTriggered && letterNum > 0)
            {
                letterNum--;
                UIManager.instance.GetPlayerStateHolder().SetLetterNumber(letterNum); //修改ui显示
                tc.isTriggered = true;
                AudioPlayer.instance.Play("positive");
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

            if (!cell.isTriggered)
            {
                SetCurrentHealth(GetCurrentHealth() + 1);
                AudioPlayer.instance.Play("positive");
                cell.isTriggered = true;
            }
        }
        else if (type == "NegaCell")
        {
            Debug.Log("到达减益点位");

            if (!cell.isTriggered)
            {
                CreateDamage(1);
                AudioPlayer.instance.Play("negative");
                cell.isTriggered = true;
            }
        }
        else if (type == "GoldenCell")
        {
            Debug.Log("到达黄金点位");

            SetCurrentHealth(maxHealth);
            AudioPlayer.instance.Play("positive");
        }
        else if(type == "NormalCell")
        {
            Debug.Log("到达普通点位");

            if(!cell.isTriggered)
            {
                float rand = UnityEngine.Random.value; // [0.0, 1.0]
                if(rand <= EventManager.instance.eventTriggerProbability)
                {
                    EventManager.instance.EventHappen();
                }
                cell.isTriggered = true;
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


    /// <summary> 触发道具效果或者使用道具时调用该方法触发OnUseProperty事件 </summary>
    // 可能是被动道具 => 一获取就改变玩家属性，或者在特定时候发动（攻击时，移动结束后）
    // 可能是主动道具 => 通过道具栏的道具使用按钮调用（恢复生命值，传送）
    public void UseProperty(UsePropertyEventArgs _args)
    {
        OnUseProperty(this, _args);
    }
}