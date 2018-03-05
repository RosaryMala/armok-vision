using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Poly2Tri;

public class ComplexPoly
{
    Dictionary<Point2D, List<PolygonPoint>> PolygonSegmentsByEnd = new Dictionary<Point2D, List<PolygonPoint>>();
    Dictionary<Point2D, List<PolygonPoint>> PolygonSegmentsByStart = new Dictionary<Point2D, List<PolygonPoint>>();
    PolygonSet CompletedPolygons = new PolygonSet();

    public void Clear()
    {
        PolygonSegmentsByEnd.Clear();
        PolygonSegmentsByStart.Clear();
        CompletedPolygons.Polygons.Clear();
    }

    public PolygonSet Polygons
    {
        get
        {
            return DeNestHoles(CompletedPolygons);
        }
    }

    public void AddLineSegment(params PolygonPoint[] values)
    {
        AddLineSegment(new List<PolygonPoint>(values));
    }

    public void AddLineSegment(List<PolygonPoint> segment)
    {
        if (segment.Count < 2)
            return; //can't add a single point.

        //if both the start and end match, then we're bridging two together.
        if (PolygonSegmentsByEnd.ContainsKey(segment[0]) && PolygonSegmentsByStart.ContainsKey(segment[segment.Count - 1]))
        {
            //get the segment that this one completes.
            var startSegment = PolygonSegmentsByEnd[segment[0]];
            var endSegment = PolygonSegmentsByStart[segment[segment.Count - 1]];
            //remove both from both collections.
            PolygonSegmentsByEnd.Remove(segment[0]);
            PolygonSegmentsByStart.Remove(segment[segment.Count - 1]);

            if (startSegment == endSegment) // We're completing a closed polygon.
            {
                //remove the shared point between both
                startSegment.RemoveAt(startSegment.Count - 1);
                //join them together
                startSegment.AddRange(segment);
                //remove one of the now same two points that are at either ends.
                startSegment.RemoveAt(startSegment.Count - 1);
                Polygon newPoly = new Polygon(startSegment);
                newPoly.Simplify();
                InsertIntoSet(CompletedPolygons, newPoly);
            }
            else
            {
                //remove the shared point between both
                startSegment.RemoveAt(startSegment.Count - 1);
                //join them together
                startSegment.AddRange(segment);
                //remove one of the now same two points that are at either ends.
                startSegment.RemoveAt(startSegment.Count - 1);
                //join them together
                startSegment.AddRange(endSegment);

                PolygonSegmentsByEnd[startSegment[startSegment.Count - 1]] = startSegment;
                PolygonSegmentsByStart[startSegment[0]] = startSegment;
            }
        }
        //If we have a segment who's end matches our beginning, we join them.
        else if (PolygonSegmentsByEnd.ContainsKey(segment[0]))
        {
            //get the segment we need to join to
            var oldSegment = PolygonSegmentsByEnd[segment[0]];

            //Remove the old segment from that list.
            //It will need a new address.
            PolygonSegmentsByEnd.Remove(segment[0]);

            //remove the last point from the old segment,
            //because it's the same as the first point from the new one.
            oldSegment.RemoveAt(oldSegment.Count - 1);

            //now join them together.
            oldSegment.AddRange(segment);

            //finally add the newly joined segment to the list of ends.
            PolygonSegmentsByEnd[oldSegment[oldSegment.Count - 1]] = oldSegment;

            //The other list doesn't need changeing because we keep the start point.
        }
        //likewise, if we have a segment who's beginning matches our end.
        else if (PolygonSegmentsByStart.ContainsKey(segment[segment.Count - 1]))
        {
            var oldSegment = PolygonSegmentsByStart[segment[segment.Count - 1]];
            PolygonSegmentsByStart.Remove(segment[segment.Count - 1]);
            segment.RemoveAt(segment.Count - 1);
            oldSegment.InsertRange(0, segment);
            PolygonSegmentsByStart[oldSegment[0]] = oldSegment;
        }
        //finally, if there's no existing connection, just add it to both lists.
        else
        {
            PolygonSegmentsByStart[segment[0]] = segment;
            PolygonSegmentsByEnd[segment[segment.Count - 1]] = segment;
        }
    }

    static void InsertIntoSet(PolygonSet set, Polygon poly)
    {
        //first test to see if any of the existing polygons are inside our input.
        for (int i = set.Polygons.Count - 1; i >= 0; i--)
        {
            if (TryInsertHole(poly, set.Polygons[i]))
                set.Polygons.RemoveAt(i); //we go from the end, so the indices remain valid.
        }
        foreach (var item in set.Polygons)
        {
            if (TryInsertHole(item, poly))
            {
                return; //nothing more to do.
            }
        }
        set.Add(poly);
    }


