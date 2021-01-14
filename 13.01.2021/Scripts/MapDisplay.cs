using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour
{
	public Renderer textureRender;
	public GameObject meshObject;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	public MeshCollider meshCollider;
	public Material material;
	public void DrawTexture(Texture2D texture)
	{
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height);
	}

	public void DrawMesh(MeshData meshData)
	{
		meshObject.name = "Chunk";
		meshFilter.sharedMesh = meshData.CreateMesh();
		meshCollider.sharedMesh = meshFilter.sharedMesh;
		meshObject.transform.position = new Vector3(0, 0, 0);
		meshFilter.transform.localScale = Vector3.one * FindObjectOfType<MapGenerator>().noiseData.uniformScale;
	}
}
