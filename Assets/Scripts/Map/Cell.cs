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
    public bool isTriggered = false; //点位效果是否已经触发过

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
    public void SetHightLight(bool hightLight, bool isPlayer=true)
    {
        isHightLighting = hightLight;
        Color mColor = isPlayer ? Color.white: Color.red;
        if (isHightLighting)
        {
            Ring.SetActive(true);
            Ring.GetComponent<SpriteRenderer>().color = mColor;
            Ring.GetComponent<Ring>().SetShining(true);
        }
        else
        {
            Ring.SetActive(false);
            Ring.GetComponent<SpriteRenderer>().color = mColor;
            Ring.GetComponent<Ring>().SetShining(false);
        }
    }

    public bool IsHightLighting()
    {
        return isHightLighting;
    }

    //侦测鼠标点击
    private void OnMouseUp() {
        //如果打开了面板，则不执行
        if (PropertyManager.instance.isOpenedPanel || EventManager.instance.isEventPanelOpen)
            return;

        Debug.Log("CLICK");
        Debug.Log("!Input.GetMouseButtonUp(0)：" + !Input.GetMouseButtonUp(0));
        Debug.Log("GameManager.instance.IsGameOver()：" + GameManager.instance.IsGameOver());
        Debug.Log("GameManager.instance.turnState != GameManager.TurnState.WaitngPlayer：" + (GameManager.instance.turnState != GameManager.TurnState.WaitngPlayer));
        if (!Input.GetMouseButtonUp(0) || GameManager.instance.IsGameOver() || GameManager.instance.turnState != GameManager.TurnState.WaitngPlayer)
            return;
        
        //检测是否可达
        if (GameManager.instance.player.GetHeightLightCellList().Contains(this))
        {
            //发起主角移动
            GameManager.instance.player.WalkTo(GetPosition());
        }
    }

}
