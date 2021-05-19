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

/// <summary>
/// TODO: 扇形的碰撞检测没有做，原因是要在界面里“画出”扇形比较麻烦，先不管了
/// TODO: Cube任意的旋转之后，Cube与Cube之间的碰撞检测
/// TODO: Sphere 与任意旋转之后的Cube之间的碰撞检测
/// TODO: Capsule 与 Capsule 之间的碰撞检测
/// TODO: Capsule 与 Cube 任意旋转之后的碰撞检测
/// TODO: 每一个碰撞体都需要实现自身的BoundBox，支持不同的旋转的情况下 --> 如果粗暴一点就取最大值就好了
/// </summary>
public class CheckHitEngine : MonoBehaviour
{
    public List<HitableCube> m_Cubes;
    public List<HitableSphere> m_Spheres;
    public List<HitableCapsule> m_Capsule;

    private void Start()
    {
        m_Cubes = FindObjectsOfType<HitableCube>().ToList();
        m_Spheres = FindObjectsOfType<HitableSphere>().ToList();
        m_Capsule = FindObjectsOfType<HitableCapsule>().ToList();
    }



    private void FixedUpdate()
    {
        ResetAllHitStatus();

        CheckCubes();
        CheckSpheres();
        CheckCapsules();
    }

    private void ResetAllHitStatus()
    {
        foreach (var item in m_Cubes) item.SetHitStatus(false);
        foreach (var item in m_Spheres) item.SetHitStatus(false);
        foreach (var item in m_Capsule) item.SetHitStatus(false);
    }

    private void CheckCubes()
    {
        for (var i = 0; i < m_Cubes.Count; i++)
        {
            var cube = m_Cubes[i];
            for (var j = i + 1; j < m_Cubes.Count; j++)
            {
                var cube2 = m_Cubes[j];
                if (CheckCubeAndCube(cube, cube2))
                {
                    cube.SetHitStatus(true);
                    cube2.SetHitStatus(true);
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

    private void CheckSpheres()
    {
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

            foreach (var cube in m_Cubes)
            {
                if (CheckSphereAndCube(sphere, cube))
                {
                    sphere.SetHitStatus(true);
                    cube.SetHitStatus(true);
                }
            }
        }
    }

    private bool CheckSphereAndSphere(HitableSphere sphereA, HitableSphere sphereB)
    {
        var deltaX = sphereA.Point.x - sphereB.Point.x;
        var deltaY = sphereA.Point.y - sphereB.Point.y;
        var disSqrt = deltaX * deltaX + deltaY * deltaY;

        var radiusSum = sphereA.Radius + sphereB.Radius;
        return disSqrt <= (radiusSum * radiusSum);
    }

    private bool CheckSphereAndCube(HitableSphere sphere, HitableCube cube)
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

    private void CheckCapsules()
    {
        for (var i = 0; i < m_Capsule.Count; i++)
        {
            var capsule = m_Capsule[i];
            for (var j = i + 1; j < m_Capsule.Count; j++)
            {
                if (CheckCapsuleAndCapsule(capsule, m_Capsule[j]))
                {
                    capsule.SetHitStatus(true);
                    m_Capsule[j].SetHitStatus(true);
                }
            }

            foreach (var cube in m_Cubes)
            {
                if (CheckCapsuleAndCube(capsule, cube))
                {
                    capsule.SetHitStatus(true);
                    cube.SetHitStatus(true);
                }
            }

            foreach (var sphere in m_Spheres)
            {
                if (CheckCapsuleAndSphere(capsule, sphere))
                {
                    capsule.SetHitStatus(true);
                    sphere.SetHitStatus(true);
                }
            }
        }
    }

    private bool CheckCapsuleAndCapsule(HitableCapsule capsuleA, HitableCapsule capsuleB)
    {
        // 平行
        if (capsuleA.Vec.x / capsuleB.Vec.x == capsuleA.Vec.y / capsuleB.Vec.y)
        {
            
        }
        else
        {
            // 求两个线段离得最近的点，求点之间的距离
            // 距离小于半径之和就碰撞
        }

        return false;
    }

    private bool CheckCapsuleAndCube(HitableCapsule capsule, HitableCube cube)
    {
        return false;
    }

    private bool CheckCapsuleAndSphere(HitableCapsule capsule, HitableSphere sphere)
    {
        var dx = sphere.Point.x - capsule.Point.x;
        var dy = sphere.Point.y - capsule.Point.y;
        var t = (capsule.Vec.x * dx + capsule.Vec.y * dy) /
                (capsule.Vec.x * capsule.Vec.x + capsule.Vec.y * capsule.Vec.y);
        t = Mathf.Clamp01(t);

        var mx = capsule.Vec.x * t + capsule.Point.x;
        var my = capsule.Vec.y * t + capsule.Point.y;

        var disSqrt = (mx - sphere.Point.x) * (mx - sphere.Point.x) +
                        (my - sphere.Point.y) * (my - sphere.Point.y);

        var radiusSum = capsule.Radius + sphere.Radius;
        if (disSqrt <= radiusSum * radiusSum)
        {
            return true;
        }

        return false;
    }
}