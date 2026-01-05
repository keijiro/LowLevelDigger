using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public sealed class DirtRenderer : MonoBehaviour
{
    #region Editable Fields

    [SerializeField] DirtManager _source = null;

    #endregion

    #region MonoBehaviour Implementation

    void Start()
    {
        CreateMesh(_source.BodyCount);
        CreateTexture(_source.BodyCount);
        SetUpMaterialProperties(_source.BodyCount);
    }

    void OnDestroy()
    {
        Destroy(_mesh);
        Destroy(_texture);
    }

    void OnDisable()
    {
        if (_data.IsCreated) _data.Dispose();
    }

    void LateUpdate()
      => UpdateBodyData(_source.BodySpan);

    #endregion

    #region Resource Setup

    Mesh _mesh;
    Texture2D _texture;
    NativeArray<Vector4> _data;

    void CreateMesh(int bodyCount)
    {
        var vertices = new Vector3[bodyCount * 4];
        var indicies = new int[bodyCount * 6];

        for (var i = 0; i < bodyCount; i++)
        {
            var v = i * 4;
            var t = i * 6;

            vertices[v + 0] = new Vector3(-0.5f, -0.5f, 0);
            vertices[v + 1] = new Vector3(-0.5f,  0.5f, 0);
            vertices[v + 2] = new Vector3( 0.5f,  0.5f, 0);
            vertices[v + 3] = new Vector3( 0.5f, -0.5f, 0);

            indicies[t + 0] = v + 0;
            indicies[t + 1] = v + 1;
            indicies[t + 2] = v + 2;
            indicies[t + 3] = v + 2;
            indicies[t + 4] = v + 3;
            indicies[t + 5] = v + 0;
        }

        _mesh = new Mesh { name = "Dirt Quad Mesh" };
        _mesh.vertices = vertices;
        _mesh.SetIndices(indicies, MeshTopology.Triangles, 0);
        _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

        GetComponent<MeshFilter>().sharedMesh = _mesh;
    }

    void CreateTexture(int bodyCount)
    {
        _texture = new Texture2D(bodyCount, 1, TextureFormat.RGBAFloat, false);
        _texture.filterMode = FilterMode.Point;
        _texture.wrapMode = TextureWrapMode.Clamp;

        _data = new NativeArray<Vector4>(bodyCount, Allocator.Persistent);
    }

    void SetUpMaterialProperties(int bodyCount)
    {
        var props = new MaterialPropertyBlock();
        props.SetTexture("_BodyTex", _texture);
        props.SetInt("_BodyCount", _source.BodyCount);
        GetComponent<MeshRenderer>().SetPropertyBlock(props);
    }

    #endregion

    #region Data Upload

    void UpdateBodyData(ReadOnlySpan<PhysicsBody> bodies)
    {
        for (var i = 0; i < bodies.Length; i++)
        {
            var body = bodies[i];
            var xform = body.transform;
            _data[i] = new Vector4(xform.position.x,
                                   xform.position.y,
                                   xform.rotation.angle,
                                   body.enabled ? 1 : 0);
        }

        _texture.LoadRawTextureData(_data);
        _texture.Apply(false);
    }

    #endregion
}
