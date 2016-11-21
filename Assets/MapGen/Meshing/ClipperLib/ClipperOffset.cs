using System;
using System.Collections.Generic;

namespace ClipperLib
{
    public class ClipperOffset
    {
        private List<List<IntPoint>> m_destPolys;
        private List<IntPoint> m_srcPoly;
        private List<IntPoint> m_destPoly;
        private List<DoublePoint> m_normals = new List<DoublePoint>();
        private double m_delta, m_sinA, m_sin, m_cos;
        private double m_miterLim, m_StepsPerRad;

        private IntPoint m_lowest;
        private PolyNode m_polyNodes = new PolyNode();

        public double ArcTolerance { get; set; }
        public double MiterLimit { get; set; }

        private const double two_pi = Math.PI * 2;
        private const double def_arc_tolerance = 0.25;

        public ClipperOffset(
          double miterLimit = 2.0, double arcTolerance = def_arc_tolerance)
        {
            MiterLimit = miterLimit;
            ArcTolerance = arcTolerance;
            m_lowest.X = -1;
        }
        //------------------------------------------------------------------------------

        public void Clear()
        {
            m_polyNodes.Childs.Clear();
            m_lowest.X = -1;
        }
        //------------------------------------------------------------------------------

        internal static int Round(double value)
        {
            return value < 0 ? (int)(value - 0.5) : (int)(value + 0.5);
        }
        //------------------------------------------------------------------------------

        public void AddPath(List<IntPoint> path, JoinType joinType, EndType endType)
        {
            int highI = path.Count - 1;
            if (highI < 0) return;
            PolyNode newNode = new PolyNode();
            newNode.m_jointype = joinType;
            newNode.m_endtype = endType;

            //strip duplicate points from path and also get index to the lowest point ...
            if (endType == EndType.etClosedLine || endType == EndType.etClosedPolygon)
                while (highI > 0 && path[0] == path[highI]) highI--;
            newNode.m_polygon.Capacity = highI + 1;
            newNode.m_polygon.Add(path[0]);
            int j = 0, k = 0;
            for (int i = 1; i <= highI; i++)
                if (newNode.m_polygon[j] != path[i])
                {
                    j++;
                    newNode.m_polygon.Add(path[i]);
                    if (path[i].Y > newNode.m_polygon[k].Y ||
                      (path[i].Y == newNode.m_polygon[k].Y &&
                      path[i].X < newNode.m_polygon[k].X)) k = j;
                }
            if (endType == EndType.etClosedPolygon && j < 2) return;

            m_polyNodes.AddChild(newNode);

            //if this path's lowest pt is lower than all the others then update m_lowest
            if (endType != EndType.etClosedPolygon) return;
            if (m_lowest.X < 0)
                m_lowest = new IntPoint(m_polyNodes.ChildCount - 1, k);
            else
            {
                IntPoint ip = m_polyNodes.Childs[(int)m_lowest.X].m_polygon[(int)m_lowest.Y];
                if (newNode.m_polygon[k].Y > ip.Y ||
                  (newNode.m_polygon[k].Y == ip.Y &&
                  newNode.m_polygon[k].X < ip.X))
                    m_lowest = new IntPoint(m_polyNodes.ChildCount - 1, k);
            }
        }
        //------------------------------------------------------------------------------

        public void AddPaths(List<List<IntPoint>> paths, JoinType joinType, EndType endType)
        {
            foreach (List<IntPoint> p in paths)
                AddPath(p, joinType, endType);
        }
        //------------------------------------------------------------------------------

