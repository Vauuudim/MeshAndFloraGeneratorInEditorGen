using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class FloraModels : UpdatableData
{
	public ModelsOfFlora floraModels;
}

[System.Serializable]
public struct ModelsOfFlora
{
	public GameObject[] Underwater; //Водоросли
	public GameObject[] TreesT1;    //Деревья типа 1
	public GameObject[] TreesT1R;   //Деревья типа 1 редкие
	public GameObject[] TreesT2;    //Деревья типа 2
	public GameObject[] TreesT2R;   //Деревья типа 2 редкие
	public GameObject[] TreesT1W;   //Деревья типа 1 зимние
	public GameObject[] TreesT1WR;  //Деревья типа 1 зимние редкие
	public GameObject[] TreesT2W;   //Деревья типа 2 зимние
	public GameObject[] TreesT2WR;  //Деревья типа 2 зимние редкие
	public GameObject[] Grass;      //Трава
	public GameObject[] Bushes;     //Кусты
}