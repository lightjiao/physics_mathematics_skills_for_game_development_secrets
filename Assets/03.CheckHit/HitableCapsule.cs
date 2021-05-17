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
    private const float _height = 1f;

    private void FixedUpdate()
    {
        var scale = transform.localScale.x;

        Radius = _radius * scale;

        var pos = transform.position;
        Point.x = pos.x;
        Point.y = pos.y - (_height - (Radius * 2)) * scale / 2;

        var anotherPoint = new Vector2();
        anotherPoint.x = pos.x;
        anotherPoint.y = pos.y + (_height - (Radius * 2)) * scale / 2;

        Vec = anotherPoint - Point;
    }
}
