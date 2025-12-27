using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class DynamicBodyBridge : MonoBehaviour
{
    #region Editable Fields

    [SerializeField] float _density = 1;
    [SerializeField] Categories _category = Categories.Default;
    [SerializeField] Categories _ignore = Categories.None;

    #endregion

    #region Public Properties

    public PhysicsBody Body { get; private set; }

    #endregion

    #region Physics Body Management

    void CreateBody()
    {
        var bodyDef = PhysicsBodyDefinition.defaultDefinition;
        bodyDef.type = PhysicsBody.BodyType.Dynamic;
        bodyDef.position = transform.position;
        bodyDef.rotation = new PhysicsRotate(transform.eulerAngles.z * Mathf.Deg2Rad);

        Body = PhysicsWorld.defaultWorld.CreateBody(bodyDef);

        var shapeDef = PhysicsShapeDefinition.defaultDefinition;
        shapeDef.density = _density;
        shapeDef.triggerEvents = true;

        var category = new PhysicsMask((int)_category);
        var mask = PhysicsMask.All;
        mask.ResetBit((int)_ignore);
        shapeDef.contactFilter = new PhysicsShape.ContactFilter(category, mask);

        GetComponent<CompositeShapeBuilder>().CreateShapes(Body, shapeDef);
    }

    void SyncTransform()
    {
        var xy = Body.transform.position;
        var z = transform.position.z;

        var rz =Body.transform.rotation.angle * Mathf.Rad2Deg;
        var r = Quaternion.AngleAxis(rz, Vector3.forward);

        transform.SetPositionAndRotation(new Vector3(xy.x, xy.y, z), r);
    }

    #endregion

    #region MonoBehaviour Implementation

    void OnEnable()
    {
        if (!Body.isValid) CreateBody();
        SyncTransform();
    }

    void OnDisable()
    {
        if (Body.isValid) Body.Destroy();
    }

    void FixedUpdate()
      => SyncTransform();

    #endregion
}