    static bool TryInsertHole(Polygon parent, Polygon hole)
    {
        //Can't go in.
        if (!parent.IsPointInside(hole[0]))
            return false;

        if (parent.Holes != null)
            foreach (var item in parent.Holes)
            {
                if (TryInsertHole(item, hole))
                    return true;
            }
        //it doesn't fit into any of the daughter holes.
        parent.AddHole(hole);
        return true;
    }

    static PolygonSet DeNestHoles(PolygonSet set)
    {
        PolygonSet output = new PolygonSet();
        foreach (var polygon in set.Polygons)
        {
            output.Polygons.AddRange(DeNestHoles(polygon));
        }
        return output;
    }

    static List<Polygon> DeNestHoles(Polygon poly)
    {
        List<Polygon> output = new List<Polygon>();
        output.Add(poly);
        if (poly.Holes != null)
            foreach (var hole in poly.Holes)
            {
                if (hole.Holes != null)
                {
                    for (int i = hole.Holes.Count - 1; i >= 0; i--)
                    {
                        output.AddRange(DeNestHoles(hole.Holes[i]));
                        hole.Holes.RemoveAt(i);
                    }
                }
            }
        return output;
    }

    public void DrawGizmos(Transform transform, Color partialColor, Color completeColor, float height)
    {
        foreach (var item in PolygonSegmentsByEnd)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(new Vector3(item.Key.Xf, height, item.Key.Yf)), 0.3f);
            Gizmos.color = partialColor;
            DrawPolygonSegment(transform, item.Value, height);
        }
        foreach (var item in PolygonSegmentsByStart)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(new Vector3(item.Key.Xf, height, item.Key.Yf)), 0.3f);
        }
        Gizmos.color = completeColor;
        foreach (var item in CompletedPolygons.Polygons)
        {
            DrawPolygon(transform, item, height);
        }
    }


    private void DrawPolygon(Transform transform, Polygon poly, float height)
    {
        DrawPolygonSegment(transform, poly.Points, height, true);
        if (poly.Holes != null)
        {
            foreach (var item in poly.Holes)
            {
                DrawPolygon(transform, item, height);
            }
        }
    }

    private void DrawPolygonSegment(Transform transform, IList<TriangulationPoint> segment, float height, bool closed = false)
    {
        for (int i = 0; i < segment.Count - 1; i++)
        {
            if (i > 0 || closed)
                Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(new Vector3(segment[i].Xf, height, segment[i].Yf)), 0.05f);
            Gizmos.DrawLine(
                transform.localToWorldMatrix.MultiplyPoint(new Vector3(segment[i].Xf, height, segment[i].Yf)),
                transform.localToWorldMatrix.MultiplyPoint(new Vector3(segment[i + 1].Xf, height, segment[i + 1].Yf)));
        }
        if (closed)
        {
            Gizmos.DrawLine(
                transform.localToWorldMatrix.MultiplyPoint(new Vector3(segment[segment.Count - 1].Xf, height, segment[segment.Count - 1].Yf)), 
                transform.localToWorldMatrix.MultiplyPoint(new Vector3(segment[0].Xf, height, segment[0].Yf)));
            Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(new Vector3(segment[segment.Count - 1].Xf, height, segment[segment.Count - 1].Yf)), 0.05f);
        }
    }


    private void DrawPolygonSegment(Transform transform, IList<PolygonPoint> segment, float height, bool closed = false)
    {
        for (int i = 0; i < segment.Count - 1; i++)
        {
            if (i > 0 || closed)
                Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(new Vector3(segment[i].Xf, height, segment[i].Yf)), 0.05f);
            Gizmos.DrawLine(
                transform.localToWorldMatrix.MultiplyPoint(new Vector3(segment[i].Xf, height, segment[i].Yf)),
                transform.localToWorldMatrix.MultiplyPoint(new Vector3(segment[i + 1].Xf, height, segment[i + 1].Yf)));
        }
        if (closed)
        {
            Gizmos.DrawLine(
                transform.localToWorldMatrix.MultiplyPoint(new Vector3(segment[segment.Count - 1].Xf, height, segment[segment.Count - 1].Yf)), 
                transform.localToWorldMatrix.MultiplyPoint(new Vector3(segment[0].Xf, height, segment[0].Yf)));
            Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(new Vector3(segment[segment.Count - 1].Xf, height, segment[segment.Count - 1].Yf)), 0.05f);
        }
    }
}
