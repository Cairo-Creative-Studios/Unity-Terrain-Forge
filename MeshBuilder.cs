using UnityEngine;

namespace TerrainBuilder
{
    /// <summary>
    /// The MeshBuilder class is used to create meshes
    /// </summary>
    public static class MeshBuilder
    {
        /// <summary>
        /// Creates a mesh from a set of points
        /// </summary>
        /// <param name="points"></param>
        /// <param name="material"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static MeshInfo CreateMesh(Vector3[] points, Material material, Texture2D texture)
        {
            GameObject meshObject = new GameObject("Custom Mesh");

            Mesh mesh = new Mesh();
            mesh.vertices = points;
            mesh.triangles = GenerateTriangles(points.Length);
            mesh.uv = GenerateUVs(points);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            if (texture != null)
            {
                meshRenderer.material.mainTexture = texture;
            }

            return meshObject.AddComponent<MeshInfo>();
        }

        /// <summary>
        /// Generates triangles for a mesh based on the number of points
        /// </summary>
        /// <param name="numPoints"></param>
        /// <returns></returns>
        private static int[] GenerateTriangles(int numPoints)
        {
            int[] triangles = new int[(numPoints - 2) * 3];

            int index = 0;
            for (int i = 1; i < numPoints - 1; i++)
            {
                triangles[index++] = 0;
                triangles[index++] = i;
                triangles[index++] = i + 1;
            }

            return triangles;
        }

        /// <summary>
        /// Generates UVs for a mesh based on the X and Z values of the points
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static Vector2[] GenerateUVs(Vector3[] points)
        {
            Vector2[] uvs = new Vector2[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                uvs[i] = new Vector2(points[i].x, points[i].z);
            }

            return uvs;
        }
        
        public static Mesh TransformPointsInRange(Mesh mesh, Vector3 minPoint, Vector3 maxPoint, Vector3 transformAmount, float falloff)
        {
            Vector3[] vertices = mesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].x >= minPoint.x && vertices[i].x <= maxPoint.x &&
                    vertices[i].y >= minPoint.y && vertices[i].y <= maxPoint.y &&
                    vertices[i].z >= minPoint.z && vertices[i].z <= maxPoint.z)
                {
                    float distance = Vector3.Distance(vertices[i], minPoint);
                    float percent = distance / falloff;
                    percent = Mathf.Clamp01(percent);
                    vertices[i] += transformAmount * percent;
                }
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
        
        /// <summary>
        /// Modifies the mesh of a game object
        /// </summary>
        /// <param name="go"></param>
        /// <param name="minRange"></param>
        /// <param name="maxRange"></param>
        /// <returns></returns>
        public static GameObject ModifyMesh(GameObject go, Vector3 minRange, Vector3 maxRange, float falloff)
        {
            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf != null)
            {
                Mesh mesh = mf.mesh;
                mesh.vertices = TransformPointsInRange(mesh, minRange, maxRange, Vector3.up, falloff).vertices;
                mesh.RecalculateNormals();
                mf.mesh = mesh;
            }
            return go;
        }
        
        /// <summary>
        /// Modifies the mesh of a mesh info object
        /// </summary>
        /// <param name="meshInfo"></param>
        /// <param name="minRange"></param>
        /// <param name="maxRange"></param>
        /// <param name="falloff"></param>
        /// <returns></returns>
        public static MeshInfo ModifyMesh(MeshInfo meshInfo, Vector3 minRange, Vector3 maxRange, float falloff)
        {
            Mesh mesh = meshInfo.GetComponent<MeshFilter>().mesh;
            mesh.vertices = TransformPointsInRange(mesh, minRange, maxRange, Vector3.up, falloff).vertices;
            mesh.RecalculateNormals();
            meshInfo.GetComponent<MeshFilter>().mesh = mesh;
            return meshInfo;
        }
    }
}