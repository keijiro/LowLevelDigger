using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public static class GeometryCache
{
    public static readonly int MaxSides = PhysicsConstants.MaxPolygonVertices;

    static readonly PolygonGeometry[] Geometries = BuildGeometries();

    public static PolygonGeometry GetRegularPolygon(int sides)
      => Geometries[Mathf.Clamp(sides, 3, MaxSides) - 3];

    static PolygonGeometry[] BuildGeometries()
    {
        var geometries = new PolygonGeometry[MaxSides - 2];
        for (var sides = 3; sides <= MaxSides; ++sides)
        {
            var vertices = new Vector2[sides];
            for (var i = 0; i < sides; ++i)
            {
                var r = Mathf.PI * 2 * i / sides;
                vertices[i] = new Vector2(Mathf.Cos(r), Mathf.Sin(r));
            }
            geometries[sides - 3] = PolygonGeometry.Create(vertices, 0);
        }
        return geometries;
    }
}