        private void FixOrientations()
        {
            //fixup orientations of all closed paths if the orientation of the
            //closed path with the lowermost vertex is wrong ...
            if (m_lowest.X >= 0 &&
              !Clipper.Orientation(m_polyNodes.Childs[(int)m_lowest.X].m_polygon))
            {
                for (int i = 0; i < m_polyNodes.ChildCount; i++)
                {
                    PolyNode node = m_polyNodes.Childs[i];
                    if (node.m_endtype == EndType.etClosedPolygon ||
                      (node.m_endtype == EndType.etClosedLine &&
                      Clipper.Orientation(node.m_polygon)))
                        node.m_polygon.Reverse();
                }
            }
            else
            {
                for (int i = 0; i < m_polyNodes.ChildCount; i++)
                {
                    PolyNode node = m_polyNodes.Childs[i];
                    if (node.m_endtype == EndType.etClosedLine &&
                      !Clipper.Orientation(node.m_polygon))
                        node.m_polygon.Reverse();
                }
            }
        }
        //------------------------------------------------------------------------------

        internal static DoublePoint GetUnitNormal(IntPoint pt1, IntPoint pt2)
        {
            double dx = (pt2.X - pt1.X);
            double dy = (pt2.Y - pt1.Y);
            if ((dx == 0) && (dy == 0)) return new DoublePoint();

            double f = 1 * 1.0 / Math.Sqrt(dx * dx + dy * dy);
            dx *= f;
            dy *= f;

            return new DoublePoint(dy, -dx);
        }
        //------------------------------------------------------------------------------

