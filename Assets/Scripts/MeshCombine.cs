using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombine : MonoBehaviour {

    void Start()
    {
        //Zero transformation is needed because of localToWorldMatrix transform
        Vector3 position = transform.position;
        transform.position = Vector3.zero;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        List<Material> mats = new List<Material>();


        int i = 0;
        /*
        // Then we add the colliders 
        var colliders = GetComponentsInChildren<CapsuleCollider>();
        for (i = 0; i < colliders.Length; i++)
        {
            var collider = transform.gameObject.AddComponent<CapsuleCollider>();
            collider.center = colliders[i].transform.position;
            collider.radius = colliders[i].radius;
            collider.height = colliders[i].height;
        }*/

        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            Material mat = meshFilters[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            if (mat!=null && !mats.Contains(mat)) mats.Add(mat);
            
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.GetComponent<MeshRenderer>().materials = mats.ToArray();
        
        transform.gameObject.SetActive(true);

        transform.position = position;
    }

}
