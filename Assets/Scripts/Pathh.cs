using System.Net.NetworkInformation;
using UnityEngine;

public class Pathh
{
    public Vector3[] lookPoints;
    public Line[] turnBoundaries;
    public int slowDownIndex;

    private Vector3 st;
    public Pathh(Vector3[] waypoints, Vector3 startPos, float turnDist, float stoppingDist)
    {
        st = startPos;
        lookPoints = waypoints;
        turnBoundaries = new Line[waypoints.Length];

        Vector2 previousPoint = V3toV2(startPos);
        for(int i = 0; i < turnBoundaries.Length; i++)
        {
            Vector2 currentPoint = V3toV2(lookPoints[i]);
            Vector2 dir = (currentPoint - previousPoint).normalized;

            Vector2 turnBoundaryPoint = (i == turnBoundaries.Length - 1) ? currentPoint : currentPoint - dir * turnDist;

            turnBoundaries[i] = new Line(turnBoundaryPoint, currentPoint + dir);
            previousPoint = turnBoundaryPoint;
        }

        float distFromEndPoint = 0;
        for(int i = Length - 1; i  >= 1; i--)
        {
            distFromEndPoint += Vector2.Distance(lookPoints[i - 1], lookPoints[i]);
            if(distFromEndPoint > stoppingDist)
            {
                slowDownIndex = i;
                break;
            }
        }

    }

    private Vector2 V3toV2(Vector3 v3) => new Vector2 (v3.x, v3.z);

    public int Length { get { return lookPoints.Length; } }

    public void DrawWithGizmos()
    {
        if (lookPoints == null || turnBoundaries == null) return;

        Vector3 prvPoint = st;
        foreach(Vector3 point in lookPoints)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(point + Vector3.up, Vector3.one * 0.2f);
        }

        foreach (var l in turnBoundaries)
        {
            l.DrawWithGizmos(10);
        }
    }

}
