using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move1_2 : MonoBehaviour
{
    private float m_Speed = 10f;
    private bool m_RepeatFlag = false;
    private Vector3 m_Pos;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        MoveSphere();
    }

    void MoveSphere()
    {
        m_Pos.x += m_Speed * Time.fixedDeltaTime;
        if (m_Pos.x > 10)
        {
            m_Pos.x = 10;
            m_Speed = -m_Speed;
        }
        if (m_Pos.x < 0)
        {
            m_Pos.x = 0;
            m_Speed = -m_Speed;
        }

        m_RepeatFlag = !m_RepeatFlag;
        if (m_RepeatFlag)
        {
            transform.position = Vector3.zero;
        }
        else
        {
            transform.position = m_Pos;
        }
    }
}
