using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 3;
    public float speed= 0.5f;
    public float chooseMaxDistance = 1.5f;
    [SerializeField]private GameObject arrow;
    [SerializeField]private Transform arrowHolder;
    [SerializeField]private GameObject circle;
    
    
    private bool isUnderMouse = false;
    private Map mapObject = null;
    private Vector2Int mapShape= Vector2Int.zero;
    private int x, y, index;
    private Cell onCell;
    private Vector2 targetPos;


    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //将屏幕坐标转换为世界坐标
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float dis = Vector2.Distance(mouse, this.transform.position);
            //根据距离判断是否点到Player
            if (dis <= chooseMaxDistance && !isUnderMouse)
            {
                isUnderMouse = true;
                ShowArrow();
            }
            else
            {
                if (isUnderMouse)
                {
                    isUnderMouse = false;
                    HideArrow();
                }
            }
        }
            
    }

    public void setMap(Map map)
    {
        mapObject = map;
        mapShape = mapObject.GetMapShape();
    }

    public Vector2Int GetIndex()
    {
        return new Vector2Int(x, y);
    }

    public void SetIndex(int _x, int _y)
    {
        x = _x;
        y = _y;
        index = x + y * mapShape.x;
        onCell = mapObject.GetCellByIndex(new Vector2Int(x, y));
    }

    //显示圆圈和所有可达箭头
    private void ShowArrow()
    {
        //Debug.Log("show");
        List<Cell> cellAdjustList = onCell.GetAdjCellList();
        circle.SetActive(true);
        for (int i = 0; i < cellAdjustList.Count; i++)
        {
            //根据相对位置显示相应箭头
            GameObject a = Instantiate(arrow, this.transform.position, Quaternion.identity);
            a.transform.SetParent(arrowHolder);
            a.GetComponent<PlayerMoveArrow>().init(this, cellAdjustList[i].GetIndex());
            
            //根据目标点位旋转箭头
            Vector2 e = (Vector2)(cellAdjustList[i].GetIndex() - GetIndex());
            e = new Vector2(e.y, e.x);
            
            e = e.normalized;
            float angle = Vector2.Angle(Vector2.right, e);
            if (e.y > 0)
            {
                angle = -angle;
            }
            Debug.Log(cellAdjustList[i].GetIndex() +", "+ GetIndex()+", "+e+", "+angle);
            a.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    //删除所有箭头，隐藏圆圈
    private void HideArrow()
    {
        circle.SetActive(false);
        for (int i = 0; i < arrowHolder.childCount; i++)
        {
            Destroy(arrowHolder.GetChild(i).gameObject, 0f);
        }
    }

    public void WalkTo(Vector2Int to)
    {
        Debug.Log("Walk to " + to);
        
        targetPos = mapObject.AdjustPosition(to.y, to.x);
        HideArrow();
        SetIndex(to.x, to.y);
        StartCoroutine(Walk());
    }

    IEnumerator Walk()
    {
        Vector2 substract = targetPos - (Vector2)transform.position;
        Vector2 targetPosDir = (substract).normalized;
        float dis = Vector2.Distance(transform.position, targetPos);
        float num = dis / speed;
        Vector2 per = substract / num;
        Debug.Log(dis + ", " + num);
        for (int i = 0; i < num; i++)
        {
            transform.position = transform.position + (Vector3)per;
            yield return new WaitForSeconds(0.05f);
        }
        transform.position = targetPos;
         yield return new WaitForSeconds(0.05f);
    }


}
