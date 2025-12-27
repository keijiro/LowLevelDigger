using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class TriggerBridge : MonoBehaviour
{
    #region Editable Fields

    [SerializeField] Categories _detect = Categories.Default;

    #endregion

    #region Public Properties

    public PhysicsBody Body { get; private set; }

    #endregion

    #region Transform Cache and Checker

    (Vector2 position, float rotation) _lastXform;

    bool IsPositionChanged
      => _lastXform.position != (Vector2)transform.position;

    bool IsRotationChanged
      => !Mathf.Approximately(Mathf.DeltaAngle(_lastXform.rotation, transform.eulerAngles.z), 0);

    void CacheTransform()
    {
        _lastXform.position = transform.position;
        _lastXform.rotation = transform.eulerAngles.z;
    }

    #endregion

    #region Physics Body Management

    void CreateBody()
    {
        var bodyDef = PhysicsBodyDefinition.defaultDefinition;
        bodyDef.position = transform.position;
        bodyDef.rotation = new PhysicsRotate(transform.eulerAngles.z * Mathf.Deg2Rad);

        Body = PhysicsWorld.defaultWorld.CreateBody(bodyDef);

        var shapeDef = PhysicsShapeDefinition.defaultDefinition;
        shapeDef.isTrigger = true;
        shapeDef.triggerEvents = true;

        var category = new PhysicsMask((int)Categories.Trigger);
        var mask = new PhysicsMask((int)_detect);
        shapeDef.contactFilter = new PhysicsShape.ContactFilter(category, mask);

        GetComponent<CompositeShapeBuilder>().CreateShapes(Body, shapeDef);
    }

    void ApplyTransform()
    {
        var rot = new PhysicsRotate(transform.eulerAngles.z * Mathf.Deg2Rad);
        var xform = new PhysicsTransform(transform.position, rot);
        Body.SetAndWriteTransform(xform);
    }

    #endregion

    #region MonoBehaviour Implementation

    void OnEnable()
    {
        if (!Body.isValid) CreateBody();
        ApplyTransform();
        CacheTransform();
    }

    void OnDestroy()
    {
        if (Body.isValid) Body.Destroy();
    }

    void FixedUpdate()
    {
        if (IsPositionChanged || IsRotationChanged)
        {
            ApplyTransform();
            CacheTransform();
        }
    }

    #endregion
}
