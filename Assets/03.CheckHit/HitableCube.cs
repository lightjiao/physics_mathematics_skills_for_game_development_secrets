using UnityEngine;

// 长方形的碰撞检测
public class HitableCube : MonoBehaviour, IHitable
{
    public float Left;
    public float Right;
    public float Top;
    public float Bottom;

    private void FixedUpdate()
    {
        var pos = transform.position;
        var scale = transform.localScale;

        var xOffset = scale.x / 2;
        var yOffset = scale.y / 2;

        Left = pos.x - xOffset;
        Right = pos.x + xOffset;
        Top = pos.y + yOffset;
        Bottom = pos.y - yOffset;
    }
}