        private void DoOffset(double delta)
        {
            m_destPolys = new List<List<IntPoint>>();
            m_delta = delta;

            //if Zero offset, just copy any CLOSED polygons to m_p and return ...
            if (ClipperBase.near_zero(delta))
            {
                m_destPolys.Capacity = m_polyNodes.ChildCount;
                for (int i = 0; i < m_polyNodes.ChildCount; i++)
                {
                    PolyNode node = m_polyNodes.Childs[i];
                    if (node.m_endtype == EndType.etClosedPolygon)
                        m_destPolys.Add(node.m_polygon);
                }
                return;
            }

            //see offset_triginometry3.svg in the documentation folder ...
            if (MiterLimit > 2) m_miterLim = 2 / (MiterLimit * MiterLimit);
            else m_miterLim = 0.5;

            double y;
            if (ArcTolerance <= 0.0)
                y = def_arc_tolerance;
            else if (ArcTolerance > Math.Abs(delta) * def_arc_tolerance)
                y = Math.Abs(delta) * def_arc_tolerance;
            else
                y = ArcTolerance;
            //see offset_triginometry2.svg in the documentation folder ...
            double steps = Math.PI / Math.Acos(1 - y / Math.Abs(delta));
            m_sin = Math.Sin(two_pi / steps);
            m_cos = Math.Cos(two_pi / steps);
            m_StepsPerRad = steps / two_pi;
            if (delta < 0.0) m_sin = -m_sin;

            m_destPolys.Capacity = m_polyNodes.ChildCount * 2;
            for (int i = 0; i < m_polyNodes.ChildCount; i++)
            {
                PolyNode node = m_polyNodes.Childs[i];
                m_srcPoly = node.m_polygon;

                int len = m_srcPoly.Count;

                if (len == 0 || (delta <= 0 && (len < 3 ||
                  node.m_endtype != EndType.etClosedPolygon)))
                    continue;

                m_destPoly = new List<IntPoint>();

                if (len == 1)
                {
                    if (node.m_jointype == JoinType.jtRound)
                    {
                        double X = 1.0, Y = 0.0;
                        for (int j = 1; j <= steps; j++)
                        {
                            m_destPoly.Add(new IntPoint(
                              Round(m_srcPoly[0].X + X * delta),
                              Round(m_srcPoly[0].Y + Y * delta)));
                            double X2 = X;
                            X = X * m_cos - m_sin * Y;
                            Y = X2 * m_sin + Y * m_cos;
                        }
                    }
                    else
                    {
                        double X = -1.0, Y = -1.0;
                        for (int j = 0; j < 4; ++j)
                        {
                            m_destPoly.Add(new IntPoint(
                              Round(m_srcPoly[0].X + X * delta),
                              Round(m_srcPoly[0].Y + Y * delta)));
                            if (X < 0) X = 1;
                            else if (Y < 0) Y = 1;
                            else X = -1;
                        }
                    }
                    m_destPolys.Add(m_destPoly);
                    continue;
                }

                //build m_normals ...
                m_normals.Clear();
                m_normals.Capacity = len;
                for (int j = 0; j < len - 1; j++)
                    m_normals.Add(GetUnitNormal(m_srcPoly[j], m_srcPoly[j + 1]));
                if (node.m_endtype == EndType.etClosedLine ||
                  node.m_endtype == EndType.etClosedPolygon)
                    m_normals.Add(GetUnitNormal(m_srcPoly[len - 1], m_srcPoly[0]));
                else
                    m_normals.Add(new DoublePoint(m_normals[len - 2]));

                if (node.m_endtype == EndType.etClosedPolygon)
                {
                    int k = len - 1;
                    for (int j = 0; j < len; j++)
                        OffsetPoint(j, ref k, node.m_jointype);
                    m_destPolys.Add(m_destPoly);
                }
                else if (node.m_endtype == EndType.etClosedLine)
                {
                    int k = len - 1;
                    for (int j = 0; j < len; j++)
                        OffsetPoint(j, ref k, node.m_jointype);
                    m_destPolys.Add(m_destPoly);
                    m_destPoly = new List<IntPoint>();
                    //re-build m_normals ...
                    DoublePoint n = m_normals[len - 1];
                    for (int j = len - 1; j > 0; j--)
                        m_normals[j] = new DoublePoint(-m_normals[j - 1].X, -m_normals[j - 1].Y);
                    m_normals[0] = new DoublePoint(-n.X, -n.Y);
                    k = 0;
                    for (int j = len - 1; j >= 0; j--)
                        OffsetPoint(j, ref k, node.m_jointype);
                    m_destPolys.Add(m_destPoly);
                }
                else
                {
                    int k = 0;
                    for (int j = 1; j < len - 1; ++j)
                        OffsetPoint(j, ref k, node.m_jointype);

                    IntPoint pt1;
                    if (node.m_endtype == EndType.etOpenButt)
                    {
                        int j = len - 1;
                        pt1 = new IntPoint((int)Round(m_srcPoly[j].X + m_normals[j].X *
                          delta), (int)Round(m_srcPoly[j].Y + m_normals[j].Y * delta));
                        m_destPoly.Add(pt1);
                        pt1 = new IntPoint((int)Round(m_srcPoly[j].X - m_normals[j].X *
                          delta), (int)Round(m_srcPoly[j].Y - m_normals[j].Y * delta));
                        m_destPoly.Add(pt1);
                    }
                    else
                    {
                        int j = len - 1;
                        k = len - 2;
                        m_sinA = 0;
                        m_normals[j] = new DoublePoint(-m_normals[j].X, -m_normals[j].Y);
                        if (node.m_endtype == EndType.etOpenSquare)
                            DoSquare(j, k);
                        else
                            DoRound(j, k);
                    }

                    //re-build m_normals ...
                    for (int j = len - 1; j > 0; j--)
                        m_normals[j] = new DoublePoint(-m_normals[j - 1].X, -m_normals[j - 1].Y);

                    m_normals[0] = new DoublePoint(-m_normals[1].X, -m_normals[1].Y);

                    k = len - 1;
                    for (int j = k - 1; j > 0; --j)
                        OffsetPoint(j, ref k, node.m_jointype);

                    if (node.m_endtype == EndType.etOpenButt)
                    {
                        pt1 = new IntPoint((int)Round(m_srcPoly[0].X - m_normals[0].X * delta),
                          (int)Round(m_srcPoly[0].Y - m_normals[0].Y * delta));
                        m_destPoly.Add(pt1);
                        pt1 = new IntPoint((int)Round(m_srcPoly[0].X + m_normals[0].X * delta),
                          (int)Round(m_srcPoly[0].Y + m_normals[0].Y * delta));
                        m_destPoly.Add(pt1);
                    }
                    else
                    {
                        k = 1;
                        m_sinA = 0;
                        if (node.m_endtype == EndType.etOpenSquare)
                            DoSquare(0, 1);
                        else
                            DoRound(0, 1);
                    }
                    m_destPolys.Add(m_destPoly);
                }
            }
        }
        //------------------------------------------------------------------------------

