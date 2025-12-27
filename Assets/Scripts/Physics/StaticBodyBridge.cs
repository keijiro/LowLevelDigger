using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class StaticBodyBridge : MonoBehaviour
{
    #region Editable Fields

    [SerializeField] bool _isKinematic = false;
    [SerializeField] Categories _category = Categories.Default;
    [SerializeField] Categories _ignore = Categories.None;

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
        bodyDef.type = _isKinematic ? PhysicsBody.BodyType.Kinematic :
                                      PhysicsBody.BodyType.Static;
        bodyDef.position = transform.position;

        Body = PhysicsWorld.defaultWorld.CreateBody(bodyDef);

        var shapeDef = PhysicsShapeDefinition.defaultDefinition;

        var category = new PhysicsMask((int)_category);
        var mask = PhysicsMask.All;
        mask.ResetBit((int)_ignore);
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

    void Start()
    {
        CreateBody();
        ApplyTransform();
        CacheTransform();
    }

    void OnDestroy()
      => Body.Destroy();

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
