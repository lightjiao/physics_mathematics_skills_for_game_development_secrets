using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Hitable : MonoBehaviour
{
    private static CheckHitEngine m_HitEngint;

    public ReactBox BoundingBox;

    protected virtual void Awake()
    {
        BoundingBox = new ReactBox();
        if (m_HitEngint == null)
        {
            m_HitEngint = FindObjectOfType<CheckHitEngine>();
        }
    }

    public void SetHitStatus(bool hit)
    {
        var color = hit ? Color.red : Color.white;
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    private void FixedUpdate()
    {
        // m_HitEngint?.UpdatePos();
    }
}

/// <summary>
/// 用于包裹碰撞体的包围盒，便于计算空间的划分
/// 这个是轴对齐的，即不会随着物体的旋转而改变方向
/// </summary>
public struct ReactBox
{
    public float Left;
    public float Right;
    public float Top;
    public float Bottom;

    public float MiddleHeight => (Top + Bottom) / 2;
    public float MiddleLength => (Left + Right) / 2;

    public bool IsInBox(ReactBox b)
    {
        if (Left >= b.Left && Right <= b.Right && Top <= b.Top && Bottom >= b.Bottom)
        {
            return true;
        }
        return false;
    }

    public bool IsHit(ReactBox b)
    {
        if (Right >= b.Left && Left <= b.Right)
        {
            if (Bottom <= b.Top && Top >= b.Bottom)
            {
                return true;
            }
        }

        return false;
    }
}

/// <summary>
/// TODO: Cube任意的旋转之后，Cube与Cube之间的碰撞检测
/// TODO: Sphere 与任意旋转之后的Cube之间的碰撞检测
/// TODO: Capsule 与 Capsule 之间的碰撞检测
/// TODO: Capsule 与 Cube 任意旋转之后的碰撞检测
/// </summary>
public class CheckHitEngine : MonoBehaviour
{
    public float ReactBoxLength = 100;
    private List<HitableCube> m_Cubes;
    private List<HitableSphere> m_Spheres;
    private List<HitableCapsule> m_Capsule;
    private Quadtree m_Quadtree;

    private void Start()
    {
        var allSpace = new ReactBox
        {
            Left = -ReactBoxLength / 2,
            Right = ReactBoxLength / 2,
            Top = ReactBoxLength / 2,
            Bottom = -ReactBoxLength / 2
        };

        m_Quadtree = new Quadtree(allSpace);
        m_Quadtree.Init(FindObjectsOfType<Hitable>().ToList());

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
            InnerCheckCubes(cube, m_Quadtree);
        }
    }

    private void InnerCheckCubes(HitableCube cube, Quadtree quadtree)
    {
        if (quadtree == null) return;

        foreach (var hitable in quadtree.Hitables)
        {
            // CheckCollision
        }

        InnerCheckCubes(cube, quadtree.LeftTop);
        InnerCheckCubes(cube, quadtree.RightTop);
        InnerCheckCubes(cube, quadtree.LeftBottom);
        InnerCheckCubes(cube, quadtree.RightBottom);
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
        // 求两个线段离得最近的点，求点之间的距离
        // 距离小于半径之和就碰撞

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


    /// <summary>
    /// 四叉树加速碰撞检测
    /// </summary>
    /// <see>https://www.zhihu.com/question/25111128/answer/30129131</see>
    /// - 总的区域一开始要提前计算好边界，用于划分树节点的中心点和区域
    public class Quadtree
    {
        private static Dictionary<Hitable, Quadtree> m_HitableLookup = new Dictionary<Hitable, Quadtree>();

        private const int m_MaxDepth = 3;

        // 稀疏的空隙
        private const float m_LooseSpacing = 10f;

        // 树节点的空间
        public ReactBox SpaceBox { get; private set; }

        // 这个节点包含的可碰撞对象
        public List<HitableCube> HitableCubes;
        public List<HitableSphere> HitableSpheres;
        public List<HitableCapsule> HitableCapsules;

        public Quadtree Parent;
        public Quadtree LeftTop;
        public Quadtree RightTop;
        public Quadtree LeftBottom;
        public Quadtree RightBottom;

        public Quadtree(ReactBox spaceBox, Quadtree parent = null)
        {
            SpaceBox = spaceBox;
            Parent = parent;
            HitableCubes = new List<HitableCube>();
            HitableSpheres = new List<HitableSphere>();
            HitableCapsules = new List<HitableCapsule>();
        }

        public void Init(List<Hitable> hitables)
        {
            CreateChildRecursive(1 + 1);
            foreach (var hitable in hitables)
            {
                UpdateHitablePos(hitable, this);
            }
        }

        internal void CreateChildRecursive(int depth)
        {
            if (depth == m_MaxDepth) return;

            var leftTopBox = new ReactBox
            {
                Left = SpaceBox.Left,
                Right = SpaceBox.MiddleLength + m_LooseSpacing,
                Top = SpaceBox.Top,
                Bottom = SpaceBox.MiddleHeight - m_LooseSpacing
            };
            LeftTop = new Quadtree(leftTopBox, this);
            LeftTop.CreateChildRecursive(depth + 1);

            var rightTopBox = new ReactBox
            {
                Left = SpaceBox.MiddleLength - m_LooseSpacing,
                Right = SpaceBox.Right,
                Top = SpaceBox.Top,
                Bottom = SpaceBox.MiddleHeight - m_LooseSpacing
            };
            RightTop = new Quadtree(rightTopBox, this);
            RightTop.CreateChildRecursive(depth + 1);

            var leftBottomBox = new ReactBox
            {
                Left = SpaceBox.Left,
                Right = SpaceBox.MiddleLength + m_LooseSpacing,
                Top = SpaceBox.MiddleHeight + m_LooseSpacing,
                Bottom = SpaceBox.Bottom
            };
            LeftBottom = new Quadtree(leftBottomBox, this);
            LeftBottom.CreateChildRecursive(depth + 1);

            var rightBottomBox = new ReactBox
            {
                Left = SpaceBox.MiddleLength - m_LooseSpacing,
                Right = SpaceBox.Right,
                Top = SpaceBox.MiddleHeight + m_LooseSpacing,
                Bottom = SpaceBox.Bottom
            };
            RightBottom = new Quadtree(rightBottomBox, this);
            RightBottom.CreateChildRecursive(depth + 1);
        }

        /// <summary>
        /// 更新Hitable在树节点中的位置
        /// </summary>
        /// <param name="hitable"></param>
        /// <param name="node"></param>
        /// <returns>返回实际更新到了的节点位置</returns>
        private bool UpdateHitablePos<T>(T hitable, Quadtree node) where T : Hitable
        {
            if (node == null || false == hitable.BoundingBox.IsInBox(node.SpaceBox))
            {
                return false;
            }

            // 检查并更新到子节点
            var isInChild = UpdateHitablePos(hitable, node.LeftTop) ||
                            UpdateHitablePos(hitable, node.RightTop) ||
                            UpdateHitablePos(hitable, node.LeftBottom) ||
                            UpdateHitablePos(hitable, node.RightBottom);
            if (isInChild)
            {
                return true;
            }

            if (m_HitableLookup.TryGetValue(hitable, out var oldNode))
            {
                if (oldNode == node)
                {
                    return true;
                }
            }

            oldNode?.Hitables.Remove(hitable);
            node.Hitables.Add(hitable);
            m_HitableLookup[hitable] = node;

            return true;
        }

        public void AddHitable<T>(T hitable) where T : Hitable
        {
        }

        public void RemoveHitable(Hitable hitable)
        {
            if (hitable is HitableCube)
            {
                HitableCubes.Remove(hitable);
            }
        }

        /// <summary>
        /// 更新Hitable在树节点中的位置
        /// </summary>
        /// <param name="hitable"></param>
        public void UpdateHitablePos<T>(T hitable) where T : Hitable
        {
            m_HitableLookup.TryGetValue(hitable, out var node);
            if (node == null) node = this;

            do
            {
                if (UpdateHitablePos<T>(hitable, node))
                {
                    break;
                }

                node = node.Parent;
            } while (node != null);
        }

        public void DestroyHitables<T>(T hitable) where T : Hitable
        {

        }
    }
}
