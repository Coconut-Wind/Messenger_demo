using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//目标点位
public class TargetCell : Cell
{
    // Start is called before the first frame update
    public bool isReached = false;
    public GameObject castle;

    private void Start() {
        GameObject c = Instantiate(castle, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
        c.transform.SetParent(transform);
        c.GetComponent<SpriteRenderer>().sortingOrder = 3;
    }
}
