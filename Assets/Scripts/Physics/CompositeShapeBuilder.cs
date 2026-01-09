using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

[ExecuteInEditMode]
public sealed class CompositeShapeBuilder : MonoBehaviour
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
        foreach (var shape in _shapes)
        {
            switch (shape.Type)
            {
                case ShapeType.Circle:  body.CreateShape(CreateCircle (shape), def); break;
                case ShapeType.Polygon: body.CreateShape(CreatePolygon(shape), def); break;
                case ShapeType.Box:     body.CreateShape(CreateBox    (shape), def); break;
            }
        }
    }

    #endregion

    #region Debug Visualization

    #if UNITY_EDITOR
    void OnRenderObject()
    {
        if (Application.isPlaying) return;

        foreach (var shape in _shapes)
        {
            switch (shape.Type)
            {
                case ShapeType.Circle:  DrawGeometry(CreateCircle (shape)); break;
                case ShapeType.Polygon: DrawGeometry(CreatePolygon(shape)); break;
                case ShapeType.Box:     DrawGeometry(CreateBox    (shape)); break;
            }
        }
    }
    #endif

    #endregion

    #region Private Helpers

    PhysicsWorld World
      => PhysicsWorld.defaultWorld;

    PhysicsTransform ConvertedTransform
      => PhysicsMath.ToPhysicsTransform(transform, World.transformPlane);

    PhysicsTransform ExtractTransform(in ShapeElement shape)
      => new PhysicsTransform
           (shape.Center, new PhysicsRotate(shape.Rotation * Mathf.Deg2Rad));

    PolygonGeometry BuildRegularPolygonGeometry(in ShapeElement shape)
    {
        var geo = GeometryCache.GetRegularPolygon(shape.Sides);
        var scale = new Vector3(shape.Radius, shape.Radius, 1);
        return geo.Transform(Matrix4x4.Scale(scale), true);
    }

    CircleGeometry CreateCircle(in ShapeElement shape)
      => new CircleGeometry { center = shape.Center, radius = shape.Radius };

    PolygonGeometry CreatePolygon(in ShapeElement shape)
      => BuildRegularPolygonGeometry(shape).Transform(ExtractTransform(shape));

    PolygonGeometry CreateBox(in ShapeElement shape)
      => PolygonGeometry.CreateBox(shape.Scale, 0, ExtractTransform(shape));

    void DrawGeometry(in CircleGeometry geo)
      => World.DrawGeometry(geo, ConvertedTransform, Gizmos.color);

    void DrawGeometry(in PolygonGeometry geo)
      => World.DrawGeometry(geo, ConvertedTransform, Gizmos.color);

    #endregion
}
