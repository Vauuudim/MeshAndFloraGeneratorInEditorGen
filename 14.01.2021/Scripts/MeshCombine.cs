using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshFloraCombine : MonoBehaviour
{
    public void MeshCombining(Material material)
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        this.GetComponent<MeshRenderer>().material = material;
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }

     /*
     void CombineMeshes()
     {
         MeshFilter[] meshFilters = GetComponentInChildren<MeshFilter>();
         CombineInstance[] combines = new CombineInstance[meshFilters.Length];

         for (int i = 0; i < meshFilters.Length; i++)
         {
             combines[i].mesh = meshFilters[i].sharedMesh;
             combines[i].transform = meshFilters[i].transform.localToWorldMatrix;
             meshFilters[i].gameObject.SetActive(false);
         }
         MeshFilter meshFiltere = transform.getComponent<MeshFilter>();
         meshFiltere.mesh = new Mesh();
         meshFiltere.mesh.CombineMeshes(combines);
         GetComponent<MeshCollider>().sharedMesh = meshFiltere.mesh;
         transform.gameObject.SetActive(true);
     }

    }*/
