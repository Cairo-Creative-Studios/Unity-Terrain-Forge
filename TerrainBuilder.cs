using UnityEngine;

namespace TerrainBuilder
{
    /// <summary>
    /// The TerrainBuilder class is used to create and modify Terrain Meshes
    /// </summary>
    public class TerrainBuilder
    {
        /// <summary>
        /// Creates a terrain from a height map
        /// </summary>
        /// <param name="heightMap"></param>
        /// <param name="maxHeight"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public static MeshInfo CreateTerrainFromHeightMap(Texture2D heightMap, float maxHeight, Material material)
        {
            int width = heightMap.width;
            int height = heightMap.height;
            float scaleX = 10f / width;
            float scaleZ = 10f / height;

            Vector3[] vertices = new Vector3[width * height];
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    float heightValue = heightMap.GetPixel(x, z).grayscale * maxHeight;
                    vertices[z * width + x] = new Vector3(x * scaleX, heightValue, z * scaleZ);
                }
            }

            int[] triangles = new int[(width - 1) * (height - 1) * 6];
            int index = 0;
            for (int z = 0; z < height - 1; z++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int vertexIndex = z * width + x;
                    triangles[index++] = vertexIndex;
                    triangles[index++] = vertexIndex + width;
                    triangles[index++] = vertexIndex + 1;

                    triangles[index++] = vertexIndex + 1;
                    triangles[index++] = vertexIndex + width;
                    triangles[index++] = vertexIndex + width + 1;
                }
            }

            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x / 10f, vertices[i].z / 10f);
            }

            // TODO: Figure out where to get Texture from when creating terrain
            var meshObj = MeshBuilder.CreateMesh(vertices, material, null);
            meshObj.name = "Terrain";

            return meshObj;
        }
        
        /// <summary>
        /// Smooths the terrain by averaging the height of each point with the height of the points around it
        /// </summary>
        /// <param name="meshInfo"></param>
        /// <param name="iterations"></param>
        public static void SmoothTerrain(MeshInfo meshInfo, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                foreach(var point in meshInfo.meshPoints)
                {
                    point.y = GetAverageHeight(meshInfo, point.position);
                }
            }
        }

        /// <summary>
        /// Smooths a single point by averaging the height of the point with the height of the points around it
        /// </summary>
        /// <param name="meshInfo"></param>
        /// <param name="position"></param>
        public static void SmoothPoint(MeshInfo meshInfo, Vector3 position)
        {
            position.y = GetAverageHeight(meshInfo, position);
        }

        /// <summary>
        /// Smooths a range of points by averaging the height of the points with the height of the points around them
        /// </summary>
        /// <param name="meshInfo">The Mesh Info that is being Smoothed</param>
        /// <param name="position">The position at which to smooth from</param>
        /// <param name="range">The area in which to Smooth</param>
        /// <param name="falloff">The fade of the smoothing</param>
        public static void SmoothRange(MeshInfo meshInfo, Vector3 position, float range, float falloff)
        {
            foreach(var point in meshInfo.meshPoints)
            {
                if (Vector3.Distance(position, point.position) < range)
                {
                    point.y = Mathf.Lerp(point.y, GetAverageHeight(meshInfo, point.position), Vector3.Distance(position, point.position)/range * falloff); 
                }
            }
        }
        
        /// <summary>
        /// Adds height to the terrain
        /// </summary>
        /// <param name="meshInfo"></param>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <param name="falloff"></param>
        /// <param name="height"></param>
        public static void AddHeight(MeshInfo meshInfo, Vector3 position, float range, float falloff, float height)
        {
            foreach(var point in meshInfo.meshPoints)
            {
                if (Vector3.Distance(position, point.position) < range)
                {
                    point.y = Mathf.Lerp(point.y, point.y + height, Vector3.Distance(position, point.position)/range * falloff); 
                }
            }
        }
        
        /// <summary>
        /// Subtracts height from the terrain
        /// </summary>
        /// <param name="meshInfo"></param>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <param name="falloff"></param>
        /// <param name="height"></param>
        public static void SubtractHeight(MeshInfo meshInfo, Vector3 position, float range, float falloff, float height)
        {
            foreach(var point in meshInfo.meshPoints)
            {
                if (Vector3.Distance(position, point.position) < range)
                {
                    point.y = Mathf.Lerp(point.y, point.y - height, Vector3.Distance(position, point.position)/range * falloff); 
                }
            }
        }
        
        /// <summary>
        /// Flattens the terrain
        /// </summary>
        /// <param name="meshInfo"></param>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <param name="falloff"></param>
        /// <param name="height"></param>
        public static void Flatten(MeshInfo meshInfo, Vector3 position, float range, float falloff, float height)
        {
            foreach(var point in meshInfo.meshPoints)
            {
                if (Vector3.Distance(position, point.position) < range)
                {
                    point.y = Mathf.Lerp(point.y, height, Vector3.Distance(position, point.position)/range * falloff); 
                }
            }
        }
        
        /// <summary>
        /// Stamps a heightmap onto the terrain
        /// </summary>
        /// <param name="meshInfo"></param>
        /// <param name="heightMap"></param>
        /// <param name="maxHeight"></param>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <param name="falloff"></param>
        public static void AddHeightmap(MeshInfo meshInfo, Texture2D heightMap, float maxHeight, Vector3 position, float range, float falloff)
        {
            foreach(var point in meshInfo.meshPoints)
            {
                if (Vector3.Distance(position, point.position) < range)
                {
                    point.y = Mathf.Lerp(point.y, point.y + heightMap.GetPixel((int)(point.position.x * 10), (int)(point.position.z * 10)).grayscale * maxHeight, Vector3.Distance(position, point.position)/range * falloff); 
                }
            }
        }
        
        /// <summary>
        /// Stamps a heightmap onto the terrain in all directions, using the normal of the points
        /// </summary>
        /// <param name="meshInfo"></param>
        /// <param name="heightMap"></param>
        /// <param name="maxHeight"></param>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <param name="falloff"></param>
        public static void AddHeightmap3D(MeshInfo meshInfo, Texture2D heightMap, float maxHeight, Vector3 position, float range, float falloff)
        {
            foreach(var point in meshInfo.meshPoints)
            {
                if (Vector3.Distance(position, point.position) < range)
                {
                    point.x = Mathf.Lerp(point.x, point.x + heightMap.GetPixel((int)(point.position.x * 10), (int)(point.position.z * 10)).grayscale * maxHeight * point.normal.x, Vector3.Distance(position, point.position)/range * falloff);
                    point.y = Mathf.Lerp(point.y, point.y + heightMap.GetPixel((int)(point.position.x * 10), (int)(point.position.z * 10)).grayscale * maxHeight * point.normal.y, Vector3.Distance(position, point.position)/range * falloff); 
                    point.z = Mathf.Lerp(point.z, point.z + heightMap.GetPixel((int)(point.position.x * 10), (int)(point.position.z * 10)).grayscale * maxHeight * point.normal.z, Vector3.Distance(position, point.position)/range * falloff);
                }
            }
        }
        
        /// <summary>
        /// Subtracts a heightmap from the terrain in all directions, using the normal of the points
        /// </summary>
        /// <param name="meshInfo"></param>
        /// <param name="heightMap"></param>
        /// <param name="maxHeight"></param>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <param name="falloff"></param>
        public static void SubstractHeightmap3D(MeshInfo meshInfo, Texture2D heightMap, float maxHeight, Vector3 position, float range, float falloff)
        {
            foreach(var point in meshInfo.meshPoints)
            {
                if (Vector3.Distance(position, point.position) < range)
                {
                    point.x = Mathf.Lerp(point.x, point.x - heightMap.GetPixel((int)(point.position.x * 10), (int)(point.position.z * 10)).grayscale * maxHeight * point.normal.x, Vector3.Distance(position, point.position)/range * falloff);
                    point.y = Mathf.Lerp(point.y, point.y - heightMap.GetPixel((int)(point.position.x * 10), (int)(point.position.z * 10)).grayscale * maxHeight * point.normal.y, Vector3.Distance(position, point.position)/range * falloff); 
                    point.z = Mathf.Lerp(point.z, point.z - heightMap.GetPixel((int)(point.position.x * 10), (int)(point.position.z * 10)).grayscale * maxHeight * point.normal.z, Vector3.Distance(position, point.position)/range * falloff);
                }
            }
        }

        /// <summary>
        /// Gets the average height of the points around a given point
        /// </summary>
        /// <param name="meshInfo"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static float GetAverageHeight(MeshInfo meshInfo, Vector3 position)
        {
            float averageHeight = 0f;
            int numPoints = 0;
            foreach(var point in meshInfo.meshPoints)
            {
                if (Vector3.Distance(position, point.position) < 0.1f)
                {
                    averageHeight += point.position.y;
                    numPoints++;
                }
            }
            return averageHeight / numPoints;
        }
    }
}