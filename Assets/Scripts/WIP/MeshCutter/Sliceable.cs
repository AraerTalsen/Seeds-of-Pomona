using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

//Properties of a slicable object. Any object that should be able to be sliced needs this script attached.
public class Sliceable : MonoBehaviour
{
    [SerializeField]
    private bool _isSolid = true;

    [SerializeField]
    private bool _reverseWindTriangles = false;

    [SerializeField]
    private bool _useGravity = false;

    [SerializeField]
    private bool _shareVertices = false;

    [SerializeField]
    private bool _smoothVertices = false;

     [SerializeField]
    private bool _isRoot = false;

    private Vector3? _rootPos = null;

    private Dictionary<Vector3, VertexObject> _vertexNetwork = new Dictionary<Vector3, VertexObject>();

    public bool IsSolid
    {
        get
        {
            return _isSolid;
        }
        set
        {
            _isSolid = value;
        }
    }

    public bool ReverseWireTriangles
    {
        get
        {
            return _reverseWindTriangles;
        }
        set
        {
            _reverseWindTriangles = value;
        }
    }

    public bool UseGravity 
    {
        get
        {
            return _useGravity;
        }
        set
        {
            _useGravity = value;
        }
    }

    public bool ShareVertices 
    {
        get
        {
            return _shareVertices;
        }
        set
        {
            _shareVertices = value;
        }
    }

    public bool SmoothVertices 
    {
        get
        {
            return _smoothVertices;
        }
        set
        {
            _smoothVertices = value;
        }
    }

    public bool IsRoot
    {
        get
        {
            return _isRoot;
        }
        set
        {
            _isRoot = value;
        }
    }

    public Dictionary<Vector3, VertexObject> VertexNetwork
    {
        get
        {
            return _vertexNetwork;
        }
        set
        {
            _vertexNetwork = value;
        }
    }

    public Vector3? RootPos
    {
        get
        {
            return _rootPos;
        }
        set
        {
            _rootPos = value != null ? (Vector3)value : null;
        }
    }
}
