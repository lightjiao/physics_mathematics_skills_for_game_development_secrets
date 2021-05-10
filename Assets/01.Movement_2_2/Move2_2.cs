using UnityEngine;

// 按键输入加速减速运动
public class Move2_2 : MonoBehaviour
{
    private float m_Accelerate = 10f;
    private float m_MaxSpeed = 10f;
    private Vector3 m_Pos;

    private Vector3 m_Speed;

    // Start is called before the first frame update
    void Start()
    {
        m_Speed = Vector3.zero;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        MoveSphere();
    }

    void MoveSphere()
    {
        var inputx = Input.GetAxis("Horizontal");
        var inputy = Input.GetAxis("Vertical");

        // 有输入就在输入的方向加速度，没有输入就在运行方向减速
        float a_x;
        if (Mathf.Abs(inputx - 0) > float.Epsilon)
        {
            a_x = inputx * m_Accelerate;
        }
        else
        {
            a_x = -m_Accelerate;
            if (Mathf.Abs(m_Speed.x - 0) < float.Epsilon) a_x = 0;
            if (m_Speed.x < 0) a_x = -a_x;
        }

        float a_y;
        if (Mathf.Abs(inputy - 0) > float.Epsilon)
        {
            a_y = inputy * m_Accelerate;
        }
        else
        {
            a_y = -m_Accelerate;
            if (Mathf.Abs(m_Speed.y - 0) < float.Epsilon) a_y = 0;
            if (m_Speed.y < 0) a_y = -a_y;
        }

        var a = new Vector3(a_x, a_y, 0);
        m_Speed += a * Time.fixedDeltaTime;

        m_Speed.x = Mathf.Clamp(m_Speed.x, -m_MaxSpeed, m_MaxSpeed);
        m_Speed.y = Mathf.Clamp(m_Speed.y, -m_MaxSpeed, m_MaxSpeed);

        m_Pos += m_Speed * Time.fixedDeltaTime;
        m_Pos = LimitWidth(m_Pos);

        transform.position = m_Pos;
    }

    private Vector3 LimitWidth(Vector3 pos)
    {
        if (pos.x > 10) pos.x = 10;
        if (pos.x < 0) pos.x = 0;

        if (pos.y > 0) pos.y = 0;
        if (pos.y < -10) pos.y = -10;

        return pos;
    }
}
