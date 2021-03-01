using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float minTerrainHeight;
    private float maxTerrainHeight;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        if (this.GetComponent<MeshRenderer>() == null)
            rend = this.gameObject.AddComponent<MeshRenderer>();
        else
            rend = this.gameObject.GetComponent<MeshRenderer>();

        if (this.GetComponent<MeshCollider>() == null)
            col = this.gameObject.AddComponent<MeshCollider>();
        else
            col = this.gameObject.GetComponent<MeshCollider>();

        CreateShape();
        UpdateMesh();

        col.sharedMesh = mesh;
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * perlinNoiseScale.x, z * perlinNoiseScale.y) * perlinMultiplier;
                vertices[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight)
                    maxTerrainHeight = y;
                if (y < minTerrainHeight)
                    minTerrainHeight = y;
                i++;
            }
        }
        Debug.Log("Calculating vertices complete!");


        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
        Debug.Log("Calculating triangles complete!");

        vertexColours = new Color[vertices.Length];
        for (int i = 0, z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y); 
                vertexColours[i] = gradient.Evaluate(height);
                i++;
            }
        }
        Debug.Log("Vertex colouring complete!");
    }

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

            // Find closest vertex
            Vector3 playerPosition = ClientScene.localPlayer.gameObject.transform.position;
            int closestIndex = -1;
            float closestDistance = float.MaxValue;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 convertedWorldPosition = transform.TransformPoint(vertices[i]);
                float distance = Vector3.Distance(convertedWorldPosition, playerPosition);
                if (distance < closestDistance)
                {
                    closestIndex = i;
                    closestDistance = distance;
                }
            }

            // determine height
            float heightIncrement = 0.25f;
            float setHeight = 2f;
            int size = 3;
            try
            {
                for (int x = 0; x < size; x++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        int index = (closestIndex + (x - size)) + ((z + 1) + ((z - (size/2)) * zSize));
                        vertices[index].y += 0.25f;
                    }
                }

                UpdateMesh();
            }
            catch (Exception)
            {
                Debug.LogWarning("Vertex not raised. Vertex not found.");
            }            
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

    //private void OnDrawGizmos()
    //{
    //    //if (vertices.Length == 0) return;

    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(vertices[i], 0.1f);
    //    }
    //}
}
