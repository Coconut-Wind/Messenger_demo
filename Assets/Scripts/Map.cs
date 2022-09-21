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
    // TODO:还有增益点位和减益点位的预制体没创建
    [SerializeField] private GameObject edgePrefab; // 边的预制体
    [SerializeField] private GameObject player; //玩家的预制体
    [SerializeField] private GameObject enemy; //敌人的预制体


    private GameObject[,] cellMap; // 整张点位地图，二维数组
    
    [Header("---- cellMap参数 ----")]
    [SerializeField] private int cellMapRow;
    [SerializeField] private int cellMapColumn;


    [Header("---- 渲染地图的参数 ----")]
    [SerializeField] private float cellPadding = 1.0f; // 每个点位之间的间隔
    private float cellWidth; // 点位的宽
    private float cellHeight; // 点位的高
    private float offsetX; // 将地图挪到世界中心的X偏移量
    private float offsetY; // 将地图挪到世界中心的Y偏移量

    [Header("---- 点边的父节点 ----")]
    [SerializeField] private Transform cellHolder;
    [SerializeField] private Transform edgeHolder;
    [SerializeField] private Transform enemiesManager;

    private CellNode[] cellAdjList; // 邻接表
    private string[] cellType = new string[4] { "NullCell", "NormalCell", "PosiCell", "NegaCell" }; // 点位的四种类型
    private Hashtable cellTypeTable; //string -> int 映射表
    public StreamReader mapInfoTxt; // 用于读取地图信息，即Data文件夹中的mapInfo，目前为手动挂载
    public string mapInfoPath = "Assets/Data/mapInfo.txt"; // 地图信息文件路径
    
    private int[] enemiesPosition;
    private int playerPosition;

    public int[,,] cellTypeMap;


    private void Awake()
    {
        InitCellTypeTable();
        ReadMapInfo();
        InitCellMap(); // 通过邻接表cellAdjList初始化CellMap
    }

    private void Start()
    {
        
        GenerateMap();
        GenerateEdge();
        GeneratePlayer();
        GenerateEnemies();

        cellTypeMap = GetCellTypeMap();

        //test();
    }

    private void InitCellTypeTable()
    {
        cellTypeTable = new Hashtable();
        for (int i = 0; i < 4; i++)
        {
           cellTypeTable.Add(cellType[i], i); 
        }
    }

    // 读取地图信息
    private void ReadMapInfo()
    {
        //mapInfoList = new List<List<string>>(); // 初始化地图信息列表
        mapInfoTxt = new StreamReader(mapInfoPath); // 读取文件

        string inputType = ""; //正在录入的数据类型：Map、Character
        int lineCounter = 0;

        while (!mapInfoTxt.EndOfStream) // 如果没有到文件尾
        {
            List<string> lineInfo = new List<string>(mapInfoTxt.ReadLine().Split(',')); // 将一行数据分隔
            
            if (lineInfo[0] == "Map")
            {
                inputType = "Map";
                lineCounter = 0;
                Debug.Log("Map");
                continue;
            }
            else if (lineInfo[0] == "Character")
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
                    for (int j = 0; j < lineInfo.Count; j++) // 如果点位类型是空点位
                    {
                        if (j == 0)
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

                            cellAdjList[lineCounter - 1] = new CellNode();
                            cellAdjList[lineCounter - 1].cell = spawnedCell;
                            cellAdjList[lineCounter - 1].firstEdge = null;
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
    public Vector2 AdjustPosition(int x, int y)
    {
        float i = y;
        float j = x;
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
        for (int i = 0; i < cellMapRow; i++)
        {
            for (int j = 0; j < cellMapColumn; j++)
            {
                Vector2 position = AdjustPosition(j, i);

                // 以下对地图上的每一个点位的属性进行初始化
                cellMap[i, j].transform.position = position;
                cellMap[i, j].transform.SetParent(cellHolder); // 将生成的点位作为Map的子物体
                cellMap[i, j].name = $"cell {i} {j}";
                if (cellMap[i, j].GetComponent<Cell>()) // 如果生成的点位有Cell这个组件，即生成的点位是有效点位
                {
                    Cell cell = cellMap[i, j].GetComponent<Cell>(); // 获取到新生成有效点位的Cell脚本组件
                    cell.SetIndex(i, j);
                }
            }
        }
    }

    //生成玩家
    private void GeneratePlayer()
    {
        
        int y = (playerPosition / cellMapColumn);
        int x = (playerPosition % cellMapColumn);
        //Debug.Log(x + ", " + y);
        Vector2 pos = AdjustPosition(x, y);
        GameObject spawnPlayer = Instantiate(player, pos, Quaternion.identity);
        spawnPlayer.GetComponent<SpriteRenderer>().sortingOrder = 1;
        spawnPlayer.transform.SetParent(transform);
        spawnPlayer.name = "player";
        Player p = spawnPlayer.GetComponent<Player>();
        p.setMap(this);
        p.SetIndex(y, x);
    }

    //生成敌人
    private void GenerateEnemies()
    {
        for (int i = 0; i < enemiesPosition.Length; i++){
            int y = (enemiesPosition[i] / cellMapColumn);
            int x = (enemiesPosition[i] % cellMapColumn);
            Debug.Log(x + ", " + y + ", " + enemiesPosition[i]);
            Vector2 pos = AdjustPosition(x, y);
            GameObject spawnEnemy = Instantiate(enemy, pos, Quaternion.identity);
            spawnEnemy.GetComponent<SpriteRenderer>().sortingOrder = 1;
            spawnEnemy.transform.SetParent(enemiesManager);
            spawnEnemy.name = $"enemy {i + 1}";
            Enemy p = spawnEnemy.GetComponent<Enemy>();
            p.setMap(this);
            p.SetIndex(y, x);
        }
        
    }

    //根据坐标获取对应的Cell
    public Cell GetCellByIndex(Vector2Int index)
    {
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
    }

    //获取网格形状, (列，行)
    public Vector2Int GetMapShape()
    {
        return new Vector2Int(cellMapColumn, cellMapRow);
    }

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