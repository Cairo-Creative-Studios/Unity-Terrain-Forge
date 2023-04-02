using UnityEngine;

namespace TerrainBuilder
{
    /// <summary>
    /// The MeshInfo class is used to store information about a mesh
    /// </summary>
    [ExecuteAlways]
    public class MeshInfo : MonoBehaviour
    {
        public MeshPoint[] meshPoints;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            if (meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = new Mesh();
                meshFilter.sharedMesh.name = "Custom Mesh";
            }

            // Initialize the mesh points
            meshPoints = new MeshPoint[meshFilter.sharedMesh.vertexCount];
            
            for (int i = 0; i < meshPoints.Length; i++)
            {
                meshPoints[i] = new MeshPoint();
                meshPoints[i].x = meshFilter.sharedMesh.vertices[i].x;
                meshPoints[i].y = meshFilter.sharedMesh.vertices[i].y;
                meshPoints[i].z = meshFilter.sharedMesh.vertices[i].z;
                meshPoints[i].normal = meshFilter.sharedMesh.normals[i];
                meshPoints[i].uv = meshFilter.sharedMesh.uv[i];
                meshPoints[i].color = meshFilter.sharedMesh.colors[i];
                meshPoints[i].tangent = meshFilter.sharedMesh.tangents[i];
            }
        }
        
        void Update()
        {
            // Update the mesh if the mesh points have been modified
            if (meshPoints == null || meshPoints.Length != meshFilter.sharedMesh.vertexCount)
            {
                meshPoints = new MeshPoint[meshFilter.sharedMesh.vertexCount];
            }

            if (meshFilter.sharedMesh.vertices.Length != meshPoints.Length)
            {
                meshFilter.sharedMesh.vertices = new Vector3[meshPoints.Length];
            }

            if (meshFilter.sharedMesh.normals.Length != meshPoints.Length)
            {
                meshFilter.sharedMesh.normals = new Vector3[meshPoints.Length];
            }

            if (meshFilter.sharedMesh.uv.Length != meshPoints.Length)
            {
                meshFilter.sharedMesh.uv = new Vector2[meshPoints.Length];
            }

            if (meshFilter.sharedMesh.colors.Length != meshPoints.Length)
            {
                meshFilter.sharedMesh.colors = new Color[meshPoints.Length];
            }

            if (meshFilter.sharedMesh.tangents.Length != meshPoints.Length)
            {
                meshFilter.sharedMesh.tangents = new Vector4[meshPoints.Length];
            }

            for (int i = 0; i < meshPoints.Length; i++)
            {
                if (meshPoints[i] == null)
                {
                    meshPoints[i] = new MeshPoint();
                }

                if (meshPoints[i].modified)
                {
                    meshFilter.sharedMesh.vertices[i] = meshPoints[i].position;
                    meshFilter.sharedMesh.normals[i] = meshPoints[i].normal;
                    meshFilter.sharedMesh.uv[i] = meshPoints[i].uv;
                    meshFilter.sharedMesh.colors[i] = meshPoints[i].color;
                    meshFilter.sharedMesh.tangents[i] = meshPoints[i].tangent;

                    meshPoints[i].modified = false;
                }
            }

            meshFilter.sharedMesh.RecalculateNormals();
            meshFilter.sharedMesh.RecalculateBounds();
        }
        
        public class MeshPoint
        {
            private Vector3 _position;

            public Vector3 position
            {
                get
                {
                    return _position;   
                }
            }
            
            public float x
            {
                get => _position.x;
                set
                {
                    _position.x = value;
                    modified = true;
                }
            }
            public float y
            {
                get => _position.y;
                set
                {
                    _position.y = value;
                    modified = true;
                }
            }

            public float z
            {
                get => _position.z;
                set
                {
                    _position.z = value;
                    modified = true;
                }
            }

            public Vector3 normal;
            public Vector2 uv;
            public Color color;
            public Vector4 tangent;
            public Material material;
            public bool modified;
        }
    }
}