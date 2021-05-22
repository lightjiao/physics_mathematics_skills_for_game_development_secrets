using UnityEngine;

// 长方形的碰撞检测
// TODO: 旋转
public class HitableCube : Hitable
{
    public float Left;
    public float Right;
    public float Top;
    public float Bottom;

    private float m_BoundBoxHalfLength = -1f;

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

        UpdateBoundBox();
    }

    private void UpdateBoundBox()
    {
        // 最大值设置为Bound
        if (m_BoundBoxHalfLength < 0)
        {
            var disSqr = (Left - Right) * (Left - Right) + (Top - Bottom) * (Top - Bottom);
            m_BoundBoxHalfLength = Mathf.Sqrt(disSqr) / 2;
        }

        var pos = transform.position;

        BoundingBox.Left = pos.x - m_BoundBoxHalfLength;
        BoundingBox.Right = pos.x + m_BoundBoxHalfLength;
        BoundingBox.Top = pos.y + m_BoundBoxHalfLength;
        BoundingBox.Bottom = pos.y - m_BoundBoxHalfLength;
    }
}