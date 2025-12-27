using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevelPhysics2D;

public class ScoopController : MonoBehaviour
{
    #region Editable Fields

    [Space]
    [SerializeField] float _pickerSpring = 2f;
    [SerializeField] float _anchorSpring = 2f;
    [SerializeField] float _damping = 1;
    [Space]
    [SerializeField] GameObject _scoopPrefab = null;
    [SerializeField] SpoutPositionProvider _spout = null;
    [Space]
    [SerializeField] StaticBodyBridge _anchorBody = null;
    [SerializeField] Transform _anchorPoint = null;
    [Space]
    [SerializeField] Camera _targetCamera = null;

    #endregion

    #region MonoBehaviour Implementation

    void Start()
    {
        CreatePickerBody();
    }

    void OnDestroy()
    {
        if (_anchorJoint.isValid)
            _anchorJoint.Destroy();

        if (_pickerJoint.isValid)
            _pickerJoint.Destroy();

        if (_pickerBody.isValid)
            _pickerBody.Destroy();
    }

    void Update()
    {
        var pointer = Pointer.current;
        if (pointer == null)
            return;

        if (_pickerBody.isValid)
            UpdatePickerBody(pointer);

        UpdatePickerJoint(pointer);
    }

    #endregion

    #region Physics Handling

    PhysicsBody _pickerBody;
    PhysicsJoint _pickerJoint;
    PhysicsJoint _anchorJoint;
    DynamicBodyBridge _scoopBody;
    Transform _scoopTip;
    Transform _scoopRim;

    public void ThrowScoopInstance()
    {
        if (_anchorJoint.isValid)
            _anchorJoint.Destroy();

        if (_pickerJoint.isValid)
            _pickerJoint.Destroy();

        _scoopBody = null;
        _scoopTip = null;
        _scoopRim = null;
    }

    void CreatePickerBody()
    {
        var bodyDef = PhysicsBodyDefinition.defaultDefinition;
        bodyDef.type = PhysicsBody.BodyType.Kinematic;
        bodyDef.position = transform.position;
        _pickerBody = PhysicsWorld.defaultWorld.CreateBody(bodyDef);
    }

    void CreatePickerJoint()
    {
        if (_scoopBody == null || _scoopTip == null)
            return;

        var jointDef = PhysicsDistanceJointDefinition.defaultDefinition;
        jointDef.bodyA = _scoopBody.Body;
        jointDef.bodyB = _pickerBody;

        var scoopTip = jointDef.bodyA.GetLocalPoint(_scoopTip.position);
        jointDef.localAnchorA = new PhysicsTransform(scoopTip);
        jointDef.localAnchorB = PhysicsTransform.identity;

        jointDef.distance = 0;
        jointDef.enableSpring = true;
        jointDef.springFrequency = _pickerSpring;
        jointDef.springDamping = _damping;

        _pickerJoint = PhysicsWorld.defaultWorld.CreateJoint(jointDef);
    }

    void CreateAnchorJoint()
    {
        if (_scoopBody == null || _scoopRim == null)
            return;

        var jointDef = PhysicsDistanceJointDefinition.defaultDefinition;
        jointDef.bodyA = _scoopBody.Body;
        jointDef.bodyB = _anchorBody.Body;

        var scoopRim = jointDef.bodyA.GetLocalPoint(_scoopRim.position);
        var anchor = jointDef.bodyB.GetLocalPoint(_anchorPoint.position);
        jointDef.localAnchorA = new PhysicsTransform(scoopRim);
        jointDef.localAnchorB = new PhysicsTransform(anchor);

        jointDef.distance = 0;
        jointDef.enableSpring = true;
        jointDef.springFrequency = _anchorSpring;
        jointDef.springDamping = _damping;
        jointDef.collideConnected = true;

        _anchorJoint = PhysicsWorld.defaultWorld.CreateJoint(jointDef);
    }

    void UpdatePickerBody(Pointer pointer)
    {
        var pos = pointer.position.value;
        var xform = _pickerBody.transform;
        xform.position = _targetCamera.ScreenToWorldPoint(pos);
        _pickerBody.transform = xform;
        _pickerBody.linearVelocity = Vector2.zero;
        _pickerBody.angularVelocity = 0f;
    }

    void UpdatePickerJoint(Pointer pointer)
    {
        if (_scoopBody == null)
            return;

        if (pointer.press.wasPressedThisFrame)
        {
            CreatePickerJoint();
        }
        else if (pointer.press.wasReleasedThisFrame && _pickerJoint.isValid)
            _pickerJoint.Destroy();
    }

    public void SpawnScoopInstance()
    {
        EnsureSpout();
        if (_scoopPrefab == null)
            return;

        var position = _spout != null ? _spout.GetPosition() : (Vector2)transform.position;
        var instance = Instantiate(_scoopPrefab, position, transform.rotation);

        _scoopBody = instance.GetComponent<DynamicBodyBridge>();
        _scoopTip = instance.transform.Find("Anchor Tip");
        _scoopRim = instance.transform.Find("Anchor Rim");

        if (_scoopBody == null)
            return;

        CreateAnchorJoint();
    }

    void EnsureSpout()
    {
        if (_spout == null)
            _spout = GetComponent<SpoutPositionProvider>();
    }

    #endregion
}
