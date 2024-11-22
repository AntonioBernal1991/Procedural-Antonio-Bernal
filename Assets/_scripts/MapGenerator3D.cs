using System.Collections;
using UnityEngine;


//main class keeps the info , inictiate the other clases and acts as a node of position for the modules and paths
public class MapGenerator3D : MonoBehaviour, IMapGenerator
{
    public static MapGenerator3D Instance { get; private set; }

    [Header("Map Configuration")]
    [SerializeField] private int _chunkWidth = 13;
    [SerializeField] private int _chunkHeight = 13;
    [Header("Seed Configuration")]
    [SerializeField] private int _seed = 0;
    [Header("Number of Modules Configuration")]
    [SerializeField] private int _numModules = 3;
    [Header("                                  ")]
    [SerializeField] private float _spacing = 1.2f;
    [SerializeField] private GameObject _cubePrefab;
    [SerializeField] private Material _groundMaterial;
    [SerializeField] private Material _grassMaterial;
    [SerializeField] private float _moduleSpacing = 1.2f;



    private ModuleGenerator module;
    private Vector3 nextModulePosition = Vector3.zero;
    private ObjectPool pool;
    private PathGenerator pathGenerator;
   
    public int MapWidth => _chunkWidth;
    public int MapHeight => _chunkHeight;
    public float Spacing =>_spacing;
    public PathGenerator PathGenerator => pathGenerator;
    public Vector3 NextModulePosition => nextModulePosition;
    public Material GroundMaterial => _groundMaterial;
    public Material GrassMaterial => _grassMaterial;
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
            pool = new ObjectPool(_cubePrefab, _chunkWidth * _chunkHeight * _numModules);
            pathGenerator = new PathGenerator(this, pool);    
            module = new ModuleGenerator(this, pool);
        }
    }



    void Start()
    {
        
        Vector3 modposCopy = new Vector3(nextModulePosition.x, nextModulePosition.y, nextModulePosition.z);
        CurrentDirection myLastDirection  = CurrentDirection.DOWN;
        ModuleInfo myModuleInfo = new ModuleInfo(modposCopy,myLastDirection);
        ModuleInfoQueueManager.Enqueue(myModuleInfo);
        Random.InitState(_seed);
        if (module == null)
        {  
            return;
        }
        StartCoroutine(GenerateModules());

    }
   
    //Start creating the modules
    private IEnumerator GenerateModules()
    {
        yield return StartCoroutine(module.StartRecursiveGeneration(_numModules));
    }

    //Node that gets info of the potsition oof the path and module and sets the new position for continuing the path
    public void DecideNextModulePosition(int exitX, int exitZ, CurrentDirection exitDirection)
    {
        float offsetX = _chunkWidth * _spacing;
        float offsetZ = _chunkHeight * _spacing;

        // Calculate global module postion
        nextModulePosition = exitDirection switch
        {
            CurrentDirection.DOWN => new Vector3(
                nextModulePosition.x,
                0,
                nextModulePosition.z + offsetZ + _moduleSpacing
            ),
            CurrentDirection.LEFT => new Vector3(
                nextModulePosition.x - offsetX - _moduleSpacing,
                0,
                nextModulePosition.z
            ),
            CurrentDirection.RIGHT => new Vector3(
                nextModulePosition.x + offsetX + _moduleSpacing,
                0,
                nextModulePosition.z
            ),
            _ => nextModulePosition
        };
      
        Vector3 modposCopy = new Vector3(nextModulePosition.x, nextModulePosition.y, nextModulePosition.z);
        CurrentDirection myCurrentDirection = exitDirection;
        ModuleInfo myModuleInfo = new ModuleInfo(modposCopy, myCurrentDirection);
        ModuleInfoQueueManager.Enqueue(myModuleInfo);

        //Updates the last exit of the path so it knows where to continue on the nex module
        LastExit = exitDirection switch
        {
            CurrentDirection.DOWN => new Vector2Int(exitX, 0),
            CurrentDirection.LEFT => new Vector2Int(_chunkWidth - 1, exitZ),
            CurrentDirection.RIGHT => new Vector2Int(0, exitZ),
            _ => new Vector2Int(exitX, exitZ)
        };

       
    }




}
