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
        foreach (var item in m_Cubes) item.SetHitStatus(false);
        foreach (var item in m_Spheres) item.SetHitStatus(false);

        // 简单的列表遍历所有可碰撞对象
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

            foreach (var sphere in m_Spheres)
            {
                if (CheckCubeAndSphere(cube1, sphere))
                {
                    cube1.SetHitStatus(true);
                    sphere.SetHitStatus(true);
                }
            }
        }

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
        // 圆心在长方形很外面
        if ((sphere.Point.x < cube.Left - sphere.Radius) ||
            (sphere.Point.x > cube.Right + sphere.Radius) ||
            (sphere.Point.y > cube.Top + sphere.Radius) ||
            (sphere.Point.y < cube.Bottom - sphere.Radius)
            )
        {
            return false;
        }


        var result = true;
        var radiusSqrt = sphere.Radius * sphere.Radius;
        if (sphere.Point.x < cube.Left)
        {
            // 左上角
            if (sphere.Point.y > cube.Top)
            {
                var deltaX = sphere.Point.x - cube.Left;
                var deltaY = sphere.Point.y - cube.Top;
                var disSqrt = deltaX * deltaX + deltaY * deltaY;
                if (radiusSqrt < disSqrt) result = false;
            }

            // 左下角
            else if (sphere.Point.y < cube.Bottom)
            {
                var deltaX = sphere.Point.x - cube.Left;
                var deltaY = sphere.Point.y - cube.Bottom;
                var disSqrt = deltaX * deltaX + deltaY * deltaY;
                if (radiusSqrt < disSqrt) result = false;
            }
        }
        else if (sphere.Point.x > cube.Right)
        {
            // 右上角
            if (sphere.Point.y > cube.Top)
            {
                var deltaX = sphere.Point.x - cube.Right;
                var deltaY = sphere.Point.y - cube.Top;
                var disSqrt = deltaX * deltaX + deltaY * deltaY;
                if (radiusSqrt < disSqrt) result = false;
            }

            // 右下角
            else if (sphere.Point.y < cube.Bottom)
            {
                var deltaX = sphere.Point.x - cube.Right;
                var deltaY = sphere.Point.y - cube.Bottom;
                var disSqrt = deltaX * deltaX + deltaY * deltaY;
                if (radiusSqrt < disSqrt) result = false;
            }
        }

        return result;
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