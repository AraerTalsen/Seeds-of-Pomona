using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts
{
    class Slicer
    {
        /// <summary>
        /// Slice the object by the plane 
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="objectToCut"></param>
        /// <returns></returns>
        public static GameObject[] Slice(Plane plane, GameObject objectToCut, Vector3[] planeConstraint, out List<Vector3> intersectingPoints)
        {            
            //Get the current mesh and its verts and tris
            Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;
            var a = mesh.GetSubMesh(0);
            Sliceable sliceable = objectToCut.GetComponent<Sliceable>();

            if(sliceable == null)
            {
                throw new NotSupportedException("Cannot slice non sliceable object, add the sliceable script to the object or inherit from sliceable to support slicing");
            }
            
            //Create left and right slice of hollow object
            SlicesMetadata slicesMeta = new SlicesMetadata(plane, mesh, sliceable.VertexNetwork, planeConstraint, sliceable.RootPos, sliceable.IsSolid, sliceable.ReverseWireTriangles, sliceable.ShareVertices, sliceable.SmoothVertices);
            intersectingPoints = slicesMeta.IntersectingPoints;

            GameObject positiveObject = CreateMeshGameObject(objectToCut, slicesMeta.RootSide, sliceable.RootPos);
            positiveObject.name = string.Format("{0}_positive", objectToCut.name);

            GameObject negativeObject = CreateMeshGameObject(objectToCut, !slicesMeta.RootSide, sliceable.RootPos);
            negativeObject.name = string.Format("{0}_negative", objectToCut.name);

            var positiveSideMeshData = slicesMeta.PositiveSideMesh;
            var negativeSideMeshData = slicesMeta.NegativeSideMesh;

            positiveObject.GetComponent<MeshFilter>().mesh = positiveSideMeshData;
            negativeObject.GetComponent<MeshFilter>().mesh = negativeSideMeshData;

            positiveObject.GetComponent<Sliceable>().VertexNetwork = SetVertexNeighborsForMesh.SetVertexNeighbors(positiveSideMeshData);
            negativeObject.GetComponent<Sliceable>().VertexNetwork = SetVertexNeighborsForMesh.SetVertexNeighbors(negativeSideMeshData);

            SetupCollidersAndRigidBodys(ref positiveObject, positiveSideMeshData, sliceable.UseGravity, positiveObject.GetComponent<Sliceable>().IsRoot);
            SetupCollidersAndRigidBodys(ref negativeObject, negativeSideMeshData, sliceable.UseGravity, negativeObject.GetComponent<Sliceable>().IsRoot);

            return new GameObject[] { positiveObject, negativeObject};
        }        

        /// <summary>
        /// Creates the default mesh game object.
        /// </summary>
        /// <param name="originalObject">The original object.</param>
        /// <returns></returns>
        private static GameObject CreateMeshGameObject(GameObject originalObject, bool isRoot, Vector3? rootPos)
        {
            var originalMaterial = originalObject.GetComponent<MeshRenderer>().materials;

            GameObject meshGameObject = new GameObject();
            Sliceable originalSliceable = originalObject.GetComponent<Sliceable>();

            meshGameObject.AddComponent<MeshFilter>();
            meshGameObject.AddComponent<MeshRenderer>();
            Sliceable sliceable = meshGameObject.AddComponent<Sliceable>();

            sliceable.IsSolid = originalSliceable.IsSolid;
            sliceable.ReverseWireTriangles = originalSliceable.ReverseWireTriangles;
            sliceable.UseGravity = originalSliceable.UseGravity;

            //The root half is a stationary object, the other piece is the "slice"
            if(rootPos != null)
            {
                sliceable.IsRoot = isRoot;
                sliceable.RootPos = isRoot ? originalSliceable.RootPos : null;
            }
            else
            {
                sliceable.IsRoot = false;
                sliceable.RootPos = null;
            }

            meshGameObject.GetComponent<MeshRenderer>().materials = originalMaterial;

            meshGameObject.transform.localScale = originalObject.transform.localScale;
            meshGameObject.transform.rotation = originalObject.transform.rotation;
            meshGameObject.transform.position = originalObject.transform.position;

            meshGameObject.tag = originalObject.tag;

            return meshGameObject;
        }

        /// <summary>
        /// Add mesh collider and rigid body to game object. The root should remain in place, whereas the slice should fall
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="mesh"></param>
        private static void SetupCollidersAndRigidBodys(ref GameObject gameObject, Mesh mesh, bool useGravity, bool isRoot)
        {                     
            //Future Edit: Only the non-root needs a rigidbody
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = !isRoot;
            meshCollider.isTrigger = !isRoot;

            var rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = !isRoot ? useGravity : false;
            rb.isKinematic = isRoot;
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX;
        }
    }
}
