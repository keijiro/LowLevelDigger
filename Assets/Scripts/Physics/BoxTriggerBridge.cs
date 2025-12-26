using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class BoxTriggerBridge : MonoBehaviour
{
    #region Editable Fields

    [SerializeField] Vector2 _size = Vector2.one;
    [SerializeField] Categories _detect = Categories.Default;

    #endregion

    #region Transform Cache and Checker

    (Vector2 position, float rotation, Vector2 scale) _lastXform;

    Vector2 Scale2D => transform.lossyScale;

    Vector2 ScaledSize => Vector2.Scale(_size, Scale2D);

    bool IsPositionChanged
      => _lastXform.position != (Vector2)transform.position;

    bool IsRotationChanged
      => !Mathf.Approximately(Mathf.DeltaAngle(_lastXform.rotation, transform.eulerAngles.z), 0);

    void CacheTransform()
    {
        _lastXform.position = transform.position;
        _lastXform.rotation = transform.eulerAngles.z;
        _lastXform.scale = Scale2D;
    }

    #endregion

    #region Physics Body Management

    PhysicsBody _body;

    void CreateBody()
    {
        var bodyDef = PhysicsBodyDefinition.defaultDefinition;
        bodyDef.position = transform.position;

        _body = PhysicsWorld.defaultWorld.CreateBody(bodyDef);

        var geometry = PolygonGeometry.CreateBox(ScaledSize, 0);
        var definition = PhysicsShapeDefinition.defaultDefinition;
        definition.isTrigger = true;
        definition.triggerEvents = true;

        var category = new PhysicsMask((int)Categories.Trigger);
        var mask = new PhysicsMask((int)_detect);
        definition.contactFilter = new PhysicsShape.ContactFilter(category, mask);

        _body.CreateShape(geometry, definition);
    }

    void ApplyTransform()
    {
        var rot = new PhysicsRotate(transform.eulerAngles.z * Mathf.Deg2Rad);
        var xform = new PhysicsTransform(transform.position, rot);
        _body.SetAndWriteTransform(xform);
    }

    #endregion

    #region MonoBehaviour Implementation

    void Start()
    {
        CreateBody();
        ApplyTransform();
        CacheTransform();
    }

    void OnDestroy()
      => _body.Destroy();

    void FixedUpdate()
    {
        if (Scale2D != _lastXform.scale)
        {
            // Recreate the body if the scale has changed.
            _body.Destroy();
            CreateBody();
            ApplyTransform();
            CacheTransform();
        }
        else if (IsPositionChanged || IsRotationChanged)
        {
            // Update the body's transform if position or rotation has changed.
            ApplyTransform();
            CacheTransform();
        }
    }

    #endregion
}
