using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Movement
{
    public string enemyName = "Robber";
    public int maxHealth = 2; //生命值
    public int currentHealth = 2; // 当前生命值
    public int maxChaseDistance = 2; //距离营地的最大值
    public int maxZoomDistance = 2; //距离营地的最大值
    public int maxWatchingDistance = 2; //官网距离到达离营最大值后，使其不返回营地的与玩家距离最大值
    public int maxAttackDistance = 1; //攻击距离范围
    public Vector2Int homePosition = Vector2Int.zero;
    private int state = 0; // 0不动 1追赶 2返回;

    private GameObject healthBar = null; //血条

    private bool isDead = false;
     [Header("---- 受伤效果 ----")]
    [SerializeField] private Color originColor;
    [SerializeField] private Color flashColor;
    [SerializeField] private float flashTime = 0.2f;
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    //重载init, 添加homePosition作为营地坐标
    public override void Init(Map map, Vector2Int index)
    {
        base.Init(map, index);
        homePosition = index;
    }


    //设置对应的血条，方便敌人死亡后将其消除
    public void SetHealthBar(GameObject healthBar)
    {
        this.healthBar = healthBar;
    }

    public void SetCurrentHealth(int health)
    {
        Debug.Log("设置敌人生命："+ health);
        currentHealth = (int)Mathf.Clamp(health, 0, maxHealth);

        if (currentHealth == 0)
        {
            isDead = true;
            //this.healthBar.SetActive(false);
            if (UIManager.instance.GetEnemyStateHolder().enemy == this)
            {
                UIManager.instance.ShowEnemyInfo(null);
                
            }

            GameManager.instance.Delay(delegate(){
                this.gameObject.SetActive(false);
                Destroy(this.healthBar);
                Destroy(this.gameObject);
            }, 0.8f);
            
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    //BFS搜索节点
    private struct Node
    {
        public Vector2Int pos; //当前位置
        public int step; //已走步数
        public int last; //前一步在List中的位置
        public Node(Vector2Int pos, int step, int last)
        {
            this.pos = pos;
            this.step = step;
            this.last = last;
        }
    }

    //追赶玩家
    public void ChasePlayer()
    {
        if (isDead)
            return;

        //两次搜索，获取三种距离
        Node nA = GetNextPosition(GetPosition(), homePosition); //从自身到营地
        Node nB = GetNextPosition(GetPosition(), GameManager.instance.GetPlayerPosition()); //从自身到玩家
        Debug.Log("从自身到营地, dis:"+ (nA.step) + ", pos:" + nA.pos);
        Debug.Log("从自身到玩家, dis:"+ (nB.step) + ", pos:" + nB.pos);
        Debug.Log("从营地到玩家, dis:"+ (nB.step + nA.step));
        
        //下一个点位的变量声明
        Vector2Int nextPos = Vector2Int.zero;


        //行为逻辑树
        
        //玩家进入领地范围
        if (nB.step + nA.step <= maxZoomDistance)
        {
            //是否进入攻击范围
            if (nB.step <= maxAttackDistance) 
                state = 3; //攻击
            else
                state = 1; //追赶
        }
        //如果离开了营地范围
        else if (nA.step > maxZoomDistance)
        {
            state = 2; //返回
        }
        //如果自己处于领地边缘
        else if (nA.step == maxZoomDistance)
        {
            //玩家进入观望范围
            if (nB.step <= maxWatchingDistance)
            {
                //玩家进入攻击范围
                if (nB.step <= maxAttackDistance)   
                    state = 3; //攻击
                else 
                    state = 0; //不动
            }
            else
            {
                state = 2; //返回
            }
        }
        
        //玩家进入攻击范围
        if (nB.step <= maxAttackDistance)
            state = 3;
        if (state == 3 && nB.step > maxAttackDistance)
            state = 0;

        Debug.Log("state: "+ state);
        //行为处理
        if (state == 1)
        {
            nextPos = nB.pos;
        }
        else if (state == 2)
        {
            nextPos = nA.pos;
        }
        else if (state == 0)
        {
            nextPos = GetPosition(); //不动
        }
        else if (state == 3)
        {
            nextPos = GetPosition(); //不动
            AttackPlayer(1); //攻击
        }

        Debug.Log("next position:" + nextPos);
        WalkTo(nextPos);
    }

    //获取要前往的下一个点
    private Node GetNextPosition(Vector2Int startPos, Vector2Int targetPos)
    {
        if (startPos == targetPos)
        {
            //Debug.Log(startPos + "->" + targetPos +" dis:"+ 0 + ", pos:" + targetPos);
            return new Node(targetPos, 0, 0);
        }

        //BFS
        List<Node> list = new List<Node>();
        Node first = new Node(startPos, 0, 0);

        bool[,] map = GameManager.instance.GetCurrentStateMap(true);//new bool[mapShape.y, mapShape.x];
        map[startPos.x, startPos.y] = true;

        list.Add(first);

        //bool isFound = false;
        int head = 0, tail = 1;
        int minDis = 9999;
        int minDisId = 0;
        bool isFound = false;

        while (head < tail && !isFound)
        {
            Node curr = list[head];
            Cell currCell = GameManager.instance.GetCurrentMap().GetCellByIndex(curr.pos);
            List<Cell> adjs = currCell.GetAdjCellList(); // 获取邻接表

            //枚举所有可行路径
            for (int i = 0; i < adjs.Count; i++)
            {
                //当前位置是否检索过？
                Vector2Int npos = adjs[i].GetPosition();
                int id = npos.x + npos.y * GameManager.instance.GetCurrentMap().GetMapShape().y;
                if (!map[npos.x, npos.y])
                {
                    list.Add(new Node(npos, curr.step + 1, head));
                    tail++;

                    //比对位置
                    if (targetPos == npos)
                    {
                        if (curr.step + 1 < minDis)
                        {
                            minDis = curr.step + 1;
                            minDisId = head;
                        }
                        //Debug.Log(startPos + "->" + targetPos +" dis:"+ dis + ", pos:" + npos);
                        isFound = true;
                    }

                    //添加搜索记录
                    map[npos.x, npos.y] = true;
                }
            }
            head++;
        }

        Node node = list[minDisId]; //找到的点，距离最小值

        //回溯，找到第二个点
        while (node.last != 0)
        {
            node = list[node.last];
        }
        return new Node(node.pos, minDis, 0); //此时借用Node返回结果，dis为起点与终点的最短距离，而非第二步的node.step
    }

    public void CreateDamage(int damege)
    {
        FlashColor(); // 受伤效果
        int h = (int)Mathf.Clamp(currentHealth - damege, 0, maxHealth);
        SetCurrentHealth(h);
        if (h > 0) //检测死亡
            AudioPlayer.instance.Play("negative"); //播放受伤音效
        else
            AudioPlayer.instance.Play("dead"); //播放死亡音效

    }

    public void AttackPlayer(int damage)
    {
        GameManager.instance.player.CreateDamage(damage);
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

    //移动到达时的回调
    protected override int OnReachCell()
    {
        AudioPlayer.instance.Play("Reach");
        //GameManager.instance.SetTopBar(true); //将topbar改成玩家回合
        EnemiesManager enemiesManager = GameManager.instance.enemiesManager.GetComponent<EnemiesManager>();
        enemiesManager.unreachEnemyCount--;
        if (enemiesManager.unreachEnemyCount == 0)
        {
            GameManager.instance.SetTopBar(true); //将topbar改成玩家回合
        }
        return base.OnReachCell();
    }

    private void Update() {
        
    }

}
