using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevelPhysics2D;

public class ScoopController : MonoBehaviour
{
    #region Editable Fields

    [Space]
    [SerializeField] float _pickerSpring = 2f;
    [SerializeField] float _anchorSpring = 2f;
    [SerializeField] float _rewindSpring = 2f;
    [SerializeField] float _damping = 1;
    [Space]
    [SerializeField] DynamicBodyBridge _scoopBody = null;
    [SerializeField] Transform _scoopTip = null;
    [SerializeField] Transform _scoopRim = null;
    [Space]
    [SerializeField] StaticBodyBridge _anchorBody = null;
    [SerializeField] Transform _anchorPoint = null;
    [Space]
    [SerializeField] Camera _targetCamera = null;

    #endregion

    #region Public Methods

    public void StartRewind()
      => SetAnchorSpring(_rewindSpring);

    public void EndRewind()
      => SetAnchorSpring(_anchorSpring);

    #endregion

    #region MonoBehaviour Implementation

    void Start()
    {
        CreatePickerBody();
        CreatePickerJoint();
        CreateAnchorJoint();
    }

    void OnDestroy()
    {
        _anchorJoint.Destroy();
        _pickerJoint.Destroy();
        _pickerBody.Destroy();
    }

    void Update()
    {
        var pointer = Pointer.current;

        if (pointer.press.isPressed)
        {
            var pos = pointer.position.value;
            var xform = _pickerBody.transform;
            xform.position = _targetCamera.ScreenToWorldPoint(pos);
            _pickerBody.transform = xform;
        }
    }

    #endregion

    #region Physics Handling

    PhysicsBody _pickerBody;
    PhysicsJoint _pickerJoint;
    PhysicsJoint _anchorJoint;

    void CreatePickerBody()
    {
        var bodyDef = PhysicsBodyDefinition.defaultDefinition;
        bodyDef.type = PhysicsBody.BodyType.Kinematic;
        bodyDef.position = _scoopBody.Body.transform.position;
        _pickerBody = PhysicsWorld.defaultWorld.CreateBody(bodyDef);
    }

    void CreatePickerJoint()
    {
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

    void SetAnchorSpring(float frequency)
    {
        var joint = (PhysicsDistanceJoint)_anchorJoint;
        joint.springFrequency = frequency;
        joint.springDamping = _damping;
    }

    #endregion
}
