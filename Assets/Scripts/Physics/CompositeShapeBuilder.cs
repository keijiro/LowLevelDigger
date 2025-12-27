using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class CompositeShapeBuilder : MonoBehaviour
{
    #region Types

    public enum ShapeType { Circle, Polygon, Box }

    [System.Serializable]
    public struct ShapeElement
    {
        public ShapeType Type;
        public Vector2 Center;
        public float Rotation;
        public float Radius;
        public int Sides;
        public Vector2 Scale;
    }

    #endregion

    #region Editable Fields

    [SerializeField] ShapeElement[] _shapes = null;

    #endregion

    #region Public Methods

    public void CreateShapes(PhysicsBody body)
      => CreateShapes(body, PhysicsShapeDefinition.defaultDefinition);

    public void CreateShapes(PhysicsBody body, PhysicsShapeDefinition def)
    {
        foreach (var shape in _shapes) CreateShape(body, def, shape);
    }

    #endregion

    #region Shape Creation

    const int MaxPolygonSides = 10;

    void CreateShape(PhysicsBody body, PhysicsShapeDefinition def, ShapeElement element)
    {
        switch (element.Type)
        {
            case ShapeType.Circle: CreateCircle(body, def, element); break;
            case ShapeType.Polygon: CreatePolygon(body, def, element); break;
            case ShapeType.Box: CreateBox(body, def, element); break;
        }
    }

    void CreateCircle(PhysicsBody body, PhysicsShapeDefinition def, ShapeElement element)
    {
        var circle = new CircleGeometry
          { center = element.Center, radius = element.Radius };
        body.CreateShape(circle, def);
    }

    void CreatePolygon(PhysicsBody body, PhysicsShapeDefinition def, ShapeElement element)
    {
        var sides = Mathf.Clamp(element.Sides, 3, MaxPolygonSides);
        var vertices = new Vector2[sides];
        var rot = element.Rotation * Mathf.Deg2Rad;
        for (var i = 0; i < sides; ++i)
        {
            var r = Mathf.PI * 2 * i / sides;
            var offs = new Vector2(Mathf.Cos(r), Mathf.Sin(r)) * element.Radius;
            vertices[i] = element.Center + RotateVector(offs, rot);
        }
        body.CreateShape(PolygonGeometry.Create(vertices, 0), def);
    }

    void CreateBox(PhysicsBody body, PhysicsShapeDefinition def, ShapeElement element)
    {
        var size = new Vector2(Mathf.Abs(element.Scale.x), Mathf.Abs(element.Scale.y));
        var rot = new PhysicsRotate(element.Rotation * Mathf.Deg2Rad);
        var xform = new PhysicsTransform(element.Center, rot);
        body.CreateShape(PolygonGeometry.CreateBox(size, 0, xform), def);
    }

    #endregion

    #region Gizmos

    void OnDrawGizmos()
    {
        if (Application.isPlaying) return;

        var prevMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        foreach (var shape in _shapes) DrawShapeGizmo(shape);

        Gizmos.matrix = prevMatrix;
    }

    void DrawShapeGizmo(ShapeElement element)
    {
        switch (element.Type)
        {
            case ShapeType.Circle: Gizmos.DrawWireSphere(element.Center, element.Radius); break;
            case ShapeType.Polygon: DrawPolygonGizmo(element.Center, element.Radius, element.Sides, element.Rotation); break;
            case ShapeType.Box: DrawBoxGizmo(element.Center, element.Scale, element.Rotation); break;
        }
    }

    void DrawBoxGizmo(Vector2 center, Vector2 scale, float rotation)
    {
        var size = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), 0f);
        var rot = Quaternion.Euler(0f, 0f, rotation);
        var prevMatrix = Gizmos.matrix;
        Gizmos.matrix = Gizmos.matrix * Matrix4x4.TRS(center, rot, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = prevMatrix;
    }

    void DrawPolygonGizmo(Vector2 center, float radius, int sides, float rotation)
    {
        var count = Mathf.Clamp(sides, 3, MaxPolygonSides);
        var rot = rotation * Mathf.Deg2Rad;
        var prev = center + RotateVector(new Vector2(1, 0) * radius, rot);

        for (var i = 1; i <= count; ++i)
        {
            var angle = Mathf.PI * 2 * i / count;
            var next = center + RotateVector(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius, rot);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }

    Vector2 RotateVector(Vector2 v, float radians)
    {
        var (sin, cos) = (Mathf.Sin(radians), Mathf.Cos(radians));
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    #endregion
}
