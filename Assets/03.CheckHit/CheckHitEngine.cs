using UnityEngine;
using System.Collections.Generic;

public interface IHitable { }

public static class IHitableEx
{
    public static void SetHitStatus(this IHitable hitable, bool hit)
    {
        var color = hit ? Color.red : Color.white;
        if (hitable is MonoBehaviour monoHitable)
        {
            monoHitable.GetComponent<Renderer>().material.SetColor("_Color", color);
        }
    }
}

public class CheckHitEngine : MonoBehaviour
{
    public List<IHitable> m_HitTargets;

    private void Awake()
    {
        m_HitTargets = new List<IHitable>();
    }

    private void Start()
    {
        var objs = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
        foreach (var item in objs)
        {
            if (item is IHitable)
            {
                m_HitTargets.Add((IHitable)item);
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (var item in m_HitTargets)
        {
            item.SetHitStatus(false);
        }

        // 简单的列表遍历所有可碰撞对象
        for (var i = 0; i < m_HitTargets.Count; i++)
        {
            var item = m_HitTargets[i];
            if (item is HitableCube cubeA)
            {
                for (var j = i + 1; j < m_HitTargets.Count; j++)
                {
                    var anOtherOne = m_HitTargets[j];
                    if (CheckCubeHit(cubeA, anOtherOne))
                    {
                        item.SetHitStatus(true);
                        anOtherOne.SetHitStatus(true);
                    }
                }
            }
        }
    }

    private bool CheckCubeHit(HitableCube cube, IHitable b)
    {
        switch (b)
        {
            case HitableCube bCube:
                return CheckCubeAndCube(cube, bCube);
            default:
                return false;
        }

    }


    private bool CheckCubeAndCube(HitableCube a, HitableCube b)
    {
        if (a.Right >= b.Left && a.Left <= b.Right)
        {
            if (a.Bottom <= b.Top && a.Top >= b.Bottom)
            {
                return true;
            }
        }
        return false;
    }
}