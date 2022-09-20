using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    private LineRenderer lineRenderer; // 用于画线

    [Header("---- Edge的基本参数 ----")]
    [SerializeField] private Color color;
    [SerializeField] private float width;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // 通过LineRenderer画线
    public void DrawLine(Transform startPoint, Transform endPoint) 
    {
        // 设置颜色
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        // 设置宽度
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        // 设置线的起始点
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }
}
