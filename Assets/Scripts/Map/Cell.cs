using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public GameObject Ring;
    protected string cellType; // NullCell、NormalCell、PosiCell、NegaCell
    
    // cellMap[x, y]
    protected int x;
    protected int y;

    protected List<Cell> adjCellList; // 邻接点位数组，存储的是点位的实例
    protected bool isHightLighting = false;

    // Setter cellMap中的下标
    public void SetIndex(int _x, int _y) {
        x = _x;
        y = _y;
    }

    // Getter cellMap中的下标
    public Vector2Int GetIndex()
    {
        return new Vector2Int(x, y);
    }

    // Setter 点位类型
    public void SetCellType(string _cellType)
    {
        cellType = _cellType;
    }

    // Getter 点位类型
    public string GetCellType()
    {
        return cellType;
    }

    // Setter 邻接点位数组
    public void SetAdjCellList(List<Cell> _adjCellList)
    {
        adjCellList = new List<Cell>(_adjCellList);
    }

    // Getter 邻接点位数组
    public List<Cell> GetAdjCellList()
    {
        return adjCellList;
    }

    //通过设置光圈是否可见来实现高亮
    public void SetHightLight(bool hightLight)
    {
        isHightLighting = hightLight;
        if (isHightLighting)
        {
            Ring.SetActive(true);
        }
        else
        {
            Ring.SetActive(false);
        }
    }

    public bool IsHightLighting()
    {
        return isHightLighting;
    }

}
