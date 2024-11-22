using System.Collections;
using UnityEngine;


//Creates the modules with cube so the path generator can go through
public class ModuleGenerator
{
    private readonly IMapGenerator _mapGenerator;
    private readonly IObjectPool _pool;
    private int _totalModulesGenerated = 0; 

    public ModuleGenerator(IMapGenerator mapGenerator, IObjectPool pool)
    {
        this._mapGenerator = mapGenerator;
        this._pool = pool;
    }

    public IEnumerator StartRecursiveGeneration(int numModules)
    {
        yield return GenerateModuleRecursive(numModules);
    }

    private IEnumerator GenerateModuleRecursive(int numModules)
    {
        // Termination case for recursion
        if (_totalModulesGenerated >= numModules)
        {
            Debug.Log("All modules generated.");
            yield break;
        }

        // Generate the current module
        GameObject moduleContainer = new GameObject($"Module_{_totalModulesGenerated + 1}");
        ModuleInfo myModuleInfo = null;

        if(ModuleInfoQueueManager.Count > 0)
        {
            myModuleInfo = ModuleInfoQueueManager.Dequeue();
            moduleContainer.transform.position = myModuleInfo.NextModulePosition;
        }
       

        // Generate layers
        GameObject[,] grassLayer = GenerateLayer(moduleContainer, _mapGenerator.GrassMaterial, 0);
        GenerateLayer(moduleContainer, _mapGenerator.GroundMaterial, -1);

        // Generate the path asynchronously
        yield return _mapGenerator.PathGenerator.GeneratePath(grassLayer, _totalModulesGenerated, myModuleInfo.LastDirection);


        // Combine meshes
        MeshCombiner.CombineMeshesByMaterial(moduleContainer);

        // Increment the global counter
        _totalModulesGenerated++;

        // Wait for a frame before the next recursive call (optional)
        yield return null;

     
        // Recursive call for the next module
        if (_totalModulesGenerated % 3 == 10)
        {
            // Branching path: make two recursive calls
            Debug.Log("recursive call");
            yield return GenerateModuleRecursive(numModules); // First branch
            yield return GenerateModuleRecursive(numModules); // Second branch
        }
        else
        {
            // Single recursive call
            yield return GenerateModuleRecursive(numModules);
        }
    }

    //Generates the Grass layer and the Gorund layer below
    private GameObject[,] GenerateLayer(GameObject parent, Material material, float yOffset)
    {
        int mapWidth = _mapGenerator.MapWidth;
        int mapHeight = _mapGenerator.MapHeight;
        float spacing = _mapGenerator.Spacing;

        GameObject[,] layer = new GameObject[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                Vector3 position = new Vector3(x * spacing, yOffset, z * spacing) + parent.transform.position;
                GameObject cube = _pool.GetObject();
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
