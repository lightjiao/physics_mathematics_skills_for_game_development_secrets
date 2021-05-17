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

    private void FixedUpdate()
    {
        var scale = transform.localScale.x;

        var pos = transform.position;
        Point.x = pos.x;
        Point.y = pos.y + _height * scale / 2;
        Radius = _radius * scale;

        var anotherPoint = Point;
        anotherPoint.y = Point.y - _height * scale;
        Vec = anotherPoint - Point;
    }
}
