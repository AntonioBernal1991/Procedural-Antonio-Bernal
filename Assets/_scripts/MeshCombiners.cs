using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner
{
    public static void CombineMeshesByMaterial(GameObject parent)
    {
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0)
        {
            
            return;
        }

        // Diccionario para agrupar CombineInstance por material
        Dictionary<Material, List<CombineInstance>> combineInstancesByMaterial = new Dictionary<Material, List<CombineInstance>>();

        // Recopilar mallas y separarlas por material
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

            // Ajustar la transformación al espacio local del objeto padre
            CombineInstance combineInstance = new CombineInstance
            {
                mesh = meshFilter.sharedMesh,
                transform = parent.transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix
            };

            combineInstancesByMaterial[material].Add(combineInstance);
            meshFilter.gameObject.SetActive(false); // Desactivar los hijos
        }

        // Crear una malla combinada para cada material
        foreach (var entry in combineInstancesByMaterial)
        {
            Material material = entry.Key;
            List<CombineInstance> combineInstances = entry.Value;

            // Crear un nuevo GameObject para cada material
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
