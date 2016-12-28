using System.Collections.Generic;

namespace ClipperLib
{
    public class MyIntersectNodeSort : IComparer<IntersectNode>
    {
        public int Compare(IntersectNode node1, IntersectNode node2)
        {
            int i = node2.Pt.Y - node1.Pt.Y;
            if (i > 0) return 1;
            else if (i < 0) return -1;
            else return 0;
        }
    }
}