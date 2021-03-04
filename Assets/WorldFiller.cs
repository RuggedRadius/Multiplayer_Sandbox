using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldFiller : MonoBehaviour
{
    public GameObject world;

    public GameObject prefab_ForestTree;

    public MapGenerator mapGen;

    public float minForestHeight = float.MaxValue;
    public float maxForestHeight = float.MinValue;

    public void Fill()
    {
        Debug.Log("Min: " + minForestHeight + " Max: " + maxForestHeight);
        GetHeightBounds();
        Debug.Log("Min: " + minForestHeight + " Max: " + maxForestHeight);
        StartCoroutine(FillWorld());
    }

    private void GetHeightBounds()
    {
        Mesh mesh = world.GetComponent<MeshFilter>().mesh;

        for (int i = 0; i < mesh.colors.Length; i++)
        {
            if (mesh.colors[i] == mapGen.regions[4].color)
            {
                Debug.Log("Colour match!");
                if (mesh.vertices[i].y > maxForestHeight)
                    maxForestHeight = mesh.vertices[i].y;

                if (mesh.vertices[i].y < minForestHeight)
                    minForestHeight = mesh.vertices[i].y;
            }
        }
    }

    public IEnumerator FillWorld()
    {
        Debug.Log("Starting trees...");
        Mesh mesh = world.GetComponent<MeshFilter>().mesh;

        float chanceOfTree = 0.001f;
        MeshFilter meshFilter = world.GetComponent<MeshFilter>();

        for (int i = 0; i < mesh.vertices.Length; i++)
        {            
            //if (mesh.vertices[i].y > minForestHeight)
            //{
            //    if (mesh.vertices[i].y < maxForestHeight)
            //    {
                    

                    if (Random.Range(0f, 1f) >= (1 - chanceOfTree))
                    {
                        Debug.Log("Creating tree");

                // Create tree
                //GameObject newTree = Instantiate(prefab_ForestTree);
                GameObject newTree = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        newTree.transform.position = meshFilter.transform.TransformPoint(mesh.vertices[i]);
                        yield return null;
                    }
            //    }
            //}
        }
        yield return null;
    }
}