        public void Execute(ref List<List<IntPoint>> solution, double delta)
        {
            solution.Clear();
            FixOrientations();
            DoOffset(delta);
            //now clean up 'corners' ...
            Clipper clpr = new Clipper();
            clpr.AddPaths(m_destPolys, PolyType.ptSubject, true);
            if (delta > 0)
            {
                clpr.Execute(ClipType.ctUnion, solution,
                  PolyFillType.pftPositive, PolyFillType.pftPositive);
            }
            else
            {
                IntRect r = Clipper.GetBounds(m_destPolys);
                List<IntPoint> outer = new List<IntPoint>(4);

                outer.Add(new IntPoint(r.left - 10, r.bottom + 10));
                outer.Add(new IntPoint(r.right + 10, r.bottom + 10));
                outer.Add(new IntPoint(r.right + 10, r.top - 10));
                outer.Add(new IntPoint(r.left - 10, r.top - 10));

                clpr.AddPath(outer, PolyType.ptSubject, true);
                clpr.ReverseSolution = true;
                clpr.Execute(ClipType.ctUnion, solution, PolyFillType.pftNegative, PolyFillType.pftNegative);
                if (solution.Count > 0) solution.RemoveAt(0);
            }
        }
        //------------------------------------------------------------------------------

        public void Execute(ref PolyTree solution, double delta)
        {
            solution.Clear();
            FixOrientations();
            DoOffset(delta);

            //now clean up 'corners' ...
            Clipper clpr = new Clipper();
            clpr.AddPaths(m_destPolys, PolyType.ptSubject, true);
            if (delta > 0)
            {
                clpr.Execute(ClipType.ctUnion, solution,
                  PolyFillType.pftPositive, PolyFillType.pftPositive);
            }
            else
            {
                IntRect r = Clipper.GetBounds(m_destPolys);
                List<IntPoint> outer = new List<IntPoint>(4);

                outer.Add(new IntPoint(r.left - 10, r.bottom + 10));
                outer.Add(new IntPoint(r.right + 10, r.bottom + 10));
                outer.Add(new IntPoint(r.right + 10, r.top - 10));
                outer.Add(new IntPoint(r.left - 10, r.top - 10));

                clpr.AddPath(outer, PolyType.ptSubject, true);
                clpr.ReverseSolution = true;
                clpr.Execute(ClipType.ctUnion, solution, PolyFillType.pftNegative, PolyFillType.pftNegative);
                //remove the outer PolyNode rectangle ...
                if (solution.ChildCount == 1 && solution.Childs[0].ChildCount > 0)
                {
                    PolyNode outerNode = solution.Childs[0];
                    solution.Childs.Capacity = outerNode.ChildCount;
                    solution.Childs[0] = outerNode.Childs[0];
                    solution.Childs[0].m_Parent = solution;
                    for (int i = 1; i < outerNode.ChildCount; i++)
                        solution.AddChild(outerNode.Childs[i]);
                }
                else
                    solution.Clear();
            }
        }
        //------------------------------------------------------------------------------

