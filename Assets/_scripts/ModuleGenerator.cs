using System.Collections;
using UnityEngine;

public class ModuleGenerator
{
    private readonly IMapGenerator mapGenerator;
    private readonly IObjectPool pool;

    public ModuleGenerator(IMapGenerator mapGenerator, IObjectPool pool)
    {
        this.mapGenerator = mapGenerator;
        this.pool = pool;
    }

    public IEnumerator GenerateModule(int numModules)
    {
        for (int i = 0; i < numModules; i++)
        {
            GameObject moduleContainer = new GameObject($"Module_{i + 1}");
            moduleContainer.transform.position = mapGenerator.NextModulePosition;

            // Generar capas de cubos
            GameObject[,] grassLayer = GenerateLayer(moduleContainer, mapGenerator.GrassMaterial, 0);
            GenerateLayer(moduleContainer, mapGenerator.GroundMaterial, -1);

            // Generar el camino
            yield return mapGenerator.PathGenerator.GeneratePath(grassLayer, i, mapGenerator.LastDirection);

            // Combinar mallas después de generar el camino
            MeshCombiner.CombineMeshesByMaterial(moduleContainer);
        }
        
    }

    private GameObject[,] GenerateLayer(GameObject parent, Material material, float yOffset)
    {
        int mapWidth = mapGenerator.MapWidth;
        int mapHeight = mapGenerator.MapHeight;
        float spacing = mapGenerator.Spacing;

        GameObject[,] layer = new GameObject[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                Vector3 position = new Vector3(x * spacing, yOffset, z * spacing) + parent.transform.position;
                GameObject cube = pool.GetObject();
                cube.transform.position = position;
                cube.transform.rotation = Quaternion.identity;
                cube.transform.parent = parent.transform;
                cube.GetComponent<Renderer>().material = material;
                layer[x, z] = cube;
            }
        }
        return layer;
    }
}
