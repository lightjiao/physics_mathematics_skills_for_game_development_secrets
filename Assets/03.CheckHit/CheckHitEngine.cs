using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Hitable : MonoBehaviour
{
    public void SetHitStatus(bool hit)
    {
        var color = hit ? Color.red : Color.white;
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }
}

public class CheckHitEngine : MonoBehaviour
{
    public List<HitableCube> m_Cubes;
    public List<HitableSphere> m_Spheres;

    private void Start()
    {
        m_Cubes = FindObjectsOfType<HitableCube>().ToList();
        m_Spheres = FindObjectsOfType<HitableSphere>().ToList();
    }

    private void FixedUpdate()
    {
        // 简单的列表遍历所有可碰撞对象
        foreach (var item in m_Cubes) item.SetHitStatus(false);
        for (var i = 0; i < m_Cubes.Count; i++)
        {
            var cube1 = m_Cubes[i];
            for (var j = i + 1; j < m_Cubes.Count; j++)
            {
                var cube2 = m_Cubes[j];
                if (CheckCubeAndCube(cube1, cube2))
                {
                    cube1.SetHitStatus(true);
                    cube2.SetHitStatus(true);
                }
            }
        }

        foreach (var item in m_Spheres) item.SetHitStatus(false);
        for (var i = 0; i < m_Spheres.Count; i++)
        {
            var sphere = m_Spheres[i];
            for (var j = i + 1; j < m_Spheres.Count; j++)
            {
                if (CheckSphereAndSphere(sphere, m_Spheres[j]))
                {
                    sphere.SetHitStatus(true);
                    m_Spheres[j].SetHitStatus(true);
                }
            }
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

    private bool CheckCubeAndSphere(HitableCube cube, HitableSphere sphere)
    {
        return false;
    }

    private bool CheckSphereAndSphere(HitableSphere sphereA, HitableSphere sphereB)
    {
        var deltaX = sphereA.Point.x - sphereB.Point.x;
        var deltaY = sphereA.Point.y - sphereB.Point.y;
        var disSqrt = deltaX * deltaX + deltaY * deltaY;

        var radiusSum = sphereA.Radius + sphereB.Radius;
        return disSqrt <= (radiusSum * radiusSum);
    }
}