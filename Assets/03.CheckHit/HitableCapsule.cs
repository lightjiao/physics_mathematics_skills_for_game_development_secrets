using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitableCapsule : Hitable
{
    public Vector2 Point;
    public Vector2 Vec;
    public float Radius;

    // 标准的capsule都是 r = 0.5 height = 2 center = (0, 0, 0)
    private const float _radius = 0.5f;
    private const float _height = 2f;
    private float m_BoundBoxHalfLength = -1f;

    private void FixedUpdate()
    {
        var scale = transform.localScale.x;

        Radius = _radius * scale;

        // 判断z轴的旋转角度，计算Point与Vec
        Vec = Vector2.up * (_height - _radius * 2) * scale;
        Vec = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * Vec;

        var pos = transform.position;
        Point.x = pos.x - Vec.x / 2;
        Point.y = pos.y - Vec.y / 2;

        UpdateBoundBox();
    }

    private void UpdateBoundBox()
    {
        // 最大值设置为Bound
        if (m_BoundBoxHalfLength < 0)
        {
            m_BoundBoxHalfLength = Vec.magnitude / 2 + Radius;
        }

        var pos = transform.position;

        BoundingBox.Left = pos.x - m_BoundBoxHalfLength;
        BoundingBox.Right = pos.x + m_BoundBoxHalfLength;
        BoundingBox.Top = pos.y + m_BoundBoxHalfLength;
        BoundingBox.Bottom = pos.y - m_BoundBoxHalfLength;
    }
}
