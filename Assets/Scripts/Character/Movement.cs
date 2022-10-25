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
    protected Vector2 currentVelocity; // 提供给smoothDamp

    public virtual void Init(Map map, Vector2Int index)
    {
        SetPosition(index.x, index.y);
        mapShape = GameManager.instance.GetCurrentMap().GetMapShape();
    }


    public virtual Vector2Int GetPosition()
    {
        return new Vector2Int(x, y);
    }

    public Cell GetCell()
    {
        return onCell;
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
        if (to == GetPosition())
        {
            OnReachCell();
            return;
        }
        //Debug.Log("Walk to " + to);
        targetPos = GameManager.instance.GetCurrentMap().AdjustPosition(to);
        SetPosition(to.x, to.y);
        StartCoroutine(Walk(OnReachCell));
    }

    protected virtual int OnReachCell()
    {
        return 0;
    }

    protected IEnumerator Walk(System.Func<int> finish)
    {
        while(Vector2.Distance(transform.position, targetPos) >= 0.05f) 
        {
            transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref currentVelocity, 0.3f);
            yield return null;
        }
        transform.position = targetPos;
        //执行回调函数
        if (finish != null)
            finish();
    }

}
