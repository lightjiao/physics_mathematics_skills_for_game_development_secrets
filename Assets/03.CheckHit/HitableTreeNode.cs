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
public class HitableTreeNode
{
    // 稀疏的空隙
    private const float m_LooseSpacing = 10f;

    // 树节点的空间
    private ReactBox SpaceBox;

    private List<Hitable> m_Hitables;

    // 用这个字段判断是否是叶子节点
    private HitableTreeNode[] Childs;

    public HitableTreeNode(Vector2 spaceCenter, float spaceLength, List<Hitable> hitables)
    {
        SpaceCenter = spaceCenter;
        SpaceLength = spaceLength;
        m_Hitables = hitables;
    }

    private static void InitNode(HitableTreeNode node, List<Hitable> hitables, int recurse = 3)
    {
        if (recurse == 0) return;

        // 创建四个子节点，注意稀疏

        // 遍历所有hitables，如果一个物体在两个或以上的子节点有交叉，那就放到当前节点，否则就放到子节点
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