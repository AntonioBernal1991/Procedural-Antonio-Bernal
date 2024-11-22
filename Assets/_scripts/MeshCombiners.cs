using System.Collections.Generic;
using UnityEngine;


//unifies the meshes of the cubes winning up to 100 fps
public class MeshCombiner
{
    public static void CombineMeshesByMaterial(GameObject parent)
    {
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0)
        {
            
            return;
        }

       
        Dictionary<Material, List<CombineInstance>> combineInstancesByMaterial = new Dictionary<Material, List<CombineInstance>>();

        // Collects meshes and sort by material
        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.sharedMesh == null) continue;

            MeshRenderer renderer = meshFilter.GetComponent<MeshRenderer>();
            if (renderer == null || renderer.sharedMaterial == null) continue;

            Material material = renderer.sharedMaterial;

            if (!combineInstancesByMaterial.ContainsKey(material))
            {
                combineInstancesByMaterial[material] = new List<CombineInstance>();
            }

          
            CombineInstance combineInstance = new CombineInstance
            {
                mesh = meshFilter.sharedMesh,
                transform = parent.transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix
            };

            combineInstancesByMaterial[material].Add(combineInstance);
            meshFilter.gameObject.SetActive(false); 
        }

        // creates a mesh combine for each material
        foreach (var entry in combineInstancesByMaterial)
        {
            Material material = entry.Key;
            List<CombineInstance> combineInstances = entry.Value;

            // Creates a new gameobject for each material
            GameObject combinedObject = new GameObject($"{parent.name}_{material.name}_Combined");
            combinedObject.transform.parent = parent.transform;
            combinedObject.transform.localPosition = Vector3.zero;
            combinedObject.transform.localRotation = Quaternion.identity;

            MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
            MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();

            combinedMeshFilter.mesh = new Mesh();
            combinedMeshFilter.mesh.CombineMeshes(combineInstances.ToArray(), true, true);
            combinedMeshRenderer.material = material;

            combinedObject.SetActive(true);
        }

    }
}
