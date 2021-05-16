using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitableSphere : Hitable
{
    public Vector3 Point;
    public float Radius;

    private void FixedUpdate()
    {
        Point = transform.position;
        Radius = transform.localScale.x / 2;
    }
}
