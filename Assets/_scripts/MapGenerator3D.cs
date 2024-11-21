using System.Collections;
using UnityEngine;

public class MapGenerator3D : MonoBehaviour, IMapGenerator
{
    public static MapGenerator3D Instance { get; private set; }

    [Header("Map Configuration")]
    [SerializeField] private int mapWidth = 13;
    [SerializeField] private int mapHeight = 13;
    [SerializeField] private float spacing = 1.2f;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Material groundMaterial;
    [SerializeField] private Material grassMaterial;
    [SerializeField] private int numModules = 3;
     private ModuleGenerator module;
    [SerializeField] private float moduleSpacing = 1.2f;


    [SerializeField] private float cenetr = 13;
    [SerializeField] private float irregu = 13;


    [Header("Seed Configuration")]
    [SerializeField] private int seed = 0;

    private Vector3 nextModulePosition = Vector3.zero;
    private Vector3 newNextModulePosition ;
    private ObjectPool pool;
    private PathGenerator pathGenerator;

    public int MapWidth => mapWidth;
    public int MapHeight => mapHeight;
    public float Spacing => spacing;
    public PathGenerator PathGenerator => pathGenerator;
    public Vector3 NextModulePosition => nextModulePosition;
    public Material GroundMaterial => groundMaterial;
    public Material GrassMaterial => grassMaterial;
    public Vector2Int LastExit { get; set; }
    public CurrentDirection LastDirection { get; set; } = CurrentDirection.DOWN;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            pool = new ObjectPool(cubePrefab, mapWidth * mapHeight * numModules);
            pathGenerator = new PathGenerator(this, pool);
            module = new ModuleGenerator(this, pool);
        }
    }



    void Start()
    {
        Random.InitState(seed);
        if (module == null)
        {
         
            return;
        }
        StartCoroutine(GenerateModules());
    }

    private IEnumerator GenerateModules()
    {
        
            yield return StartCoroutine(module.GenerateModule(numModules));
        
    }

    public void ExecuteCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void DecideNextModulePosition(int exitX, int exitZ, CurrentDirection exitDirection)
    {
        float offsetX = mapWidth * spacing;
        float offsetZ = mapHeight * spacing;

        // Calcular posición del siguiente módulo en coordenadas globales
        nextModulePosition = exitDirection switch
        {
            CurrentDirection.DOWN => new Vector3(
                nextModulePosition.x,
                0,
                nextModulePosition.z + offsetZ + moduleSpacing
            ),
            CurrentDirection.LEFT => new Vector3(
                nextModulePosition.x - offsetX - moduleSpacing,
                0,
                nextModulePosition.z
            ),
            CurrentDirection.RIGHT => new Vector3(
                nextModulePosition.x + offsetX + moduleSpacing,
                0,
                nextModulePosition.z
            ),
            _ => nextModulePosition
        };

        // Actualizar la última salida relativa al borde del módulo
        LastExit = exitDirection switch
        {
            CurrentDirection.DOWN => new Vector2Int(exitX, 0),
            CurrentDirection.LEFT => new Vector2Int(mapWidth - 1, exitZ),
            CurrentDirection.RIGHT => new Vector2Int(0, exitZ),
            _ => new Vector2Int(exitX, exitZ)
        };

       
    }




}
