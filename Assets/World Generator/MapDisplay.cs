using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldGen;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        // Display in editor, not written to renderer.material as is only instantiated at runtime
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.GetComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;
        //meshRenderer.sharedMaterial.SetTexture("_BaseColorMap", texture);
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
