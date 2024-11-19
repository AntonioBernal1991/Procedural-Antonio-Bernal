using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator3D : MonoBehaviour
{
    [Header("Map Configuration")]
    public int mapWidth = 10;
    public int mapHeight = 10;
    public float spacing = 1.2f; // Espaciado entre cubos
    public GameObject cubePrefab; // Prefab para los cubos
    public Material groundMaterial; // Material para el suelo (base marrón)
    public Material grassMaterial; // Material para el terreno (verde)
    public int numModules = 3; // Número de módulos a generar

    private Vector3 nextModulePosition = Vector3.zero; // Posición del siguiente módulo
    private Vector2Int lastExit; // Última posición de salida del camino
    private CurrentDirection lastDirection = CurrentDirection.DOWN; // Última dirección del camino

    public bool isFirst;

    private enum CurrentDirection { LEFT, RIGHT, DOWN }

    void Start()
    {
        StartCoroutine(GenerateModules());
    }

    IEnumerator GenerateModules()
    {
        for (int i = 0; i < numModules; i++)
        {
            yield return StartCoroutine(GenerateModule(i));
        }
    }

    IEnumerator GenerateModule(int moduleIndex)
    {
        // Crear un contenedor para el módulo actual
        GameObject moduleContainer = new GameObject($"Module_{moduleIndex + 1}");
        moduleContainer.transform.position = nextModulePosition;

        // Generar las capas de cubos (verde y marrón)
        GameObject[,] cubes = GenerateLayer(moduleContainer, grassMaterial, 0); // Capa verde
        GameObject[,] baseCubes = GenerateLayer(moduleContainer, groundMaterial, -1); // Capa marrón

        // Generar camino
        yield return StartCoroutine(GeneratePath(cubes, moduleIndex));
    }

    GameObject[,] GenerateLayer(GameObject parent, Material material, float yOffset)
    {
        GameObject[,] layer = new GameObject[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                Vector3 position = new Vector3(x * spacing, yOffset, z * spacing) + parent.transform.position;
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, parent.transform);
                cube.GetComponent<Renderer>().material = material;
                layer[x, z] = cube;
            }
        }
        return layer;
    }

    IEnumerator GeneratePath(GameObject[,] cubes, int moduleIndex)
    {
        // Calcular posición inicial del camino según el módulo anterior
        int curX = moduleIndex == 0 ? mapWidth / 2 : lastExit.x; // Centro para el primer módulo, lastExit para los siguientes
        int curZ = moduleIndex == 0 ? mapHeight / 2 : lastExit.y; // Usar lastExit.y para módulos posteriores
        CurrentDirection curDirection = moduleIndex == 0 ? CurrentDirection.DOWN : lastDirection;

        bool hasDecidedDirection = false; // Solo decidir dirección una vez, en la mitad del módulo

        while (true)
        {
            // Eliminar cubo actual para formar el camino
            if (cubes[curX, curZ] != null)
            {
                Destroy(cubes[curX, curZ]);
                cubes[curX, curZ] = null;
            }

       // Evitar retrocesos en la dirección
CurrentDirection previousDirection = curDirection;

// Decidir dirección en la mitad del módulo
if (curZ == mapHeight / 2 && curX == mapWidth / 2 && !hasDecidedDirection)
{
    int randomDirection = Random.Range(0, 3); // 0: recto, 1: izquierda, 2: derecha
    if (randomDirection == 1 && curX > 1 && previousDirection != CurrentDirection.RIGHT) // Girar a la izquierda
    {
        curDirection = CurrentDirection.LEFT;
    }
    else if (randomDirection == 2 && curX < mapWidth - 2 && previousDirection != CurrentDirection.LEFT) // Girar a la derecha
    {
        curDirection = CurrentDirection.RIGHT;
    }
    else
    {
        curDirection = CurrentDirection.DOWN; // Continuar recto
    }
    hasDecidedDirection = true;
}


            // Continuar en la dirección actual
            switch (curDirection)
            {
                case CurrentDirection.LEFT:
                    curX = Mathf.Max(0, curX - 1); // Evitar bordes extremos
                    break;
                case CurrentDirection.RIGHT:
                    curX = Mathf.Min(mapWidth - 1, curX + 1); // Evitar bordes extremos
                    break;
                case CurrentDirection.DOWN:
                    curZ++;
                    break;
            }

            // Comprobar si hemos alcanzado un borde
            if (curZ == mapHeight - 1 || curX == 0 || curX == mapWidth - 1)
            {
                // Asegurarte de destruir el último cubo antes de registrar la salida
                if (cubes[curX, curZ] != null)
                {
                    Destroy(cubes[curX, curZ]);
                    cubes[curX, curZ] = null;
                }

                // Registrar la salida con coordenadas correctas
                lastExit = new Vector2Int(curX, curZ); // Guardar tanto X como Z
                lastDirection = curX == 0 ? CurrentDirection.LEFT :
                                curX == mapWidth - 1 ? CurrentDirection.RIGHT :
                                CurrentDirection.DOWN;

                // Decidir posición del próximo módulo
                DecideNextModulePosition(curX, curZ, curDirection);
                Debug.Log($"Camino termina en ({curX}, {curZ}), dirección: {curDirection}");
                break;
            }

            yield return new WaitForSeconds(0.05f); // Velocidad de generación del camino
        }
    }



    void DecideNextModulePosition(int exitX, int exitZ, CurrentDirection exitDirection)
    {
        // Calcular la posición del siguiente módulo basado en la salida
        switch (exitDirection)
        {
            case CurrentDirection.DOWN:
                nextModulePosition = new Vector3(nextModulePosition.x, 0, nextModulePosition.z + mapHeight * 1.3f);
                break;
            case CurrentDirection.LEFT:
                nextModulePosition = new Vector3(nextModulePosition.x - mapWidth * 1.3f, 0, nextModulePosition.z);
                break;
            case CurrentDirection.RIGHT:
                nextModulePosition = new Vector3(nextModulePosition.x + mapWidth * 1.3f, 0, nextModulePosition.z);
                break;
        }

        // Ajustar la posición inicial del camino para el siguiente módulo
        lastExit = new Vector2Int(
            exitDirection == CurrentDirection.LEFT ? mapWidth - 1 : // Entrada desde la derecha
            exitDirection == CurrentDirection.RIGHT ? 0 : // Entrada desde la izquierda
            exitX, // Continuar recto si el módulo está alineado verticalmente
            exitZ == mapHeight - 1 ? 0 : exitZ // Si salió por el borde superior, el camino empieza en Z = 0
        );

        Debug.Log($"Siguiente módulo comienza en {nextModulePosition}, entrada en {lastExit}, dirección {exitDirection}");
    }





}
