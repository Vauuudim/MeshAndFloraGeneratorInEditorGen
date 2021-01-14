using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class FloraMaterials : UpdatableData
{
	public MaterialsOfFlora floraMaterials;
}

[System.Serializable]
public struct MaterialsOfFlora
{
	public Material Seaweed;
	public Material Grass;
	public Material Bushes;
	public Material Trees;
}