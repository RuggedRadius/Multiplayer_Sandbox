using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorldGen
{

    [RequireComponent(typeof(MeshFilter))]
    public class MeshGenerator : MonoBehaviour
    {
        public Mesh mesh;
        public MeshRenderer rend;
        public MeshCollider col;


        public int xSize;
        public int zSize;
        public Vector2 perlinNoiseScale;
        public int perlinMultiplier;
        //public float height;

        Vector3[] vertices;
        int[] triangles;
        Color[] vertexColours;

        public Gradient gradient;

        public static float minTerrainHeight;
        public static float maxTerrainHeight;

        //void Start()
        //{
        //    mesh = new Mesh();
        //    GetComponent<MeshFilter>().mesh = mesh;

        //    if (this.GetComponent<MeshRenderer>() == null)
        //        rend = this.gameObject.AddComponent<MeshRenderer>();
        //    else
        //        rend = this.gameObject.GetComponent<MeshRenderer>();

        //    if (this.GetComponent<MeshCollider>() == null)
        //        col = this.gameObject.AddComponent<MeshCollider>();
        //    else
        //        col = this.gameObject.GetComponent<MeshCollider>();

        //    //CreateShape();
        //    UpdateMesh();

        //    col.sharedMesh = mesh;
        //}

        //void CreateShape()
        //{
        //    vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        //    for (int i = 0, z = 0; z <= zSize; z++)
        //    {
        //        for (int x = 0; x <= xSize; x++)
        //        {
        //            float y = Mathf.PerlinNoise(x * perlinNoiseScale.x, z * perlinNoiseScale.y) * perlinMultiplier;
        //            vertices[i] = new Vector3(x, y, z);

        //            if (y > maxTerrainHeight)
        //                maxTerrainHeight = y;
        //            if (y < minTerrainHeight)
        //                minTerrainHeight = y;
        //            i++;
        //        }
        //    }
        //    Debug.Log("Calculating vertices complete!");


        //    triangles = new int[xSize * zSize * 6];
        //    int vert = 0;
        //    int tris = 0;
        //    for (int z = 0; z < zSize; z++)
        //    {
        //        for (int x = 0; x < xSize; x++)
        //        {
        //            triangles[tris + 0] = vert + 0;
        //            triangles[tris + 1] = vert + xSize + 1;
        //            triangles[tris + 2] = vert + 1;
        //            triangles[tris + 3] = vert + 1;
        //            triangles[tris + 4] = vert + xSize + 1;
        //            triangles[tris + 5] = vert + xSize + 2;

        //            vert++;
        //            tris += 6;
        //        }
        //        vert++;
        //    }
        //    Debug.Log("Calculating triangles complete!");

        //    vertexColours = new Color[vertices.Length];
        //    for (int i = 0, z = 0; z < zSize; z++)
        //    {
        //        for (int x = 0; x < xSize; x++)
        //        {
        //            float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y); 
        //            vertexColours[i] = gradient.Evaluate(height);
        //            i++;
        //        }
        //    }
        //    Debug.Log("Vertex colouring complete!");
        //}

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    int index = Random.Range(0, vertices.Length);

            //    vertices[index].y = Random.Range(0f, 10f);

            //    //vertices[index] -= new Vector3(0, 10, 0);

            //    UpdateMesh();
            //}

            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("Terraforming");

                // Find closest vertices
                Vector3 playerPosition = ClientScene.localPlayer.gameObject.transform.position;
                Dictionary<int, float> vertexDistances = new Dictionary<int, float>();
                for (int i = 0; i < vertices.Length; i++)
                {
                    int index = i;
                    Vector3 convertedWorldPosition = transform.TransformPoint(vertices[i]);
                    float distance = Vector3.Distance(convertedWorldPosition, playerPosition);

                    vertexDistances.Add(i, distance);
                }

                List<float> distances = vertexDistances.Values.ToList();
                distances.Sort();
                distances.Reverse();

                int requiredVertexCount = 4;
                int[] closestVertexIndices = new int[requiredVertexCount];
                for (int i = 0; i < requiredVertexCount; i++)
                {
                    // Find index by distance in dictionary
                    float dist = distances[i];
                    int index = vertexDistances.FirstOrDefault(x => x.Value == dist).Key;
                    closestVertexIndices[i] = index;
                }

                for (int i = 0; i < closestVertexIndices.Length; i++)
                {
                    Debug.Log(closestVertexIndices[i]);
                }



                float heightIncrement = 0.25f;
                try
                {
                    for (int i = 0; i < requiredVertexCount; i++)
                    {
                        // Get vertex and alter height
                        int index = closestVertexIndices[i];
                        vertices[index].y += heightIncrement;

                        // Determine vertex colour
                        float normalisedHeight = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[index].y);
                        vertexColours[index] = gradient.Evaluate(normalisedHeight);

                        i++;
                    }

                    UpdateMesh();
                }
                catch (Exception)
                {
                    Debug.LogWarning("Vertex not raised. Vertex not found.");
                }




                //// Find closest vertex

                //int closestIndex = -1;
                //float closestDistance = float.MaxValue;
                //for (int i = 0; i < vertices.Length; i++)
                //{
                //    Vector3 convertedWorldPosition = transform.TransformPoint(vertices[i]);
                //    float distance = Vector3.Distance(convertedWorldPosition, playerPosition);
                //    if (distance < closestDistance)
                //    {
                //        closestIndex = i;
                //        closestDistance = distance;
                //    }
                //}

                // determine height
                //float heightIncrement = 0.25f;
                //int size = 2;
                //try
                //{
                //    for (int x = 0; x < size; x++)
                //    {
                //        for (int z = 0; z < size; z++)
                //        {
                //            int index = (closestIndex + (x - size)) + ((z + 1) + ((z - (size/2)) * zSize));
                //            vertices[index].y += heightIncrement;
                //            float normalisedHeight = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[index].y);
                //            vertexColours[index] = gradient.Evaluate(normalisedHeight);
                //        }
                //    }

                //    UpdateMesh();
                //}
                //catch (Exception)
                //{
                //    Debug.LogWarning("Vertex not raised. Vertex not found.");
                //}            
            }
        }

        void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.colors = vertexColours;

            mesh.RecalculateNormals();

            col.sharedMesh = mesh;
        }

        public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            float topLeftX = (width - 1) / -2f;
            float topLeftZ = (width - 1) / 2f;

            int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

            MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
            int vertexIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
                    meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                    if (x < (width - 1) && (y < height - 1))
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                        meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }

        //private void OnDrawGizmos()
        //{
        //    //if (vertices.Length == 0) return;

        //    for (int i = 0; i < vertices.Length; i++)
        //    {
        //        Gizmos.DrawSphere(vertices[i], 0.1f);
        //    }
        //}
    }

    public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        int triangleIndex;

        public MeshData(int meshWidth, int meshHeight)
        {
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles[triangleIndex + 0] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}