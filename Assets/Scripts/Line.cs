using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Line
{
    private float VerticalLineGradient = 1e5f;
    private float gradient, y_intercept;

    public bool FromSide = false;
    private Vector2 center;

    public Line(Vector2 PointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = (PointOnLine.x - pointPerpendicularToLine.x);
        float dy = (PointOnLine.y - pointPerpendicularToLine.y);

        gradient = (dy == 0) ? VerticalLineGradient : -dx / dy;
        y_intercept = PointOnLine.y - gradient * PointOnLine.x;

        FromSide = GetSide2(pointPerpendicularToLine);
        center = PointOnLine;
    }

    private Vector2 V3toV2(Vector3 v3) => new Vector2(v3.x, v3.z);
    public bool GetSide3(Vector3 p) => Vector2.Dot( V3toV2(p) - new Vector2(0, y_intercept), new Vector2(-gradient, 1)) >= 0;

    public bool GetSide2(Vector2 p) => Vector2.Dot(p - new Vector2(0, y_intercept), new Vector2(-gradient, 1)) >= 0;
    public bool HasCrossedLine(Vector3 p) => GetSide3(p) == FromSide;

    public void DrawWithGizmos(float length)
    {
        Gizmos.color = Color.yellow;

        Vector3 pos = new Vector3(center.x, 0, center.y) + Vector3.up;
        Vector3 lineDirection = new Vector3(1, 0, gradient).normalized;

        Gizmos.DrawLine(pos - lineDirection * length / 2, pos + lineDirection * length / 2);
    }

}
