using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Map : MonoBehaviour
{
    [Header("---- Prefabs ----")]
    [SerializeField] private GameObject nullCellPrefab; // 空点位的预制体
    [SerializeField] private GameObject normalCellPrefab; // 普通点位的预制体
    [SerializeField] private GameObject posiCellPrefab; // 增益点位的预制体
    [SerializeField] private GameObject negaCellPrefab; // 减益点位的预制体
    [SerializeField] private GameObject edgePrefab; // 边的预制体


    [Header("---- cellMap参数 ----")]
    private GameObject[,] cellMap; // 整张点位地图，二维数组
    [SerializeField] private int cellMapRow;
    [SerializeField] private int cellMapColumn;


    [Header("---- 渲染地图的参数 ----")]
    [SerializeField] private float cellPadding = 1.0f; // 每个点位之间的间隔
    private float cellWidth; // 点位的宽
    private float cellHeight; // 点位的高
    private float offsetX; // 将地图挪到世界中心的X偏移量
    private float offsetY; // 将地图挪到世界中心的Y偏移量


    private CellNode[] cellAdjList; // 邻接表
    private string[] cellType = new string[4] { "NullCell", "NormalCell", "PosiCell", "NegaCell" }; // 点位的四种类型
    
    public StreamReader mapInfoTxt; // 用于读取地图信息，即Data文件夹中的mapInfo，目前为手动挂载
    public string mapInfoPath = "Assets/Data/mapInfo.txt"; // 地图信息文件路径
    public List<List<string>> mapInfoList; // 地图信息二维列表


    private void Awake()
    {
        ReadMapInfo(); // 读取地图信息
        InitCellMapSize(); // 初始化cellMap的大小
        InitAdjCellList(); // 通过地图信息初始化邻接表
        InitCellMap(); // 通过邻接表cellAdjList初始化CellMap
    }

    private void Start()
    {
        GenerateMap(); // 生成地图
        GenerateEdge(); // 生成边
    }

    // 读取地图信息
    private void ReadMapInfo()
    {
        mapInfoList = new List<List<string>>(); // 初始化地图信息列表
        mapInfoTxt = new StreamReader(mapInfoPath); // 读取文件
        while (!mapInfoTxt.EndOfStream) // 如果没有到文件尾
        {
            List<string> lineInfo = new List<string>(mapInfoTxt.ReadLine().Split(',')); // 将一行数据分隔
            mapInfoList.Add(lineInfo);
        }
        
        // test
        // for(int i = 0; i<mapInfoList.Count; i++) 
        // {
        //     for(int j = 0; j<mapInfoList[i].Count; j++)
        //     {
        //         Debug.Log(mapInfoList[i][j]);
        //     }
        // }
    }

    // 初始化cellMap的大小
    private void InitCellMapSize()
    {
        // string to int
        cellMapRow = int.Parse(mapInfoList[0][0]);
        cellMapColumn = int.Parse(mapInfoList[0][1]);
    }

    // 通过地图信息初始化邻接表
    private void InitAdjCellList()
    {
        cellAdjList = new CellNode[cellMapRow * cellMapColumn];

        for (int i = 1; i < mapInfoList.Count; i++) // 从下标1开始，下标0是cellMap的大小
        {
            for (int j = 0; j < mapInfoList[i].Count; j++)
            {
                if (j == 0)
                {
                    GameObject spawnedCell = null;
                    // 根据点位类型生成点位
                    if (mapInfoList[i][j] == "NullCell")
                    {
                        spawnedCell = Instantiate(nullCellPrefab, transform.position, Quaternion.identity);
                    }
                    else if (mapInfoList[i][j] == "NormalCell")
                    {
                        spawnedCell = Instantiate(normalCellPrefab, transform.position, Quaternion.identity);
                        spawnedCell.GetComponent<Cell>().SetCellType("NormalCell");
                    }
                    else if (mapInfoList[i][j] == "PosiCell")
                    {
                        spawnedCell = Instantiate(posiCellPrefab, transform.position, Quaternion.identity);
                        spawnedCell.GetComponent<Cell>().SetCellType("PosiCell");
                    }
                    else if (mapInfoList[i][j] == "NegaCell")
                    {
                        spawnedCell = Instantiate(negaCellPrefab, transform.position, Quaternion.identity);
                        spawnedCell.GetComponent<Cell>().SetCellType("NegaCell");
                    }

                    cellAdjList[i - 1] = new CellNode();
                    cellAdjList[i - 1].cell = spawnedCell;
                    cellAdjList[i - 1].firstEdge = null;
                }
                else
                {
                    EdgeNode newEdge = new EdgeNode();
                    newEdge.adjCell = int.Parse(mapInfoList[i][j]);
                    newEdge.nextEdge = cellAdjList[i - 1].firstEdge;
                    cellAdjList[i - 1].firstEdge = newEdge;
                }
            }
        }

        // test
        // for (int i = 0; i < cellMapRow * cellMapColumn; i++)
        // {
        //     Debug.Log(cellAdjList[i].cell);
        //     EdgeNode pointer = cellAdjList[i].firstEdge;
        //     while (pointer != null)
        //     {
        //         Debug.Log(pointer.adjCell);
        //         pointer = pointer.nextEdge;
        //     }
        // }
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

    // 通过cellMap生成地图
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
                // 调整点位生成的位置，此时坐标原点在左上角
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

                // 以下对地图上的每一个点位的属性进行初始化
                cellMap[i, j].transform.position = new Vector2(positionX - offsetX / 2.0f, positionY + offsetY / 2.0f);
                cellMap[i, j].transform.SetParent(transform); // 将生成的点位作为Map的子物体
                cellMap[i, j].name = $"cell {i} {j}";
                if (cellMap[i, j].GetComponent<Cell>()) // 如果生成的点位有Cell这个组件，即生成的点位是有效点位
                {
                    Cell cell = cellMap[i, j].GetComponent<Cell>(); // 获取到新生成有效点位的Cell脚本组件
                    cell.SetIndex(i, j);
                }
            }
        }
    }

    // 根据邻接表生成整个地图点位的边
    private void GenerateEdge()
    {
        // 遍历邻接表
        for (int i = 0; i < cellMapRow * cellMapColumn; i++)
        {
            if (cellAdjList[i].cell.GetComponent<Cell>())
            {
                GameObject startCell = cellAdjList[i].cell; // 边的起点

                EdgeNode pointer = cellAdjList[i].firstEdge; // 遍历边表用
                while (pointer != null) // 遍历边表
                {
                    int adjCellIndex = pointer.adjCell; // 邻接点位的下标

                    GameObject endCell = cellAdjList[adjCellIndex].cell; // 边的终点

                    // 对生成的边进行设置
                    var edge = Instantiate(edgePrefab, (startCell.transform.position + endCell.transform.position) / 2.0f, Quaternion.identity);
                    edge.name = $"edge {i} {adjCellIndex}";
                    edge.transform.SetParent(transform);

                    edge.GetComponent<Edge>().DrawLine(startCell.transform, endCell.transform); // 调用边的DrawLine函数使用LineRenderer画出一条线

                    pointer = pointer.nextEdge;
                }
            }
        }
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