        void OffsetPoint(int j, ref int k, JoinType jointype)
        {
            //cross product ...
            m_sinA = (m_normals[k].X * m_normals[j].Y - m_normals[j].X * m_normals[k].Y);

            if (Math.Abs(m_sinA * m_delta) < 1.0)
            {
                //dot product ...
                double cosA = (m_normals[k].X * m_normals[j].X + m_normals[j].Y * m_normals[k].Y);
                if (cosA > 0) // angle ==> 0 degrees
                {
                    m_destPoly.Add(new IntPoint(Round(m_srcPoly[j].X + m_normals[k].X * m_delta),
                      Round(m_srcPoly[j].Y + m_normals[k].Y * m_delta)));
                    return;
                }
                //else angle ==> 180 degrees   
            }
            else if (m_sinA > 1.0) m_sinA = 1.0;
            else if (m_sinA < -1.0) m_sinA = -1.0;

            if (m_sinA * m_delta < 0)
            {
                m_destPoly.Add(new IntPoint(Round(m_srcPoly[j].X + m_normals[k].X * m_delta),
                  Round(m_srcPoly[j].Y + m_normals[k].Y * m_delta)));
                m_destPoly.Add(m_srcPoly[j]);
                m_destPoly.Add(new IntPoint(Round(m_srcPoly[j].X + m_normals[j].X * m_delta),
                  Round(m_srcPoly[j].Y + m_normals[j].Y * m_delta)));
            }
            else
                switch (jointype)
                {
                    case JoinType.jtMiter:
                        {
                            double r = 1 + (m_normals[j].X * m_normals[k].X +
                              m_normals[j].Y * m_normals[k].Y);
                            if (r >= m_miterLim) DoMiter(j, k, r); else DoSquare(j, k);
                            break;
                        }
                    case JoinType.jtSquare: DoSquare(j, k); break;
                    case JoinType.jtRound: DoRound(j, k); break;
                }
            k = j;
        }
        //------------------------------------------------------------------------------

        internal void DoSquare(int j, int k)
        {
            double dx = Math.Tan(Math.Atan2(m_sinA,
                m_normals[k].X * m_normals[j].X + m_normals[k].Y * m_normals[j].Y) / 4);
            m_destPoly.Add(new IntPoint(
                Round(m_srcPoly[j].X + m_delta * (m_normals[k].X - m_normals[k].Y * dx)),
                Round(m_srcPoly[j].Y + m_delta * (m_normals[k].Y + m_normals[k].X * dx))));
            m_destPoly.Add(new IntPoint(
                Round(m_srcPoly[j].X + m_delta * (m_normals[j].X + m_normals[j].Y * dx)),
                Round(m_srcPoly[j].Y + m_delta * (m_normals[j].Y - m_normals[j].X * dx))));
        }
        //------------------------------------------------------------------------------

        internal void DoMiter(int j, int k, double r)
        {
            double q = m_delta / r;
            m_destPoly.Add(new IntPoint(Round(m_srcPoly[j].X + (m_normals[k].X + m_normals[j].X) * q),
                Round(m_srcPoly[j].Y + (m_normals[k].Y + m_normals[j].Y) * q)));
        }
        //------------------------------------------------------------------------------

        internal void DoRound(int j, int k)
        {
            double a = Math.Atan2(m_sinA,
            m_normals[k].X * m_normals[j].X + m_normals[k].Y * m_normals[j].Y);
            int steps = Math.Max((int)Round(m_StepsPerRad * Math.Abs(a)), 1);

            double X = m_normals[k].X, Y = m_normals[k].Y, X2;
            for (int i = 0; i < steps; ++i)
            {
                m_destPoly.Add(new IntPoint(
                    Round(m_srcPoly[j].X + X * m_delta),
                    Round(m_srcPoly[j].Y + Y * m_delta)));
                X2 = X;
                X = X * m_cos - m_sin * Y;
                Y = X2 * m_sin + Y * m_cos;
            }
            m_destPoly.Add(new IntPoint(
            Round(m_srcPoly[j].X + m_normals[j].X * m_delta),
            Round(m_srcPoly[j].Y + m_normals[j].Y * m_delta)));
        }
        //------------------------------------------------------------------------------
    }
}