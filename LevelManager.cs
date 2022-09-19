using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //定义一串常量
    private const int NONE = 0;     //表示不能走的地方
    private const int POINT = 1;    //表示可以走的地方
    private const int PLAYER = 2;
    private const int ENEMY = 3;
    private const int BUILDING = 4;
    private const int DESTINATION = 5;

    private const int WEST = 0;
    private const int NORTH_WEST = 1;
    private const int NORTH = 2;
    private const int NORTH_EAST = 3;
    private const int EAST = 4;
    private const int SOUTH_EAST = 5;
    private const int SOUTH = 6;
    private const int SOUTH_WEST = 7;
    

    private int[,] map = new int[25, 25];     //游戏地图
    private byte[,] lineMatrix = new byte[25, 25];      //线路的存储矩阵

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddElement(string element,int x, int y)
    {
        if (x >= 0 && x <= 25 && y >=0 && y <= 25)
        {
            switch (element)
            {
                case "Player":
                    map[x, y] = PLAYER;
                    break;
                case "Enemy":
                    map[x, y] = ENEMY;
                    break;
                case "None":
                    map[x, y] = NONE;
                    break;
                case "Building":
                    map[x, y] = BUILDING;
                    break;
                case "Point":
                    map[x, y] = POINT;
                    break;
                default:
                    break;
            }
        }
    }

    public void AddLine(int x, int y, int direction)
    {
        if (x >= 0 && x <= 25 && y >= 0 && y <= 25 && direction >=0 && direction <= 7)
        {
            lineMatrix[x, y] = UtilChangeBit(direction, 1, lineMatrix[x, y]);
        }
    }

    //这个是通用的包括玩家和敌人，其中x和y是代表起始点的坐标，负责获得原在这点的数据
    //dx和dy负责计算向哪移动取值是从-1到1
    public void MoveCharacter(int x, int y, int dx, int dy)
    {
        int character = map[x, y];  //先获取是谁
        if (character != PLAYER && character != ENEMY)  //若所指单位不是玩家或敌人，则不能移动
        {
            return;
        }
        if (map[x + dx, y + dy] == POINT || map[x + dx, y + dy] == DESTINATION)      //若可以走
        {
            map[x + dx, y + dy] = character;
            map[x, y] = POINT;
        }
    }

    public void CreateMap()
    {
        AddElement("Point", 1, 24);
        AddElement("Point", 1, 23);
    }


    //测试用的函数，通过控制台输出看地图生成,后面所有的工具方法都在前面加个util
    private void UtilCheckMap()
    {
        string str = null;
        string newline = null;
        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                newline += (map[i, j] + " ");
            }
            str += newline;
            str += ("\n");
            newline = null;
        }
        Debug.Log(str);
    }

    private byte UtilChangeBit(int index, byte state, byte target)      //index是指第几位，state指改变后状态，target指目标
    {
        if (index >= 0 && index <= 7)
        {
            if (state == 0)
            {
                target &= (byte)~(1 << index);
            }
            else if (state == 1)
            {
                target |= (byte)~(1 << index);
            }
        }
        
        return target;
    }

    private bool UtilIsBitOn(int index, byte target)
    {
        if (index >= 0 && index <= 7)
        {
            return ((target & (1 << index)) == 0);
        }
        else
        {
            return false;
        }
    }
}