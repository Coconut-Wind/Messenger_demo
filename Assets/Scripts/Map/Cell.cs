using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public GameObject Ring;
    protected string cellType; // NullCell、NormalCell、PosiCell、NegaCell
    
    // cellMap[x, y]
    protected Vector2Int position;
    protected List<Cell> adjCellList; // 邻接点位数组，存储的是点位的实例
    protected bool isHightLighting = false;

    // Setter cellMap中的下标
    public void SetPosition(Vector2Int pos) {
        position = pos;
    }

    // Getter cellMap中的下标
    public Vector2Int GetPosition()
    {
        return position;
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
            Ring.GetComponent<Ring>().SetShining(true);
        }
        else
        {
            Ring.SetActive(false);
            Ring.GetComponent<Ring>().SetShining(false);
        }
    }

    public bool IsHightLighting()
    {
        return isHightLighting;
    }

    //侦测鼠标点击
    private void OnMouseUp() {
        Debug.Log("CLICK");
        if (!Input.GetMouseButtonUp(0) || GameManager.instance.IsGameOver())
            return;
        
        //检测是否可达
        if (GameManager.instance.player.GetHeightLightCellList().Contains(this))
        {
            //发起主角移动
            GameManager.instance.player.WalkTo(GetPosition());
        }
    }

}
