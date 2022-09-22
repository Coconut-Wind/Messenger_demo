using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed= 0.5f;

    protected Map mapObject = null;
    protected Vector2Int mapShape = Vector2Int.zero;
    protected int x, y, index;
    protected Cell onCell;
    protected Vector2 targetPos;

    public virtual void init(Map map, Vector2Int index)
    {
        setMap(map);
        SetIndex(index.x, index.y);
    }

    public virtual void setMap(Map map)
    {
        mapObject = map;
        mapShape = mapObject.GetMapShape();
    }

    public virtual Vector2Int GetIndex()
    {
        return new Vector2Int(x, y);
    }

    public virtual void SetIndex(int _x, int _y)
    {
        x = _x;
        y = _y;
        index = x + y * mapShape.x;
        onCell = mapObject.GetCellByIndex(new Vector2Int(x, y));
    }


    public virtual void WalkTo(Vector2Int to)
    {
        Debug.Log("Walk to " + to);
        targetPos = mapObject.AdjustPosition(to.y, to.x);
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
        for (int i = 0; i < num-1; i++)
        {
            transform.position = transform.position + (Vector3)per;
            yield return new WaitForSeconds(0.05f);
        }
        transform.position = targetPos;

        
        yield return new WaitForSeconds(0.05f);
    }


}
