using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private string cellType; // NullCell、NormalCell、PosiCell、NegaCell
    
    // cellMap[x, y]
    private int x;
    private int y;

    private List<Cell> adjCellList; // 邻接点位数组，存储的是点位的实例

    // Setter cellMap中的下标
    public void SetIndex(int _x, int _y) {
        x = _x;
        y = _y;
    }

    // Getter cellMap中的下标
    public Vector2 GetIndex()
    {
        return new Vector2(x, y);
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

    // // Setter 邻接点位数组
    // public void SetAdjCellList(List<Cell> _adjCellList)
    // {
    //     adjCellList = new List<Cell>(_adjCellList);
    // }

    // // Getter 邻接点位数组，现在可以用邻接表拿到临界点位了
    // public List<Cell> GetAdjCellList()
    // {
    //     return adjCellList;
    // }

}
