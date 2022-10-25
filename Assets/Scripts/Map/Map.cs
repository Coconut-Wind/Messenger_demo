using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Map : MonoBehaviour
{
    [Header("---- Prefabs ----")]
    [SerializeField] private GameObject nullCellPrefab; // 空点位的预制体
    [SerializeField] private GameObject normalCellPrefab; // 普通点位的预制体
    [SerializeField] private GameObject posiCellPrefab; // 增益点位的预制体
    [SerializeField] private GameObject negaCellPrefab; // 减益点位的预制体
    [SerializeField] private GameObject targetCellPrefab; // 目标点位的预制体
    [SerializeField] private GameObject goldenCellPrefab; // 黄金点位的预制体
    // TODO:还有增益点位和减益点位的预制体没创建
    [SerializeField] private GameObject edgePrefab; // 边的预制体
    [SerializeField] private GameObject player; //玩家的预制体
    public GameObject enemy; //敌人的预制体


    private GameObject[,] cellMap; // 整张点位地图，二维数组
    
    private List<Cell> allCells= new List<Cell>(); // 整张图的所有非空点位
    
    [Header("---- cellMap参数 ----")]
    [SerializeField] private int cellMapRow;
    [SerializeField] private int cellMapColumn;


    [Header("---- 渲染地图的参数 ----")]
    [SerializeField] private float cellPadding = 1.0f; // 每个点位之间的间隔
    private float cellWidth; // 点位的宽
    private float cellHeight; // 点位的高
    private float offsetX; // 将地图挪到世界中心的X偏移量
    private float offsetY; // 将地图挪到世界中心的Y偏移量

    [Header("---- 各类父节点 ----")]
    [SerializeField] private Transform cellHolder;
    [SerializeField] private Transform edgeHolder;
    [SerializeField] private Transform enemiesManager;
    [SerializeField] private Transform playerManager;

    private CellNode[] cellAdjList; // 邻接表
    private string[] cellType = new string[6] { "NullCell", "NormalCell", "PosiCell", "NegaCell", "TargetCell", "GoldenCell" }; // 点位的6种类型
    private Hashtable cellTypeTable; //string -> int 映射表
    public StreamReader mapInfoTxt; // 用于读取地图信息，即Data文件夹中的mapInfo，目前为手动挂载
    public string mapInfoPath = "Assets/Data/level_3.txt"; // 地图信息文件路径
    
    //角色所在点位的编号
    private int[] enemiesPosition; //敌人的位置 (所在点的编号，一个整数表示)
    private int playerPosition; //玩家的位置 (所在点的编号，一个整数表示)

    //public int[,,] cellTypeMap;

    private bool isHightLighting = false;


    private void Awake()
    {
        InitCellTypeTable();
        ReadMapInfo();
        InitCellMap(); // 通过邻接表cellAdjList初始化CellMap
    }

    private void Start()
    {
        UIManager.instance.gameObject.SetActive(true);
        UIManager.instance.ClearAllEnemyHealthBar();
        PropertyManager.instance.RemoveAllPlayerProperty();
        AudioPlayer.instance.gameObject.SetActive(true);
        AudioPlayer.instance.ReplayBgm();
        GameManager.instance.SetCurrentMap(this);
        GenerateMap();
        GenerateEdge();
        GeneratePlayer();
        GenerateEnemies();
        //cellTypeMap = GetCellTypeMap();
        //test();
    }

    // 初始化点位类型映射表
    private void InitCellTypeTable()
    {
        cellTypeTable = new Hashtable();
        for (int i = 0; i < cellType.Length; i++)
        {
           cellTypeTable.Add(cellType[i], i); 
        }
    }

    // 读取地图信息
    private void ReadMapInfo()
    {
        
        //mapInfoList = new List<List<string>>(); // 初始化地图信息列表
        //mapInfoTxt = new StreamReader(mapInfoPath); // 读取文件
        /*string text = Resources.Load<TextAsset>(LevelManager.currentLevelPath).text;
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(text);
        writer.Flush();
        stream.Position = 0;
        mapInfoTxt = new StreamReader(stream); //new StreamReader(LevelManager.currentLevelPath);
        */
        Debug.Log(System.Environment.CurrentDirectory);
        mapInfoTxt = new StreamReader(System.Environment.CurrentDirectory + "\\" + LevelManager.currentLevelPath);
        string inputType = ""; //正在录入的数据类型：Map、Character
        int lineCounter = 0;

        while (!mapInfoTxt.EndOfStream) // 如果没有到文件尾
        {
            List<string> lineInfo = new List<string>(mapInfoTxt.ReadLine().Split(',')); // 将一行数据分隔
            
            if (lineInfo[0] == "Map") // 准备读取Map信息
            {
                inputType = "Map";
                lineCounter = 0;
                Debug.Log("Map");
                continue;
            }
            else if (lineInfo[0] == "Character") // 准备读取角色信息
            {
                inputType = "Character";
                lineCounter = 0;
                Debug.Log("Character");
                continue;
            }

            if (inputType == "Map") // 数据中的第一行记录地图的行和列
            {
                if (lineCounter == 0)
                {
                    //初始化cellMap
                    cellMapRow = int.Parse(lineInfo[0]);
                    cellMapColumn = int.Parse(lineInfo[1]);

                    cellAdjList = new CellNode[cellMapRow * cellMapColumn];
                }
                else // 第二行后的数据记录点位的类型和邻接的点位；第一个为点位类型，剩下的为与该点位邻接的点位在邻接表的下标
                {

                    for (int j = 0; j < lineInfo.Count; j++)
                    {
                        if (j == 0) // 读取点位类型cellType
                        {
                            GameObject spawnedCell = null;
                            // 根据点位类型生成点位
                            if (lineInfo[j] == "NullCell")
                            {
                                spawnedCell = Instantiate(nullCellPrefab, transform.position, Quaternion.identity);
                                spawnedCell.GetComponent<Cell>().SetCellType("NullCell");
                            }
                            else if (lineInfo[j] == "NormalCell")
                            {
                                spawnedCell = Instantiate(normalCellPrefab, transform.position, Quaternion.identity);
                                spawnedCell.GetComponent<Cell>().SetCellType("NormalCell");
                            }
                            else if (lineInfo[j] == "PosiCell")
                            {
                                spawnedCell = Instantiate(posiCellPrefab, transform.position, Quaternion.identity);
                                spawnedCell.GetComponent<Cell>().SetCellType("PosiCell");
                            }
                            else if (lineInfo[j] == "NegaCell")
                            {
                                spawnedCell = Instantiate(negaCellPrefab, transform.position, Quaternion.identity);
                                spawnedCell.GetComponent<Cell>().SetCellType("NegaCell");
                            }
                            else if (lineInfo[j] == "TargetCell")
                            {
                                spawnedCell = Instantiate(targetCellPrefab, transform.position, Quaternion.identity);
                                spawnedCell.GetComponent<Cell>().SetCellType("TargetCell");
                            }
                            else if (lineInfo[j] == "GoldenCell")
                            {
                                spawnedCell = Instantiate(goldenCellPrefab, transform.position, Quaternion.identity);
                                spawnedCell.GetComponent<Cell>().SetCellType("GoldenCell");
                            }

                            cellAdjList[lineCounter - 1] = new CellNode();
                            cellAdjList[lineCounter - 1].cell = spawnedCell;
                            cellAdjList[lineCounter - 1].firstEdge = null;

                            // 处理非空点位列表
                            if (lineInfo[j] != "NullCell")
                            {
                                allCells.Add(spawnedCell.GetComponent<Cell>());
                            }
                        }
                        else
                        {
                            EdgeNode newEdge = new EdgeNode();
                            newEdge.adjCell = int.Parse(lineInfo[j]);
                            newEdge.nextEdge = cellAdjList[lineCounter - 1].firstEdge;
                            cellAdjList[lineCounter - 1].firstEdge = newEdge;
                        }
                    }
                }
                lineCounter++;
                //Debug.Log(lineCounter);
            }
            else if (inputType == "Character")
            {
                //接下来第一行是主角所在的点位，第二行都是敌人的点位
                if (lineCounter == 0)
                {
                    playerPosition = int.Parse(lineInfo[0]);
                }
                else
                {
                    enemiesPosition = new int[lineInfo.Count];
                    for (int k = 0; k < lineInfo.Count; k++)
                    {
                        enemiesPosition[k] = int.Parse(lineInfo[k]);
                    }
                }
                
                lineCounter++;
            }
        }

    }


    // 通过邻接表cellAdjList初始化CellMap
    private void InitCellMap()
    {
        cellMap = new GameObject[cellMapRow, cellMapColumn];

        for (int i = 0; i < cellMapRow; i++)
        {
            for (int j = 0; j < cellMapColumn; j++)
            {
                cellMap[i, j] = cellAdjList[i * cellMapColumn + j].cell;
                // Debug.Log(cellMap[i, j]);
            }
        }        
    }


    // 调整点位生成的位置，坐标原点在左上角
    public Vector2 AdjustPosition(Vector2Int pos)
    {
        return AdjustPosition(pos.x, pos.y);
    }
    
    public Vector2 AdjustPosition(int i, int j)
    {
        float positionX = 0; // 点位最终渲染的位置x分量
        float positionY = 0; // 点位最终渲染的位置y分量
        if (i != 0 && j != 0)
        {
            positionX = j + j * cellPadding;
            positionY = -(i + i * cellPadding);
        }
        else if (i == 0 && j != 0)
        {
            positionX = j + j * cellPadding;
            positionY = i;
        }
        else if (i != 0 && j == 0)
        {
            positionX = j;
            positionY = -(i + i * cellPadding);
        }
        else
        {
            positionX = j;
            positionY = i;
        }
        return new Vector2(positionX - offsetX / 2.0f, positionY + offsetY / 2.0f);
    }

    // 生成地图
    private void GenerateMap()
    {
        // 地图渲染参数初始化
        cellWidth = normalCellPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        cellHeight = normalCellPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        offsetX = (cellMapColumn - 1) * (cellWidth + cellPadding);
        offsetY = (cellMapRow - 1) * (cellHeight + cellPadding);

        List<Vector2Int> targets = new List<Vector2Int>();

        for (int i = 0; i < cellMapRow; i++)
        {
            for (int j = 0; j < cellMapColumn; j++)
            {
                Vector2 position = AdjustPosition(i, j);

                // 以下对地图上的每一个点位的属性进行初始化
                cellMap[i, j].transform.position = position;
                cellMap[i, j].transform.SetParent(cellHolder); // 将生成的点位作为Map的子物体
                cellMap[i, j].name = $"cell {i} {j}";
                if (cellMap[i, j].GetComponent<Cell>()) // 如果生成的点位有Cell这个组件，即生成的点位是有效点位
                {
                    Cell cell = cellMap[i, j].GetComponent<Cell>(); // 获取到新生成有效点位的Cell脚本组件
                    cell.SetPosition(new Vector2Int(i, j));

                    //如果为目标点位，则向GM提交
                    if (cell.GetCellType() == "TargetCell")
                    {
                        targets.Add(new Vector2Int(i, j));
                    }
                    
                }
            }
        }

        //向GM提交目标点位的List
        GameManager.instance.PostTargetPositions(targets);
    }


    //生成玩家
    private void GeneratePlayer()
    {
        int y = (playerPosition / cellMapColumn);
        int x = (playerPosition % cellMapColumn);

        Vector2 pos = AdjustPosition(y, x);
        GameObject spawnPlayer = Instantiate(player, pos, Quaternion.identity);
        spawnPlayer.GetComponent<SpriteRenderer>().sortingOrder = 1;
        spawnPlayer.transform.SetParent(playerManager);
        spawnPlayer.name = "player";
        Player p = spawnPlayer.GetComponent<Player>();
        p.Init(this, new Vector2Int(y, x));
    }

    //生成敌人
    private void GenerateEnemies()
    {
        if (enemiesPosition == null) return;

        for (int i = 0; i < enemiesPosition.Length; i++){
            int y = (enemiesPosition[i] / cellMapColumn);
            int x = (enemiesPosition[i] % cellMapColumn);

            Vector2 pos = AdjustPosition(y, x);
            GameObject spawnEnemy = Instantiate(enemy, pos, Quaternion.identity);
            spawnEnemy.GetComponent<SpriteRenderer>().sortingOrder = 1;
            spawnEnemy.transform.SetParent(enemiesManager);
            spawnEnemy.name = $"enemy {i + 1}";
            Enemy p = spawnEnemy.GetComponent<Enemy>();
            p.Init(this, new Vector2Int(y, x));
        }
    }

    //根据坐标获取对应的Cell
    public Cell GetCellByIndex(Vector2Int index)
    {
        if (index.x < 0 || index.y < 0 || index.x >= cellMapRow || index.y >= cellMapColumn)
        {
            Debug.Log("点位坐标超出范围");
            return null;
        }
        return cellMap[index.x, index.y].GetComponent<Cell>();
    }

    // 生成整个地图点位的边
    private void GenerateEdge()
    {
        // 遍历cellMap
        for (int i = 0; i < cellMapRow; i++)
        {
            for (int j = 0; j < cellMapColumn; j++)
            {
                if (cellMap[i, j].GetComponent<Cell>()) // 如果cellMap[i, j]存储的是有效点位
                {
                    List<Cell> adjCellList = new List<Cell>(); // 该点位的邻接点数组
                    GameObject startCell = cellMap[i, j]; // 该点位，作为边的出发位置

                    EdgeNode pointer = cellAdjList[i * cellMapColumn + j].firstEdge; // 用于遍历
                    while (pointer != null)
                    {
                        int adjCellIndex = pointer.adjCell;
                        GameObject endCell = cellMap[adjCellIndex / cellMapColumn, adjCellIndex % cellMapColumn]; // 邻接点位，作为边的终点位置

                        var edge = Instantiate(edgePrefab, (startCell.transform.position + endCell.transform.position) / 2.0f, Quaternion.identity);
                        edge.name = $"edge {i * cellMapColumn + j} {adjCellIndex}";
                        edge.transform.SetParent(edgeHolder);

                        edge.GetComponent<Edge>().DrawLine(startCell.transform, endCell.transform); // 调用边的DrawLine函数使用LineRenderer画出一条线
                        adjCellList.Add(endCell.GetComponent<Cell>()); // 将邻接点位加入邻接点数组

                        pointer = pointer.nextEdge;
                    }
                    cellMap[i, j].GetComponent<Cell>().SetAdjCellList(adjCellList); // 设置该点位的邻接点数组
                }
            }

        }
    }

    //获取网格形状, (x=列，y=行)
    public Vector2Int GetMapShape()
    {
        return new Vector2Int(cellMapColumn, cellMapRow);
    }

    //设置可达点位高亮
    public void SetHightLightAvailablePoint(bool hightLight, List<Cell> availableAdjCellList)
    {
        if (isHightLighting == hightLight)
            return;
        isHightLighting = hightLight;

        foreach (Cell cell in availableAdjCellList)
        {
            cell.SetHightLight(hightLight);
        }
    }

    public List<Cell> GetAllCells()
    {
        return allCells;
    }
    /*
    //返回一个简易三维数组作为地图, 两层二维，一层存点位，一层存角色
    public int[,,] GetCellTypeMap()
    {
        int[,,] result = new int[2, cellMapRow, cellMapColumn];
        for (int i = 0; i < cellMapRow; i++)
        {
            for (int j = 0 ; j < cellMapColumn; j++)
            {
                if (cellMap[i, j])
                {
                    result[0, i, j] = (int)cellTypeTable[cellMap[i, j].GetComponent<Cell>().GetCellType()];
                }
            }
        }
        return result;
    }

    // test
    private void test()
    {
        for (int i = 0; i < cellMapRow; i++)
        {
            for (int j = 0; j < cellMapColumn; j++)
            {
                if (cellMap[i, j].GetComponent<Cell>())
                {
                    Cell cell = cellMap[i, j].GetComponent<Cell>();
                    for (int k = 0; k < cell.GetAdjCellList().Count; k++)
                    {
                        Debug.Log(cell.GetAdjCellList()[k].name);
                    }

                    Debug.Log(cellMap[i, j].GetComponent<Cell>().GetCellType());
                    Debug.Log(cellMap[i, j].GetComponent<Cell>().GetIndex());
                }
            }
        }
    }*/

}

// 顶点表
public class CellNode
{
    public GameObject cell;
    public EdgeNode firstEdge;
}

// 边表
public class EdgeNode
{
    public int adjCell; // 邻接的点位在顶点表的下标
    public EdgeNode nextEdge;
}