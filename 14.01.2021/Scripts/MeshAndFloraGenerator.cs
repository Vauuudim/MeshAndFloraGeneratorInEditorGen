using UnityEngine;
using System.Collections;

public static class MeshAndFloraGenerator
{
	public static MeshData GenerateTerrainMeshAndFlora(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail, bool useFlatShading, ModelsOfFlora floraModels, float[,] noiseMapFlora, HeightOfFlora heightOfFlora, DensityOfFlora densityOfFlora, float[,] noiseMapForTypes, float[,] noiseMapForDensityOfFlora, float percentageOfForest, float uniformScale, SizeOfModels sizeOfModels, int maxBushes, FloraMaterials floraMaterials)
	{
		AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

		int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
		int borderedSize = heightMap.GetLength(0);
		int meshSize = borderedSize - 2 * meshSimplificationIncrement;
		int meshSizeUnsimplified = borderedSize - 2;

		float topLeftX = (meshSizeUnsimplified - 1) / -2f;
		float topLeftZ = (meshSizeUnsimplified - 1) / 2f;
			
		GameObject inst_objParent = new GameObject("Flora");
		inst_objParent.transform.position = new Vector3(0, 0, 0);
		inst_objParent.isStatic = true;
		GameObject grass = new GameObject("Grass");
		inst_objParent.transform.position = new Vector3(0, 0, 0);
		grass.transform.SetParent(inst_objParent.transform);
		grass.AddComponent<MeshFilter>();
		grass.AddComponent<MeshRenderer>();
		//grass.AddComponent<MeshCombine>();
		grass.isStatic = true;

		int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;

		MeshData meshData = new MeshData(verticesPerLine, useFlatShading);

		int[,] vertexIndicesMap = new int[borderedSize, borderedSize];
		int meshVertexIndex = 0;
		int borderVertexIndex = -1;
		int meshCombainCount = 0;
		int countMeshFilters = 95;
		MeshFilter[] meshFilters = new MeshFilter[countMeshFilters];

		for (int y = 0; y < borderedSize; y += meshSimplificationIncrement)
		{
			for (int x = 0; x < borderedSize; x += meshSimplificationIncrement)
			{
				bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

				if (isBorderVertex)
				{
					vertexIndicesMap[x, y] = borderVertexIndex;
					borderVertexIndex--;
				}
				else
				{
					vertexIndicesMap[x, y] = meshVertexIndex;
					meshVertexIndex++;
				}
			}
		}

		for (int y = 0; y < borderedSize; y += meshSimplificationIncrement)
		{
			for (int x = 0; x < borderedSize; x += meshSimplificationIncrement)
			{
				int vertexIndex = vertexIndicesMap[x, y];
				Vector2 percent = new Vector2((x - meshSimplificationIncrement) / (float)meshSize, (y - meshSimplificationIncrement) / (float)meshSize);
				float height = heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;
				Vector3 vertexPosition = new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height, topLeftZ - percent.y * meshSizeUnsimplified);

				meshData.AddVertex(vertexPosition, percent, vertexIndex);

				if (x < borderedSize - 1 && y < borderedSize - 1)
				{
					int a = vertexIndicesMap[x, y];
					int b = vertexIndicesMap[x + meshSimplificationIncrement, y];
					int c = vertexIndicesMap[x, y + meshSimplificationIncrement];
					int d = vertexIndicesMap[x + meshSimplificationIncrement, y + meshSimplificationIncrement];
					meshData.AddTriangle(a, d, c);
					meshData.AddTriangle(d, a, b);
				}

				//œŒƒ ¬ŒƒŒ…
				if (floraModels.Underwater.Length != 0 && heightMap[x, y] <= heightOfFlora.Underwater && Random.Range(0, 100) <= densityOfFlora.Underwater * noiseMapForDensityOfFlora[x, y])
				{
					Vector3 koords = (vertexPosition + new Vector3(Random.Range(0f, 0.5f), 0, Random.Range(0f, 0.5f))) * uniformScale;
					GameObject inst_obj = GameObject.Instantiate(floraModels.Underwater[0 + Random.Range(0, floraModels.Underwater.Length)], koords, Quaternion.Euler(Random.Range(-4, 5), Random.Range(-360, 360), Random.Range(-4, 5))) as GameObject;
					inst_obj.transform.localScale = new Vector3(sizeOfModels.Seaweed, sizeOfModels.Seaweed, sizeOfModels.Seaweed) * Random.Range(0.8f, 1.2f) * uniformScale;
					inst_obj.transform.SetParent(inst_objParent.transform);
				}
				//¬€ÿ≈ œÀﬂ∆¿
				if (heightMap[x, y] >= heightOfFlora.Beach && heightMap[x, y] <= heightOfFlora.Ground)
                {
					//“–¿¬¿
					if (floraModels.Grass.Length != 0 && Random.Range(1, 100) <= densityOfFlora.Grass)
					{
						Vector3 koords = vertexPosition * uniformScale;
						GameObject inst_obj = GameObject.Instantiate(floraModels.Grass[0], koords, Quaternion.Euler(Random.Range(-4, 5), Random.Range(-360, 360), Random.Range(-4, 5))) as GameObject;
						inst_obj.transform.localScale = new Vector3(sizeOfModels.Grass, sizeOfModels.Grass, sizeOfModels.Grass) * Random.Range(0.8f, 1.2f) * uniformScale;
						inst_obj.transform.SetParent(grass.transform);

						meshFilters[meshCombainCount] = inst_obj.GetComponent<MeshFilter>();
						meshCombainCount++;	
						if (meshCombainCount == countMeshFilters)
                        {
							CombineInstance[] combine = new CombineInstance[meshCombainCount];
							for (int i = 0; i < meshCombainCount; i++)
							{
								combine[i].mesh = meshFilters[i].sharedMesh;
								combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
								meshFilters[i].gameObject.SetActive(false);
							}
							grass.GetComponent<MeshRenderer>().material = floraMaterials.floraMaterials.Grass;
							grass.transform.GetComponent<MeshFilter>().mesh = new Mesh();
							grass.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
							grass.transform.gameObject.SetActive(true);

							grass = new GameObject("Grass");
							grass.AddComponent<MeshFilter>();
							grass.AddComponent<MeshRenderer>();
							inst_objParent.transform.position = new Vector3(0, 0, 0);
							grass.transform.SetParent(inst_objParent.transform);
							//grass.AddComponent<MeshFloraCombine>();
							meshCombainCount = 0;
							grass.isStatic = true;
						}
					}
					//ÀÂÒ “»œ1
					if (floraModels.TreesT1.Length != 0 && Random.Range(1, 100) <= densityOfFlora.Ground * noiseMapForDensityOfFlora[x, y] && noiseMapForTypes[x, y] < 0.5f && noiseMapFlora[x, y] < percentageOfForest)
					{
						Vector3 koords = (vertexPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f))) * uniformScale;
						GameObject inst_obj = GameObject.Instantiate(floraModels.TreesT1[0 + Random.Range(0, floraModels.TreesT1.Length)], koords, Quaternion.Euler(Random.Range(-4, 5), Random.Range(-360, 360), Random.Range(-4, 5))) as GameObject;
						inst_obj.transform.localScale = new Vector3(sizeOfModels.Trees, sizeOfModels.Trees, sizeOfModels.Trees) * Random.Range(0.7f, 1.5f) * uniformScale;
						inst_obj.transform.SetParent(inst_objParent.transform);
						for (int i = 0; i < Random.Range(1, maxBushes); i++)
                        {
							if (floraModels.Bushes.Length != 0 && Random.Range(0, 100) <= densityOfFlora.Bushes)
							{
								inst_obj = GameObject.Instantiate(floraModels.Bushes[0 + Random.Range(0, floraModels.Bushes.Length)], koords + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * uniformScale, Quaternion.Euler(0, Random.Range(-360, 360), 0)) as GameObject;
								inst_obj.transform.localScale = new Vector3(sizeOfModels.Buches, sizeOfModels.Buches, sizeOfModels.Buches) * Random.Range(0.8f, 1.2f) * uniformScale;
								inst_obj.transform.SetParent(inst_objParent.transform);
							}
						}
					}
					else
					if (floraModels.TreesT1R.Length != 0 && Random.Range(1, 100) <= noiseMapForDensityOfFlora[x, y] * densityOfFlora.Rare && noiseMapForTypes[x, y] < 0.5f && noiseMapFlora[x, y] < percentageOfForest)
                    {
						Vector3 koords = (vertexPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f))) * uniformScale;
						GameObject inst_obj = GameObject.Instantiate(floraModels.TreesT1R[0 + Random.Range(0, floraModels.TreesT1R.Length)], koords, Quaternion.Euler(Random.Range(-4, 5), Random.Range(-360, 360), Random.Range(-4, 5))) as GameObject;
						inst_obj.transform.localScale = new Vector3(sizeOfModels.Trees, sizeOfModels.Trees, sizeOfModels.Trees) * Random.Range(0.7f, 1.5f) * uniformScale;
						inst_obj.transform.SetParent(inst_objParent.transform);
					}
					//ÀÂÒ “»œ2
					if (floraModels.TreesT2.Length != 0 && Random.Range(1, 100) <= densityOfFlora.Ground * noiseMapForDensityOfFlora[x, y] && noiseMapForTypes[x, y] > 0.5f && noiseMapFlora[x, y] < percentageOfForest)
					{
						Vector3 koords = (vertexPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f))) * uniformScale;
						GameObject inst_obj = GameObject.Instantiate(floraModels.TreesT2[0 + Random.Range(0, floraModels.TreesT2.Length)], koords, Quaternion.Euler(Random.Range(-4, 5), Random.Range(-360, 360), Random.Range(-4, 5))) as GameObject;
						inst_obj.transform.localScale = new Vector3(sizeOfModels.Trees, sizeOfModels.Trees, sizeOfModels.Trees) * Random.Range(0.7f, 1.5f) * uniformScale;
						inst_obj.transform.SetParent(inst_objParent.transform);
						for (int i = 0; i < Random.Range(1, maxBushes); i++)
						{
							if (floraModels.Bushes.Length != 0 && Random.Range(0, 100) <= densityOfFlora.Bushes)
							{
								inst_obj = GameObject.Instantiate(floraModels.Bushes[0 + Random.Range(0, floraModels.Bushes.Length)], koords + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * uniformScale, Quaternion.Euler(0, Random.Range(-360, 360), 0)) as GameObject;
								inst_obj.transform.localScale = new Vector3(sizeOfModels.Buches, sizeOfModels.Buches, sizeOfModels.Buches) * Random.Range(0.8f, 1.2f) * uniformScale;
								inst_obj.transform.SetParent(inst_objParent.transform);
							}
						}
						
					}
					else
					if (floraModels.TreesT2R.Length != 0 && Random.Range(1, 100) <= noiseMapForDensityOfFlora[x, y] * densityOfFlora.Rare && noiseMapForTypes[x, y] > 0.5f && noiseMapFlora[x, y] < percentageOfForest)
					{
						Vector3 koords = (vertexPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f))) * uniformScale;
						GameObject inst_obj = GameObject.Instantiate(floraModels.TreesT2R[0 + Random.Range(0, floraModels.TreesT2R.Length)], koords, Quaternion.Euler(Random.Range(-4, 5), Random.Range(-360, 360), Random.Range(-4, 5))) as GameObject;
						inst_obj.transform.localScale = new Vector3(sizeOfModels.Trees, sizeOfModels.Trees, sizeOfModels.Trees) * Random.Range(0.7f, 1.5f) * uniformScale;
						inst_obj.transform.SetParent(inst_objParent.transform);
					}
				}
				//¬ √Œ–¿’
				if (heightMap[x, y] >= heightOfFlora.Ground && heightMap[x, y] <= heightOfFlora.Mountain && noiseMapFlora[x, y] < percentageOfForest)
                {
					//«ËÏÀÂÒ “»œ1
					if (floraModels.TreesT1W.Length != 0 && Random.Range(1, 100) <= densityOfFlora.Mountain * noiseMapForDensityOfFlora[x, y] && noiseMapForTypes[x, y] <= 0.5f)
					{
						Vector3 koords = (vertexPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f))) * uniformScale;
						GameObject inst_obj = GameObject.Instantiate(floraModels.TreesT1W[0 + Random.Range(0, floraModels.TreesT2.Length)], koords, Quaternion.Euler(Random.Range(-4, 5), Random.Range(-360, 360), Random.Range(-4, 5))) as GameObject;
						inst_obj.transform.localScale = new Vector3(sizeOfModels.Trees, sizeOfModels.Trees, sizeOfModels.Trees) * Random.Range(0.7f, 1.5f) * uniformScale;
						inst_obj.transform.SetParent(inst_objParent.transform);
					}
					else
					if (floraModels.TreesT1WR.Length != 0 && Random.Range(1, 100) <= densityOfFlora.Ground * noiseMapForDensityOfFlora[x, y] * 0.01f * densityOfFlora.Rare)
					{
						Vector3 koords = (vertexPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f))) * uniformScale;
						GameObject inst_obj = GameObject.Instantiate(floraModels.TreesT1WR[0 + Random.Range(0, floraModels.TreesT1WR.Length)], koords, Quaternion.Euler(Random.Range(-4, 5), Random.Range(-360, 360), Random.Range(-4, 5))) as GameObject;
						inst_obj.transform.localScale = new Vector3(sizeOfModels.Trees, sizeOfModels.Trees, sizeOfModels.Trees) * Random.Range(0.7f, 1.5f) * uniformScale;
						inst_obj.transform.SetParent(inst_objParent.transform);
					}
					//«ËÏÀÂÒ “»œ2
					if (floraModels.TreesT1W.Length != 0 && heightMap[x, y] >= heightOfFlora.Ground && heightMap[x, y] <= heightOfFlora.Mountain && Random.Range(1, 100) <= densityOfFlora.Mountain * noiseMapForDensityOfFlora[x, y] && noiseMapForTypes[x, y] > 0.5f)
					{
						Vector3 koords = (vertexPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f))) * uniformScale;
						GameObject inst_obj = GameObject.Instantiate(floraModels.TreesT2W[0 + Random.Range(0, floraModels.TreesT2W.Length)], koords, Quaternion.Euler(Random.Range(-4, 5), Random.Range(-360, 360), Random.Range(-4, 5))) as GameObject;
						inst_obj.transform.localScale = new Vector3(sizeOfModels.Trees, sizeOfModels.Trees, sizeOfModels.Trees) * Random.Range(0.7f, 1.5f) * uniformScale;
						inst_obj.transform.SetParent(inst_objParent.transform);
					}
					else
					if (floraModels.TreesT2WR.Length != 0 && Random.Range(1, 100) <= densityOfFlora.Ground * noiseMapForDensityOfFlora[x, y] * 0.01f * densityOfFlora.Rare)
					{
						Vector3 koords = (vertexPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f))) * uniformScale;
						GameObject inst_obj = GameObject.Instantiate(floraModels.TreesT2WR[0 + Random.Range(0, floraModels.TreesT2WR.Length)], koords, Quaternion.Euler(Random.Range(-4, 5), Random.Range(-360, 360), Random.Range(-4, 5))) as GameObject;
						inst_obj.transform.localScale = new Vector3(sizeOfModels.Trees, sizeOfModels.Trees, sizeOfModels.Trees) * Random.Range(0.7f, 1.5f) * uniformScale;
						inst_obj.transform.SetParent(inst_objParent.transform);
					}
				}
				vertexIndex++;
			}
		}
		meshData.ProcessMesh();
		return meshData;
	}

}