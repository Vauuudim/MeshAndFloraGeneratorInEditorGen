using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode {NoiseMap, Mesh, MeshAndFlora};
	public DrawMode drawMode;

	[Range(0, 6)]
	public int levelOfDetail;

	public NoiseData noiseData;
	public FloraData floraData;
	public TextureData textureData;
	public Material terrainMaterial;
	public bool randomNoiseMesh;
	public bool randomNoiseFlora;
	public bool randomNoiseForTypesFlora;
	public bool randomNoiseForDensityOfFlora;
	public bool autoUpdate;

	void OnValuesUpdated()
	{
		if (!Application.isPlaying)
		{
			DrawMapInEditor ();
		}
	}

	void OnTextureValuesUpdated()
	{
		textureData.ApplyToMaterial (terrainMaterial);
	}

	public int mapChunkSize
	{
		get
		{
			if (noiseData.useFlatShading)
			{
				return 95;
			} else {
				return 239;
			}
		}
	}

	public void DrawMapInEditor() {
		MapData mapData = GenerateMapData (Vector2.zero);
		MapDisplay display = FindObjectOfType<MapDisplay> ();

		if (drawMode == DrawMode.NoiseMap)
		{
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (mapData.heightMap));
		}
		else if (drawMode == DrawMode.Mesh)
		{
			display.DrawMesh (MeshGenerator.GenerateTerrainMesh (mapData.heightMap, noiseData.meshHeightMultiplier, noiseData.meshHeightCurve, levelOfDetail, noiseData.useFlatShading));
		}
		else if (drawMode == DrawMode.MeshAndFlora)
		{
			display.DrawMesh(MeshAndFloraGenerator.GenerateTerrainMeshAndFlora(mapData.heightMap, noiseData.meshHeightMultiplier, noiseData.meshHeightCurve, levelOfDetail, noiseData.useFlatShading, floraData.floraModels, mapData.noiseMapFlora, floraData.heightOfFlora, floraData.densityOfFlora, mapData.noiseMapForTypes, mapData.noiseMapForDensityOfFlora, floraData.percentageOfForest, noiseData.uniformScale, floraData.sizeOfModels, floraData.maxBushes));
		}
	}

	MapData GenerateMapData(Vector2 centre)
	{
		if (randomNoiseMesh == true)
		{
			noiseData.seed = UnityEngine.Random.Range(1, 999);
		}
		if (randomNoiseFlora == true)
		{
			floraData.seedFlora = noiseData.seed + UnityEngine.Random.Range(1, 999);
		}
		if (randomNoiseForTypesFlora == true)
		{
			floraData.seedForTypes = noiseData.seed + UnityEngine.Random.Range(-999, -1);
		}
		if (randomNoiseForDensityOfFlora == true)
		{
			floraData.seedForDensityOfFlora = floraData.seedFlora + UnityEngine.Random.Range(1, 999);
		}
		/*
		//Фикс для одиннаковых сидов флоры и меша.
		if (noiseData.seed == floraData.seedFlora)
		{
			floraData.seedFlora = noiseData.seed + UnityEngine.Random.Range(1, 999);
		}
		//Фикс для одиннаковых сидов флоры и типов флоры.
		if (floraData.seedFlora == floraData.seedForTypes)
        {
			floraData.seedForTypes = floraData.seedFlora + UnityEngine.Random.Range(1, 999);
		}
		*/
			

		float[,] noiseMap = Noise.GenerateNoiseMap (mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, centre + noiseData.offset, noiseData.normalizeMode, noiseData.falloff, noiseData.sizeFalloff, noiseData.multy);
		float[,] noiseMapFlora = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, floraData.seedFlora, floraData.noiseScale.Flora, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, centre + noiseData.offset, noiseData.normalizeMode, noiseData.falloff, noiseData.sizeFalloff, noiseData.multy);
		float[,] noiseMapForTypes = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, floraData.seedForTypes, floraData.noiseScale.Types, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, centre + noiseData.offset, noiseData.normalizeMode, noiseData.falloff, noiseData.sizeFalloff, noiseData.multy);
		float[,] noiseMapForDensityOfFlora = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, floraData.seedForDensityOfFlora, floraData.noiseScale.DensityOfFlora, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, centre + noiseData.offset, noiseData.normalizeMode, noiseData.falloff, noiseData.sizeFalloff, noiseData.multy);
		textureData.UpdateMeshHeights (terrainMaterial, noiseData.minHeight, noiseData.maxHeight);

		return new MapData (noiseMap, noiseMapFlora, noiseMapForTypes, noiseMapForDensityOfFlora);
	}

	void OnValidate()
	{
		if (noiseData != null)
		{
			noiseData.OnValuesUpdated -= OnValuesUpdated;
			noiseData.OnValuesUpdated += OnValuesUpdated;
		}
		if (textureData != null)
		{
			textureData.OnValuesUpdated -= OnTextureValuesUpdated;
			textureData.OnValuesUpdated += OnTextureValuesUpdated;
		}
	}
}

public struct MapData
{
	public readonly float[,] heightMap;
	public readonly float[,] noiseMapFlora;
	public readonly float[,] noiseMapForTypes;
	public readonly float[,] noiseMapForDensityOfFlora;
	public MapData (float[,] heightMap, float[,] noiseMapFlora, float[,] noiseMapForTypes, float[,] noiseMapForDensityOfFlora)
	{
		this.heightMap = heightMap;
		this.noiseMapFlora = noiseMapFlora;
		this.noiseMapForTypes = noiseMapForTypes;
		this.noiseMapForDensityOfFlora = noiseMapForDensityOfFlora;
	}
}
