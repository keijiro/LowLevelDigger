using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class DirtManager : MonoBehaviour
{
    #region Editable Fields

    [Space]
    [SerializeField] float _radius = 0.2f;
    [SerializeField] float _density = 1;
    [Space]
    [SerializeField] float _pourRate = 512;
    [SerializeField] int _bodyCount = 1024;
    [Space]
    [SerializeField] SpoutPositionProvider _spout = null;
    [SerializeField] float _recycleY = -10;

    #endregion

    #region Public Members

    public int BodyCount => _bodyCount;
    public ReadOnlySpan<PhysicsBody> BodySpan => _bodyPool;

    public void RequestInjection()
      => CollectInactiveBodies();

    #endregion

    #region MonoBehaviour Implementation

    void Start()
      => CreatePool();

    void OnDestroy()
      => DestroyPool();

    void Update()
    {
        UpdateInjection();
        UpdateRecycling();
    }

    #endregion

    #region Body Pool Lifecycle

    PhysicsBody[] _bodyPool;

    void CreatePool()
    {
        _bodyPool = new PhysicsBody[_bodyCount];

        var bodyDef = PhysicsBodyDefinition.defaultDefinition;
        bodyDef.type = PhysicsBody.BodyType.Dynamic;

        var shapeDef = PhysicsShapeDefinition.defaultDefinition;
        shapeDef.density = _density;

        var categories = new PhysicsMask((int)Categories.Dirt);
        var contacts = PhysicsMask.All;
        shapeDef.contactFilter = new PhysicsShape.ContactFilter(categories, contacts);

        var geometry = new CircleGeometry { radius = _radius };

        for (var i = 0; i < _bodyCount; ++i)
        {
            var body = PhysicsWorld.defaultWorld.CreateBody(bodyDef);
            body.enabled = false;
            body.CreateShape(geometry, shapeDef);
            _bodyPool[i] = body;
        }
    }

    void DestroyPool()
    {
        foreach (var body in _bodyPool) body.Destroy();
        _bodyPool = null;
    }

    #endregion

    #region Body Injection

    Queue<PhysicsBody> _pendingBodies;
    float _pourAccumulator;

    void CollectInactiveBodies()
      => _pendingBodies =
           new Queue<PhysicsBody>(_bodyPool.Where(body => !body.enabled));

    void UpdateInjection()
    {
        if (_pendingBodies == null) return;

        _pourAccumulator += _pourRate * Time.deltaTime;

        while (_pourAccumulator >= 1 && _pendingBodies.Count > 0)
        {
            _pourAccumulator -= 1;

            var xform = new PhysicsTransform(_spout.GetPosition());

            var body = _pendingBodies.Dequeue();
            body.enabled = true;
            body.SetAndWriteTransform(xform);
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }

        if (_pendingBodies.Count == 0) _pendingBodies = null;
    }

    #endregion

    #region Body Recycling

    void UpdateRecycling()
    {
        foreach (var body in _bodyPool)
        {
            if (!body.enabled) continue;
            if (body.transform.position.y >= _recycleY) continue;
            body.enabled = false;
        }
    }

    #endregion
}
