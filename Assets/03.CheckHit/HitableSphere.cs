using UnityEngine;

public class HitableSphere : Hitable
{
    public Vector2 Point;
    public float Radius;

    private void FixedUpdate()
    {
        Point = transform.position;
        Radius = transform.localScale.x / 2;

        UpdateBoundBox();
    }

    private void UpdateBoundBox()
    {
        BoundingBox.Left = Point.x - Radius;
        BoundingBox.Right = Point.x + Radius;
        BoundingBox.Top = Point.y + Radius;
        BoundingBox.Bottom = Point.y - Radius;
    }
}
