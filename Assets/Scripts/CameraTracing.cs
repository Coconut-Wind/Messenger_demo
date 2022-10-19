using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracing : MonoBehaviour
{/*
    private float originFieldOfView = 0f;
    private float originOrthographicSize = 0f;

    private bool isTracing = false;

    private Vector3 p1;

    void Start()
    {
        //记录初始值
        originFieldOfView = Camera.main.fieldOfView;
        originOrthographicSize = Camera.main.orthographicSize;
    }

    void Update()
    {
        //如果玩家正在移动中，跟随
        if (GameManager.instance.player.isMoving)
        {
            if (!isTracing)
            {
                isTracing = true;
                //记录的位置
                p1 = Camera.main.transform.position;
                Camera.main.orthographicSize = 2f;
            }
        }
        else
        {
            if (isTracing)
            {
                isTracing = false;
                //回到记录的位置
                Camera.main.transform.position = p1;
                Camera.main.orthographicSize = originOrthographicSize;
            }
        }


        if (!isTracing)
        {
            //否则玩家可以自由拖动镜头
            //暂时顶替 滚轮控制镜头缩放

            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel < 0)
            {
                if (Camera.main.fieldOfView <= 0)
                    Camera.main.fieldOfView += 2;
                if (Camera.main.orthographicSize <= 10)
                    Camera.main.orthographicSize += 0.5f;
            }
            else if (wheel > 0)
            {
                if (Camera.main.fieldOfView > 2)
                    Camera.main.fieldOfView -= 2;
                if (Camera.main.orthographicSize >= 1)
                    Camera.main.orthographicSize -= 0.5f;
            }
        }
        else
        {
            Vector3 pos = GameManager.instance.player.transform.position;
            transform.position = new Vector3(
                pos.x, pos.y, -100
            );
        }
    }*/
}
