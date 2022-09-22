using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    Transform[] childs;
    public bool hasChilds = false;
    void Start()
    {
        
    }

    public void setFacing()
    {
        childs = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i++){
            childs[i] = transform.GetChild(i);
        }
        Debug.Log(childs.Length);
        hasChilds = true;
    }

    void Update()
    {
        if(!hasChilds)
        {
            return;
        }
        for(int i = 0; i < transform.childCount; i++){
            childs[i].rotation = Camera.main.transform.rotation;
        }
    }
}
