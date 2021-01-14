using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class NoiseData : UpdatableData {

	public Noise.NormalizeMode normalizeMode;

	public float noiseScale;
	[Range(1, 31)]
	public int octaves;
	[Range(0, 1)]
	public float persistance;
	public float lacunarity;

	public int seed;

	public Vector2 offset;

	public float uniformScale = 2.5f;
	public bool useFlatShading;
	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;
	public bool falloff;
	[Range(0.1f, 10f)]
	public float sizeFalloff;
	[Range(-10, 10)]
	public float multy;

	public float minHeight
	{
		get
		{
			return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(0);
		}
	}

	public float maxHeight
	{
		get
		{
			return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(1);
		}
	}

	protected override void OnValidate() {
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}

		base.OnValidate ();
	}

}
