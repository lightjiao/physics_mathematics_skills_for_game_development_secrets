using System.Collections.Generic;
using UnityEngine;

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
        throw new System.Exception();
    }
}


/// <summary>
/// 四叉树优化2D的碰撞
/// </summary>
/// <see>https://www.zhihu.com/question/25111128/answer/30129131</see>
/// - 总的区域一开始要提前计算好边界，用于划分树节点的中心点和区域
public class Quadtree
{
    // 稀疏的空隙
    private const float m_LooseSpacing = 10f;

    // 树节点的空间
    public ReactBox SpaceBox { get; private set; }

    // 这个树节点包含的可碰撞对象
    private List<Hitable> m_Hitables;

    private Quadtree m_LeftTop;
    private Quadtree m_RightTop;
    private Quadtree m_LeftBottom;
    private Quadtree m_RightBottom;

    public Quadtree(ReactBox spaceBox)
    {
        SpaceBox = spaceBox;
    }

    private void Init(List<Hitable> hitables, int depth = 1)
    {
        // 不超过三层
        if (depth == 3)
        {
            m_Hitables = hitables;
            return;
        }

        CreateChilds();

        m_Hitables = new List<Hitable>();
        var leftTopHitables = new List<Hitable>();
        var rightTopHitables = new List<Hitable>();
        var leftBottomHitables = new List<Hitable>();
        var rightBottomHitables = new List<Hitable>();

        // 遍历所有hitables，如果一个物体没有完全的在任何子box中，那就放到当前节点，否则就放到子节点
        foreach (var hitable in hitables)
        {
            if (hitable.BoundingBox.IsInBox(m_LeftTop.SpaceBox))
            {
                leftTopHitables.Add(hitable);
            }
            else if (hitable.BoundingBox.IsInBox(m_RightTop.SpaceBox))
            {
                rightTopHitables.Add(hitable);
            }
            else if (hitable.BoundingBox.IsInBox(m_LeftBottom.SpaceBox))
            {
                leftBottomHitables.Add(hitable);
            }
            else if (hitable.BoundingBox.IsInBox(m_RightBottom.SpaceBox))
            {
                rightBottomHitables.Add(hitable);
            }
            else
            {
                m_Hitables.Add(hitable);
            }
        }

        m_LeftTop.Init(leftTopHitables, depth + 1);
        m_RightTop.Init(rightTopHitables, depth + 1);
        m_LeftBottom.Init(leftBottomHitables, depth + 1);
        m_RightBottom.Init(rightBottomHitables, depth + 1);
    }

    // 创建四个子节点的空间
    private void CreateChilds()
    {
        var leftTopBox = new ReactBox();
        leftTopBox.Left = SpaceBox.Left;
        leftTopBox.Right = SpaceBox.MiddleLength + m_LooseSpacing;
        leftTopBox.Top = SpaceBox.Top;
        leftTopBox.Bottom = SpaceBox.MiddleHeight - m_LooseSpacing;
        m_LeftTop = new Quadtree(leftTopBox);

        var rightTopBox = new ReactBox();
        rightTopBox.Left = SpaceBox.MiddleLength - m_LooseSpacing;
        rightTopBox.Right = SpaceBox.Right;
        rightTopBox.Top = SpaceBox.Top;
        rightTopBox.Bottom = SpaceBox.MiddleHeight - m_LooseSpacing;
        m_RightTop = new Quadtree(rightTopBox);


        var leftBottomBox = new ReactBox();
        leftBottomBox.Left = SpaceBox.Left;
        leftBottomBox.Right = SpaceBox.MiddleLength + m_LooseSpacing;
        leftBottomBox.Top = SpaceBox.MiddleHeight + m_LooseSpacing;
        leftBottomBox.Bottom = SpaceBox.Bottom;
        m_LeftBottom = new Quadtree(leftBottomBox);

        var rightBottomBox = new ReactBox();
        rightBottomBox.Left = SpaceBox.MiddleLength - m_LooseSpacing;
        rightBottomBox.Right = SpaceBox.Right;
        rightBottomBox.Top = SpaceBox.MiddleHeight + m_LooseSpacing;
        rightBottomBox.Bottom = SpaceBox.Bottom;
        m_RightBottom = new Quadtree(rightBottomBox);
    }

    /// <summary>
    /// 移动了碰撞体后需要更新在树中的位置
    /// </summary>
    /// <param name="hitable"></param>
    public void HitableMove(Hitable hitable)
    {
        // 缓存 <Hitable, Node>
        // 是否还在原来的Node中
        // 判断是否在parent中 --> 递归判断
        // 
    }

    public bool CheckHit(Hitable a, Hitable b)
    {

        return false;
    }
}