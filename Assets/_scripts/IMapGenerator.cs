
using System.Collections;
using UnityEngine;


public interface IMapGenerator
{
    int MapWidth { get; }
    int MapHeight { get; }
    float Spacing { get; }
    Vector3 NextModulePosition { get; }
    Material GroundMaterial { get; }
    Material GrassMaterial { get; }
    void DecideNextModulePosition(int exitX, int exitZ, CurrentDirection exitDirection);

    
    Vector2Int LastExit { get; set; }
    CurrentDirection LastDirection { get; set; }
    PathGenerator PathGenerator { get; }
}


public interface IObjectPool
{
    GameObject GetObject();
    void ReturnObject(GameObject obj);
}
