using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracing : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 pos = GameManager.instance.player.transform.position;
        transform.position = new Vector3(
            pos.x, pos.y, -100
        );
        
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
}
