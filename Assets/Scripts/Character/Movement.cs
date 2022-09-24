using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed= 0.5f;

    protected Vector2Int mapShape = Vector2Int.zero;
    protected int x, y, index;
    protected Cell onCell;
    protected Vector2 targetPos;

    public virtual void Init(Map map, Vector2Int index)
    {
        SetPosition(index.x, index.y);
        mapShape = GameManager.instance.GetCurrentMap().GetMapShape();
    }


    public virtual Vector2Int GetPosition()
    {
        return new Vector2Int(x, y);
    }

    public virtual void SetPosition(int _x, int _y)
    {
        x = _x;
        y = _y;
        index = x + y * mapShape.x;
        onCell = GameManager.instance.GetCurrentMap().GetCellByIndex(new Vector2Int(x, y));
    }


    public virtual void WalkTo(Vector2Int to)
    {
        Debug.Log("Walk to " + to);
        targetPos = GameManager.instance.GetCurrentMap().AdjustPosition(to);
        SetPosition(to.x, to.y);
        StartCoroutine(Walk());
    }

    protected IEnumerator Walk()
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
