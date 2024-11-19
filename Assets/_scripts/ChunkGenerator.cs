using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Chunk Configuration")]
    public int chunkSizeX = 13; // Ancho del chunk
    public int chunkSizeZ = 13; // Largo del chunk
    public float spacing = 1.2f; // Espaciado entre los cubos
    public int numChunks = 4; // Número total de chunks a generar
    public GameObject terrainCubePrefab; // Prefab reutilizable

    [Header("Materials")]
    public Material grassMaterial; // Material para la capa superior (camino)
    public Material groundMaterial; // Material para la capa inferior (suelo)

    private Vector3 currentChunkPosition = Vector3.zero; // Posición inicial del chunk
    private Vector2Int lastExitPosition; // Posición de salida del último chunk
    private Vector2Int lastDirection = Vector2Int.up; // Dirección del último tramo del camino
    private int chunkCounter = 1; // Contador de chunks

    void Start()
    {
        GenerateChunks();
    }

    void GenerateChunks()
    {
        for (int i = 0; i < numChunks; i++)
        {
            if (i == 0)
            {
                // Primer chunk con camino fijo
                GenerateChunk(currentChunkPosition, true, Vector2Int.up);
            }
            else
            {
                // Chunks con camino dinámico
                GenerateChunk(currentChunkPosition, false, lastDirection);
            }

            // Decidir posición del siguiente chunk
            DecideNextChunkPosition();
        }
    }

    void GenerateChunk(Vector3 position, bool isFirstChunk, Vector2Int entryDirection)
    {
        GameObject chunk = new GameObject($"Chunk_{chunkCounter}");
        chunkCounter++; // Incrementar contador para el próximo chunk
        chunk.transform.position = position;

        // Generar la capa de terreno y el camino
        GenerateGroundLayer(position, chunk);
        GenerateGrassLayer(position, chunk, isFirstChunk, entryDirection);
    }

    void GenerateGroundLayer(Vector3 position, GameObject chunk)
    {
        // Generar una capa completa de suelo (ground)
        for (int x = 0; x < chunkSizeX; x++)
        {
            for (int z = 0; z < chunkSizeZ; z++)
            {
                Vector3 cubePosition = new Vector3(x * spacing, 0, z * spacing) + position;
                GameObject groundCube = Instantiate(terrainCubePrefab, cubePosition, Quaternion.identity, chunk.transform);
                groundCube.GetComponent<Renderer>().material = groundMaterial;
            }
        }
    }

    void GenerateGrassLayer(Vector3 position, GameObject chunk, bool isFirstChunk, Vector2Int entryDirection)
    {
        // Crear todo el terreno (capa verde)
        GameObject[,] grassGrid = new GameObject[chunkSizeX, chunkSizeZ];

        for (int x = 0; x < chunkSizeX; x++)
        {
            for (int z = 0; z < chunkSizeZ; z++)
            {
                Vector3 cubePosition = new Vector3(x * spacing, spacing, z * spacing) + position;
                grassGrid[x, z] = Instantiate(terrainCubePrefab, cubePosition, Quaternion.identity, chunk.transform);
                grassGrid[x, z].GetComponent<Renderer>().material = grassMaterial;
            }
        }

        // Generar el camino
        GeneratePath(grassGrid, position, isFirstChunk, entryDirection);
    }

    void GeneratePath(GameObject[,] grassGrid, Vector3 position, bool isFirstChunk, Vector2Int entryDirection)
    {
        int startX = chunkSizeX / 2;
        int startZ = 0;

        if (!isFirstChunk)
        {
            // Usar la última posición de salida como entrada
            startX = lastExitPosition.x;
            startZ = (entryDirection == Vector2Int.up) ? 0 : chunkSizeZ - 1;
        }

        int currentX = startX;
        int currentZ = startZ;

        while (currentZ < chunkSizeZ)
        {
            if (grassGrid[currentX, currentZ] != null)
            {
                Destroy(grassGrid[currentX, currentZ]); // Destruir cubo para crear el camino
            }

            if (currentZ == chunkSizeZ / 2 && !isFirstChunk)
            {
                // Decidir dirección al llegar al centro
                int randomDirection = Random.Range(0, 3); // 0: recto, 1: izquierda, 2: derecha

                if (randomDirection == 1 && currentX > 0) // Girar a la izquierda
                {
                    currentX--;
                    lastDirection = Vector2Int.left;
                    Debug.Log($"Chunk {chunkCounter}: Giro a la izquierda");
                }
                else if (randomDirection == 2 && currentX < chunkSizeX - 1) // Girar a la derecha
                {
                    currentX++;
                    lastDirection = Vector2Int.right;
                    Debug.Log($"Chunk {chunkCounter}: Giro a la derecha");
                }
                else // Seguir recto
                {
                    currentZ++;
                    lastDirection = Vector2Int.up;
                    Debug.Log($"Chunk {chunkCounter}: Continúa recto");
                }
            }
            else
            {
                // Continuar recto hacia el borde
                currentZ++;
            }
        }

        // Guardar posición de salida
        lastExitPosition = new Vector2Int(currentX, chunkSizeZ - 1);
        Debug.Log($"Chunk {chunkCounter}: Camino sale en {lastExitPosition}");
    }

    void DecideNextChunkPosition()
    {
        // Calcular posición del siguiente chunk en función de la última dirección
        currentChunkPosition += new Vector3(
            lastDirection.x * chunkSizeX * spacing,
            0,
            lastDirection.y * chunkSizeZ * spacing
        );
    }
}
