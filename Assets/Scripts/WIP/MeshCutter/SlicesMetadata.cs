using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    /// <summary>
    /// The side of the mesh
    /// </summary>
    public enum MeshSide
    {
        Positive = 0,
        Negative = 1
    }

    /// <summary>
    /// An object used to manage the positive and negative side mesh data for a sliced object
    /// </summary>
    class SlicesMetadata
    {
        private Mesh _positiveSideMesh;
        private List<Vector3> _positiveSideVertices;
        private List<int> _positiveSideTriangles;
        private List<Vector2> _positiveSideUvs;
        private List<Vector3> _positiveSideNormals;

        private Mesh _negativeSideMesh;
        private List<Vector3> _negativeSideVertices;
        private List<int> _negativeSideTriangles;
        private List<Vector2> _negativeSideUvs;
        private List<Vector3> _negativeSideNormals;

        private readonly List<Vector3> _pointsAlongPlane;
        private Plane _plane;
        private Mesh _mesh;
        private Color[] _cols = {Color.red, Color.blue, Color.green, Color.black, Color.cyan, Color.gray, Color.white, Color.yellow, Color.magenta};
        private int _currentColor = 0;
        private Vector3[] _planeConstraint;
        private Dictionary<Vector3, VertexObject> _vertexNetwork;
        private Vector3 _startBounds;
        private Vector3 _endBounds;
        private List<Vector3> _intersectingPoints = new List<Vector3>();
        private Dictionary<Vector3, bool> _loggedVertices = new Dictionary<Vector3, bool>();
        private Vector3? _rootPos;
        private bool _rootSide = false;
        private bool _isSolid;
        private bool _useSharedVertices = false;
        private bool _smoothVertices = false;
        private bool _createReverseTriangleWindings = false;
    

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

        public Mesh PositiveSideMesh
        {
            get
            {
                if (_positiveSideMesh == null)
                {
                    _positiveSideMesh = new Mesh();
                }

                SetMeshData(MeshSide.Positive);
                return _positiveSideMesh;
            }
        }

        public Mesh NegativeSideMesh
        {
            get
            {
                if (_negativeSideMesh == null)
                {
                    _negativeSideMesh = new Mesh();
                }

                SetMeshData(MeshSide.Negative);

                return _negativeSideMesh;
            }
        }

        public List<Vector3> IntersectingPoints
        {
            get
            {
                return _intersectingPoints;
            }
            set
            {
                _intersectingPoints = value;
            }
        }

        public bool RootSide
        {
            get
            {
                return _rootSide;
            }
            set
            {
                _rootSide = value;
            }
        }

        public SlicesMetadata(Plane plane, Mesh mesh, Dictionary<Vector3, VertexObject> vertexNetwork, Vector3[] planeConstraint, Vector3? rootPos, bool isSolid, bool createReverseTriangleWindings, bool shareVertices, bool smoothVertices)
        {
            _positiveSideTriangles = new List<int>();
            _positiveSideVertices = new List<Vector3>();
            _negativeSideTriangles = new List<int>();
            _negativeSideVertices = new List<Vector3>();
            _positiveSideUvs = new List<Vector2>();
            _negativeSideUvs = new List<Vector2>();
            _positiveSideNormals = new List<Vector3>();
            _negativeSideNormals = new List<Vector3>();
            _pointsAlongPlane = new List<Vector3>();
            _plane = plane;
            _mesh = mesh;
            _planeConstraint = planeConstraint;
            _rootPos = rootPos != null ? (Vector3)rootPos : null;
            _vertexNetwork = vertexNetwork;
            _isSolid = isSolid;
            _createReverseTriangleWindings = createReverseTriangleWindings;
            _useSharedVertices = shareVertices;
            _smoothVertices = smoothVertices;

            Vector3 start = planeConstraint[0], end = planeConstraint[1], mod = planeConstraint[2];
            _startBounds = start - mod;
            _endBounds = end - mod;

            ComputeNewMeshes();
        }

        /// <summary>
        /// Add the mesh data to the correct side and calulate normals
        /// </summary>
        private void AddTrianglesNormalAndUvs(MeshSide side, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
        {
            if (side == MeshSide.Positive)
            {
                AddTrianglesNormalsAndUvs(ref _positiveSideVertices, ref _positiveSideTriangles, ref _positiveSideNormals, ref _positiveSideUvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
            }
            else
            {
                AddTrianglesNormalsAndUvs(ref _negativeSideVertices, ref _negativeSideTriangles, ref _negativeSideNormals, ref _negativeSideUvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
            }
        }


        /// <summary>
        /// Adds the vertices to the mesh sets the triangles in the order that the vertices are provided.
        /// If shared vertices is false vertices will be added to the list even if a matching vertex already exists
        /// Does not compute normals
        /// </summary>
        private void AddTrianglesNormalsAndUvs(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector3> normals, ref List<Vector2> uvs, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
        {
            int tri1Index = vertices.IndexOf(vertex1);

            if (addFirst)
            {
                ShiftTriangleIndeces(ref triangles);
            }

            //If the vertex already exists we just add a triangle reference to it, if not add the vert to the list and then add the tri index
            if (tri1Index > -1 && shareVertices)
            {                
                triangles.Add(tri1Index);
            }
            else
            {
                if (normal1 == null)
                {
                    normal1 = ComputeNormal(vertex1, vertex2, vertex3);                    
                }

                int? i = null;
                if (addFirst)
                {
                    i = 0;
                }

                AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex1, (Vector3)normal1, uv1, i);
            }

            int tri2Index = vertices.IndexOf(vertex2);

            if (tri2Index > -1 && shareVertices)
            {
                triangles.Add(tri2Index);
            }
            else
            {
                if (normal2 == null)
                {
                    normal2 = ComputeNormal(vertex2, vertex3, vertex1);
                }
                
                int? i = null;
                
                if (addFirst)
                {
                    i = 1;
                }

                AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex2, (Vector3)normal2, uv2, i);
            }

            int tri3Index = vertices.IndexOf(vertex3);

            if (tri3Index > -1 && shareVertices)
            {
                triangles.Add(tri3Index);
            }
            else
            {               
                if (normal3 == null)
                {
                    normal3 = ComputeNormal(vertex3, vertex1, vertex2);
                }

                int? i = null;
                if (addFirst)
                {
                    i = 2;
                }

                AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex3, (Vector3)normal3, uv3, i);
            }
        }

        private void AddVertNormalUv(ref List<Vector3> vertices, ref List<Vector3> normals, ref List<Vector2> uvs, ref List<int> triangles, Vector3 vertex, Vector3 normal, Vector2 uv, int? index)
        {
            if (index != null)
            {
                int i = (int)index;
                vertices.Insert(i, vertex);
                uvs.Insert(i, uv);
                normals.Insert(i, normal);
                triangles.Insert(i, i);
            }
            else
            {
                vertices.Add(vertex);
                normals.Add(normal);
                uvs.Add(uv);
                triangles.Add(vertices.IndexOf(vertex));
            }
        }

        private void ShiftTriangleIndeces(ref List<int> triangles)
        {
            for (int j = 0; j < triangles.Count; j += 3)
            {
                triangles[j] += + 3;
                triangles[j + 1] += 3;
                triangles[j + 2] += 3;
            }
        }

        /// <summary>
        /// Will render the inside of an object
        /// This is heavy as it duplicates all the vertices and creates opposite winding direction
        /// </summary>
        private void AddReverseTriangleWinding()
        {
            int positiveVertsStartIndex = _positiveSideVertices.Count;
            //Duplicate the original vertices
            _positiveSideVertices.AddRange(_positiveSideVertices);
            _positiveSideUvs.AddRange(_positiveSideUvs);
            _positiveSideNormals.AddRange(FlipNormals(_positiveSideNormals));

            int numPositiveTriangles = _positiveSideTriangles.Count;

            //Add reverse windings
            for (int i = 0; i < numPositiveTriangles; i += 3)
            {
                _positiveSideTriangles.Add(positiveVertsStartIndex + _positiveSideTriangles[i]);
                _positiveSideTriangles.Add(positiveVertsStartIndex + _positiveSideTriangles[i + 2]);
                _positiveSideTriangles.Add(positiveVertsStartIndex + _positiveSideTriangles[i + 1]);
            }

            int negativeVertextStartIndex = _negativeSideVertices.Count;
            //Duplicate the original vertices
            _negativeSideVertices.AddRange(_negativeSideVertices);
            _negativeSideUvs.AddRange(_negativeSideUvs);
            _negativeSideNormals.AddRange(FlipNormals(_negativeSideNormals));

            int numNegativeTriangles = _negativeSideTriangles.Count;

            //Add reverse windings
            for (int i = 0; i < numNegativeTriangles; i += 3)
            {
                _negativeSideTriangles.Add(negativeVertextStartIndex + _negativeSideTriangles[i]);
                _negativeSideTriangles.Add(negativeVertextStartIndex + _negativeSideTriangles[i + 2]);
                _negativeSideTriangles.Add(negativeVertextStartIndex + _negativeSideTriangles[i + 1]);
            }
        }

        /// <summary>
        /// Join the points along the plane to the halfway point
        /// </summary>
        private void JoinPointsAlongPlane()
        {
            Vector3 halfway = GetHalfwayPoint(out float distance);

            for (int i = 0; i < _pointsAlongPlane.Count; i += 2)
            {
                Vector3 firstVertex;
                Vector3 secondVertex;

                firstVertex = _pointsAlongPlane[i];
                secondVertex = _pointsAlongPlane[i + 1];

                Vector3 normal3 = ComputeNormal(halfway, secondVertex, firstVertex);
                normal3.Normalize();

                var direction = Vector3.Dot(normal3, _plane.normal);

                if(direction > 0)
                {                                        
                    AddTrianglesNormalAndUvs(MeshSide.Positive, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, true);
                    AddTrianglesNormalAndUvs(MeshSide.Negative, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, true);
                }
                else
                {
                    AddTrianglesNormalAndUvs(MeshSide.Positive, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, true);
                    AddTrianglesNormalAndUvs(MeshSide.Negative, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, true);
                }               
            }
        }

        /// <summary>
        /// For all the points added along the plane cut, get the half way between the first and furthest point
        /// </summary>
        /// <returns></returns>
        private Vector3 GetHalfwayPoint(out float distance)
        {
            if(_pointsAlongPlane.Count > 0)
            {
                Vector3 firstPoint = _pointsAlongPlane[0];
                Vector3 furthestPoint = Vector3.zero;
                distance = 0f;

                foreach (Vector3 point in _pointsAlongPlane)
                {
                    float currentDistance = 0f;
                    currentDistance = Vector3.Distance(firstPoint, point);

                    if (currentDistance > distance)
                    {
                        distance = currentDistance;
                        furthestPoint = point;
                    }
                }

                return Vector3.Lerp(firstPoint, furthestPoint, 0.5f);
            }
            else
            {
                distance = 0;
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Setup the mesh object for the specified side
        /// </summary>
        /// <param name="side"></param>
        private void SetMeshData(MeshSide side)
        {
            if (side == MeshSide.Positive)
            {
                _positiveSideMesh.vertices = _positiveSideVertices.ToArray();
                _positiveSideMesh.triangles = _positiveSideTriangles.ToArray();
                _positiveSideMesh.normals = _positiveSideNormals.ToArray();
                _positiveSideMesh.uv = _positiveSideUvs.ToArray();
            }
            else
            {
                _negativeSideMesh.vertices = _negativeSideVertices.ToArray();
                _negativeSideMesh.triangles = _negativeSideTriangles.ToArray();
                _negativeSideMesh.normals = _negativeSideNormals.ToArray();
                _negativeSideMesh.uv = _negativeSideUvs.ToArray();                
            }
        }

        /// <summary>
        /// Compute the positive and negative meshes based on the plane and mesh
        /// </summary>
        private void ComputeNewMeshes()
        {
            int[] meshTriangles = _mesh.triangles;
            Vector3[] meshVerts = _mesh.vertices;
            Vector3[] meshNormals = _mesh.normals;
            Vector2[] meshUvs = _mesh.uv;
            bool debug = false;
            float closestVertToRootDist = Mathf.Infinity;

            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                //Uncomment and set to the index you want to debug at
                /*if(i == 30) debug = true;
                else if(i == 33)
                {
                    bool b = _loggedVertices[Vector3.zero];
                }*/
                //We need the verts in order so that we know which way to wind our new mesh triangles.
                Vector3 vert1 = meshVerts[meshTriangles[i]];
                int vert1Index = Array.IndexOf(meshVerts, vert1);
                Vector2 uv1 = meshUvs[vert1Index];
                Vector3 normal1 = meshNormals[vert1Index];
                bool vert1Side = _plane.GetSide(vert1);

                Vector3 vert2 = meshVerts[meshTriangles[i + 1]];
                int vert2Index = Array.IndexOf(meshVerts, vert2);
                Vector2 uv2 = meshUvs[vert2Index];
                Vector3 normal2 = meshNormals[vert2Index];
                bool vert2Side = _plane.GetSide(vert2);

                Vector3 vert3 = meshVerts[meshTriangles[i + 2]];
                bool vert3Side = _plane.GetSide(vert3);
                int vert3Index = Array.IndexOf(meshVerts, vert3);
                Vector3 normal3 = meshNormals[vert3Index];
                Vector2 uv3 = meshUvs[vert3Index];

                if ((vert1Side == vert2Side && vert2Side == vert3Side))
                {
                    //Add the relevant triangle
                    MeshSide side = IsVertexPositive(vert1, debug, i, ref closestVertToRootDist) ? MeshSide.Positive : MeshSide.Negative;
                    AddTrianglesNormalAndUvs(side, vert1, normal1, uv1, vert2, normal2, uv2, vert3, normal3, uv3, true, false);
                }
                else
                {
                    //we need the two points where the plane intersects the triangle.
                    Vector3 intersection1;
                    Vector3 intersection2;
                    
                    Vector2 intersection1Uv;
                    Vector2 intersection2Uv;

                    MeshSide v1side = IsVertexPositive(vert1, debug, i, ref closestVertToRootDist) ? MeshSide.Positive : MeshSide.Negative;
                    MeshSide v2side = IsVertexPositive(vert2, debug, i, ref closestVertToRootDist) ? MeshSide.Positive : MeshSide.Negative;
                    MeshSide v3side = IsVertexPositive(vert3, debug, i, ref closestVertToRootDist) ? MeshSide.Positive : MeshSide.Negative;

                    //vert 1 and 2 are on the same side
                    if (vert1Side == vert2Side)
                    {
                        //Cast a ray from v2 to v3 and from v3 to v1 to get the intersections                       
                        intersection1 = GetRayPlaneIntersectionPointAndUv(vert2, uv2, vert3, uv3, out intersection1Uv);
                        intersection2 = GetRayPlaneIntersectionPointAndUv(vert3, uv3, vert1, uv1, out intersection2Uv);

                        bool withinPlaneContraint = IsPointOnIntersectingLine(intersection1, _startBounds, _endBounds) || 
                        IsPointOnIntersectingLine(intersection2, _startBounds, _endBounds);

                        //Only consider the edge of triangle intersceted (sliced) if the slice is under the cut-line drawn by the player
                        if(withinPlaneContraint)
                        {
                            _intersectingPoints.Add(intersection1);
                            _intersectingPoints.Add(intersection2);
                            //Add the positive or negative triangles
                            AddTrianglesNormalAndUvs(v1side, vert1, null, uv1, vert2, null, uv2, intersection1, null, intersection1Uv, _useSharedVertices, false);
                            AddTrianglesNormalAndUvs(v1side, vert1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, _useSharedVertices, false);

                            AddTrianglesNormalAndUvs(v3side, intersection1, null, intersection1Uv, vert3, null, uv3, intersection2, null, intersection2Uv, _useSharedVertices, false);
                        }
                        else{
                            AddTrianglesNormalAndUvs(v1side, vert1, normal1, uv1, vert2, normal2, uv2, vert3, normal3, uv3, true, false);
                        }

                    }
                    //vert 1 and 3 are on the same side
                    else if (vert1Side == vert3Side)
                    {
                        //Cast a ray from v1 to v2 and from v2 to v3 to get the intersections                       
                        intersection1 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert2, uv2, out intersection1Uv);
                        intersection2 = GetRayPlaneIntersectionPointAndUv(vert2, uv2, vert3, uv3, out intersection2Uv);

                        bool withinPlaneContraint = IsPointOnIntersectingLine(intersection1, _startBounds, _endBounds) || 
                        IsPointOnIntersectingLine(intersection2, _startBounds, _endBounds);

                        if(withinPlaneContraint)
                        {
                            _intersectingPoints.Add(intersection1);
                            _intersectingPoints.Add(intersection2);
                            //Add the positive triangles
                            AddTrianglesNormalAndUvs(v1side, vert1, null, uv1, intersection1, null, intersection1Uv, vert3, null, uv3, _useSharedVertices, false);
                            AddTrianglesNormalAndUvs(v1side, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, vert3, null, uv3, _useSharedVertices, false);

                            AddTrianglesNormalAndUvs(v2side, intersection1, null, intersection1Uv, vert2, null, uv2, intersection2, null, intersection2Uv, _useSharedVertices, false);
                        }
                        else{
                           AddTrianglesNormalAndUvs(v1side, vert1, normal1, uv1, vert2, normal2, uv2, vert3, normal3, uv3, true, false);
                        }
                    }
                    //Vert1 is alone
                    else
                    {
                        //Cast a ray from v1 to v2 and from v1 to v3 to get the intersections                       
                        intersection1 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert2, uv2, out intersection1Uv);
                        intersection2 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert3, uv3, out intersection2Uv);

                        bool withinPlaneContraint = IsPointOnIntersectingLine(intersection1, _startBounds, _endBounds) || 
                        IsPointOnIntersectingLine(intersection2, _startBounds, _endBounds);

                        if(withinPlaneContraint)
                        {
                            _intersectingPoints.Add(intersection1);
                            _intersectingPoints.Add(intersection2);
                            AddTrianglesNormalAndUvs(v1side, vert1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, _useSharedVertices, false);

                            AddTrianglesNormalAndUvs(v2side, intersection1, null, intersection1Uv, vert2, null, uv2, vert3, null, uv3, _useSharedVertices, false);
                            AddTrianglesNormalAndUvs(v2side, intersection1, null, intersection1Uv, vert3, null, uv3, intersection2, null, intersection2Uv, _useSharedVertices, false);
                        }
                        else{
                            AddTrianglesNormalAndUvs(v1side, vert1, normal1, uv1, vert2, normal2, uv2, vert3, normal3, uv3, true, false);
                        }
                    }

                    //Add the newly created points on the plane.
                    _pointsAlongPlane.Add(intersection1);
                    _pointsAlongPlane.Add(intersection2);
                }
            }

            //If the object is solid, join the new points along the plane otherwise do the reverse winding
            if (_isSolid)
            {
                JoinPointsAlongPlane();
            }
            else if (_createReverseTriangleWindings)
            {
                AddReverseTriangleWinding();
            }

            if (_smoothVertices)
            {
                SmoothVertices();
            }
        }

        /// <summary>
        /// Checks to see if vertex is within the range player drew to slice
        /// </summary>
        private bool IsPointOnIntersectingLine(Vector3 vertex, Vector3 pointA, Vector3 pointB)
        {
            Vector3 min = Vector3.Min(pointA, pointB);
            Vector3 max = Vector3.Max(pointA, pointB);
            //Debug.Log(min + " " + max + " " + vertex);
            return vertex.x >= min.x && vertex.x <= max.x
                   && vertex.y >= min.y && vertex.y <= max.y
                   && vertex.z >= min.z && vertex.z <= max.z;
        }

        //Map through vertex neighbors to see if the current vertex is on the positive or negative slice of the object. The vertex finds whether
        //it is positive or negative by following the shortest path of connected verteces it can find to the cut-line on the mesh. The vertex is
        //then listed as positive or negative depending on which side of the intersecting plane the last vertex before the intersected edge is on.
        private bool IsVertexPositive(Vector3 vertex, bool debug, int iteration, ref float closestVertToRootDist)
        {
            List<Vector3> refChain = new();
            List<Vector3> deadVertices = new();
            List<int> routes = new();
            bool inDict, intersectionFound = false;
            Vector3 nearestIntersectionPoint;
            bool isCloser = false;
            
            for(int i = 0; i < _vertexNetwork.Count; i++)
            {
                if(_rootPos != null)
                {
                    float dist = Vector3.Distance(vertex, (Vector3)_rootPos);
                    if(dist < closestVertToRootDist)
                    {
                        closestVertToRootDist = dist;
                        isCloser = true;
                    }
                }
                


                inDict = _loggedVertices.ContainsKey(vertex);

                if(inDict && !intersectionFound) 
                {
                    Vector3? prev = null;
                    for(int j = 0; j < refChain.Count; j++)
                    {
                        if(!_loggedVertices.ContainsKey(refChain[j]))
                        {
                            _loggedVertices.Add(refChain[j], _loggedVertices[vertex]);
                            if(debug)
                            {
                                if(prev != null)
                                {
                                    Debug.DrawLine((Vector3)prev, refChain[j] + new Vector3(0, 1, -8), _loggedVertices[vertex] ? Color.yellow : Color.magenta, Mathf.Infinity);
                                }
                                
                                GameObject point = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/OriginalPoints"), refChain[j] + new Vector3(0, 1, -8), Quaternion.identity);
                                point.GetComponent<MeshRenderer>().material.color = _loggedVertices[vertex] ? Color.yellow : Color.magenta;//_cols[_currentColor];
                                if(j == refChain.Count - 1)
                                {
                                    point.transform.localScale *= 2;
                                }
                                prev = refChain[j] + new Vector3(0, 1, -8);
                            }
                        } 
                    }
                    if(debug)
                    {
                        if(refChain.Count == 1)
                            Debug.DrawLine((Vector3)refChain[0] + new Vector3(0, 1, -8), vertex + new Vector3(0, 1, -8), _loggedVertices[vertex] ? Color.yellow : Color.magenta, Mathf.Infinity);
                            _currentColor = _currentColor >= _cols.Length - 1 ? 0 : _currentColor + 1;
                    }
                    
                    if(isCloser) _rootSide = _loggedVertices[vertex];
                    return _loggedVertices[vertex];
                }
                else
                {
                    nearestIntersectionPoint = GetClosestPoint(vertex);
                    if(intersectionFound)
                    {
                        bool side = _plane.GetSide(vertex);
                        if(debug)
                        {
                            Debug.Log("Team " + iteration + " side is " + side + " last vertex is " + vertex + " color is " + _cols[_currentColor]);
                            Debug.Log("Found new intersection");
                        }
                        
                        Vector3? prev = null;
                        for(int j = 0; j < refChain.Count; j++)
                        {
                             _loggedVertices.Add(refChain[j], side);
                             if(debug)
                             {
                                if(prev != null)
                                {
                                    Debug.DrawLine((Vector3)prev, refChain[j] + new Vector3(0, 1, -8), side ? Color.yellow : Color.magenta, Mathf.Infinity);
                                }
                            
                                GameObject point = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/OriginalPoints"), refChain[j] + new Vector3(0, 1, -8), Quaternion.identity);
                                point.GetComponent<MeshRenderer>().material.color = side ? Color.yellow : Color.magenta;//_cols[_currentColor];
                                if(j == refChain.Count - 1)
                                {
                                    point.transform.localScale *= 2;
                                }
                                prev = refChain[j] + new Vector3(0, 1, -8);
                            }
                        }
                        if(debug)
                        {
                            Debug.DrawLine((Vector3)refChain[refChain.Count - 1] + new Vector3(0, 1, -8), vertex + new Vector3(0, 1, -8), side ? Color.yellow : Color.magenta, Mathf.Infinity);
                         _currentColor = _currentColor >= _cols.Length - 1 ? 0 : _currentColor + 1;
                        }
                        if(isCloser) _rootSide = side;
                        return side;
                    }
                    else
                    {
                        refChain.Add(vertex);
                        vertex = ChooseNewPath(vertex, nearestIntersectionPoint, ref refChain, ref deadVertices, ref routes, debug, out int remainingRoutes, out bool deadPath, out intersectionFound);
                        if(!deadPath) routes.Add(remainingRoutes);
                        continue;
                    }
                }
            }
            return true;
        }

        //Find the closest world point on a line to the current vertex (origin)
        private Vector3 GetClosestPoint(Vector3 origin)
        {
            Vector3 linePoint1 = _startBounds;
            Vector3 linePoint2 = _endBounds;

            // Get the direction vector of the line
            Vector3 lineDirection = linePoint2 - linePoint1;
            
            // Vector from linePoint1 to the origin
            Vector3 toOrigin = origin - linePoint1;

            // Project toOrigin onto the line direction
            float t = Vector3.Dot(toOrigin, lineDirection) / Vector3.Dot(lineDirection, lineDirection);

            // Closest point on the line
            Vector3 closestPoint = linePoint1 + t * lineDirection;

            return closestPoint;
        }

        //Find next vertex to add to the path by looking at the current vertex's neighbors. The priority list is:
        //1. If the neighbor is the last vertex before the cut-line
        //2. If the neighbor is in the dictionary
        //3. If the neighbor is the closest of all other neighbors to the cut-line
        private Vector3 ChooseNewPath(Vector3 vertex, Vector3 destination, ref List<Vector3> refChain, ref List<Vector3> deadVertices, ref List<int> routes, bool debug, out int remainingRoutes, out bool deadPath, out bool intersectionFound)
        {
            ///Add check to see if neighbor is in dictionary as second priority to having an intersection
            List<Vector3> neighbors = _vertexNetwork[vertex].Neighbors;
            List<Vector3> refChainCopy = refChain;
            List<Vector3> deadVerticesCopy = deadVertices;
            intersectionFound = false;            

            neighbors = neighbors.Where(v => !refChainCopy.Contains(v) && !deadVerticesCopy.Contains(v)).ToList();

            if(neighbors.Count == 0)
            {
                deadVertices.Add(PopFromRefChain(ref refChain, ref routes, false));
                deadPath = true;
                for(int i = refChain.Count - 1; i >= 0; i--)
                {
                    neighbors = _vertexNetwork[refChain[i]].Neighbors;
                    neighbors = neighbors.Where(v => !refChainCopy.Contains(v) && !deadVerticesCopy.Contains(v)).ToList();
                    
                    if(neighbors.Count == 0)
                    {
                        deadVertices.Add(PopFromRefChain(ref refChain, ref routes, true));
                    }
                    else
                    {
                        vertex = refChain[i];
                        break;
                    }
                }
            }
            else
            {
                deadPath = false;
            }
            
            float shortestDist = Mathf.Infinity;
            Vector3 nearestPoint = Vector3.zero;
            foreach(Vector3 n in neighbors)
            {
                intersectionFound = HasIntersection(vertex, n);
                if(intersectionFound)
                {
                    nearestPoint = n;
                    break;
                }
                float currentDist = Vector3.Distance(n, destination);
                if(currentDist < shortestDist)
                {
                    shortestDist = currentDist;
                    nearestPoint = n;  
                }
            }
            remainingRoutes = neighbors.Count - 1;
            return nearestPoint;
        }

        private Vector3 PopFromRefChain(ref List<Vector3> refChain, ref List<int> routes, bool popRoute)
        {
            int lastIndex = refChain.Count - 1;
            Vector3 p = refChain[lastIndex];
            refChain.RemoveAt(lastIndex);
            if(popRoute) routes.RemoveAt(lastIndex);
            return p;
        }

        //Checks between two verteces to see if their edge is intersected by the cut-line
        private bool HasIntersection(Vector3 vert1, Vector3 vert2)
        {
            Ray r = new Ray(vert1, vert2 - vert1);
            if(_plane.Raycast(r, out float dist))
            {
                if(Vector3.Distance(vert1, vert2) >= dist)
                {
                    bool intersected = IsPointOnIntersectingLine(r.GetPoint(dist), _startBounds, _endBounds);
                    return intersected;
                } 
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// Casts a reay from vertex1 to vertex2 and gets the point of intersection with the plan, calculates the new uv as well.
        /// </summary>
        private Vector3 GetRayPlaneIntersectionPointAndUv(Vector3 vertex1, Vector2 vertex1Uv, Vector3 vertex2, Vector2 vertex2Uv, out Vector2 uv)
        {
            float distance = GetDistanceRelativeToPlane(vertex1, vertex2, out Vector3 pointOfIntersection);
            float totalDistance = Vector3.Distance(vertex1, vertex2);
            float nomralizedDeifference = distance / totalDistance;
            uv = InterpolateUvs(vertex1Uv, vertex2Uv, nomralizedDeifference);
            return pointOfIntersection;
        }

        /// <summary>
        /// Computes the distance based on the plane.
        /// </summary>
        private float GetDistanceRelativeToPlane(Vector3 vertex1, Vector3 vertex2, out Vector3 pointOfintersection)
        {
            Ray ray = new Ray(vertex1, (vertex2 - vertex1));
            _plane.Raycast(ray, out float distance);
            pointOfintersection = ray.GetPoint(distance);
            return distance;
        }

        /// <summary>
        /// Get a uv between the two provided uvs by the distance.
        /// </summary>
        private Vector2 InterpolateUvs(Vector2 uv1, Vector2 uv2, float distance)
        {
            Vector2 uv = Vector2.Lerp(uv1, uv2, distance);
            return uv;
        }

        /// <summary>
        /// Gets the point perpendicular to the face defined by the provided vertices        
        //https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
        /// </summary>
        private Vector3 ComputeNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Vector3 side1 = vertex2 - vertex1;
            Vector3 side2 = vertex3 - vertex1;

            Vector3 normal = Vector3.Cross(side1, side2);

            return normal;
        }

        /// <summary>
        /// Reverese the normals in a given list
        /// </summary>
        private List<Vector3> FlipNormals(List<Vector3> currentNormals)
        {
            List<Vector3> flippedNormals = new List<Vector3>();

            foreach (Vector3 normal in currentNormals)
            {
                flippedNormals.Add(-normal);
            }

            return flippedNormals;
        }

        //
        private void SmoothVertices()
        {
            DoSmoothing(ref _positiveSideVertices, ref _positiveSideNormals, ref _positiveSideTriangles);
            DoSmoothing(ref _negativeSideVertices, ref _negativeSideNormals, ref _negativeSideTriangles);
        }

        private void DoSmoothing(ref List<Vector3> vertices, ref List<Vector3> normals, ref List<int> triangles)
        {
            normals.ForEach(x =>
            {
                x = Vector3.zero;
            });

            for (int i = 0; i < triangles.Count; i += 3)
            {
                int vertIndex1 = triangles[i];
                int vertIndex2 = triangles[i + 1];
                int vertIndex3 = triangles[i + 2];

                Vector3 triangleNormal = ComputeNormal(vertices[vertIndex1], vertices[vertIndex2], vertices[vertIndex3]);

                normals[vertIndex1] += triangleNormal;
                normals[vertIndex2] += triangleNormal;
                normals[vertIndex3] += triangleNormal;
            }

            normals.ForEach(x =>
            {
                x.Normalize();
            });
        }
    }
}
