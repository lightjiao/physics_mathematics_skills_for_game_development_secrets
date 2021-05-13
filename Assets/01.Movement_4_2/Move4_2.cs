using UnityEngine;

// 抛物线运动 + 回弹效果
public class Move4_2 : MonoBehaviour
{
    public float m_vx = 10f;
    public float m_vy;

    public float g = 0.4f;

    // Start is called before the first frame update
    private void Start()
    {
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        m_vy -= g;

        var pos = transform.position;
        pos.x += m_vx * Time.fixedDeltaTime;
        pos.y += m_vy * Time.fixedDeltaTime;

        LimitWidth(ref pos, out var outofX, out var outOfY);
        if (outofX)
        {
            m_vx = -m_vx * 0.8f;
        }
        if (outOfY)
        {
            m_vy = -m_vy * 0.6f;
        }

        transform.position = pos;
    }

    private void LimitWidth(ref Vector3 pos, out bool outOfX, out bool outOfY)
    {
        outOfX = false;
        outOfY = false;

        if (pos.x > 10)
        {
            pos.x = 10;
            outOfX = true;
        }
        if (pos.x < 0)
        {
            pos.x = 0;
            outOfX = true;
        }

        if (pos.y > 0)
        {
            pos.y = 0;
            outOfY = true;
        }
        if (pos.y < -10)
        {
            pos.y = -10;
            outOfY = true;
        }
    }
}