using UnityEngine;

public class ModuleInfo
{
    public Vector3 NextModulePosition { get; set; }
    public CurrentDirection LastDirection { get; set; }

    public ModuleInfo(Vector3 nextModulePosition, CurrentDirection lastDirection)
    {
        NextModulePosition = nextModulePosition;
        LastDirection = lastDirection;
    }

    public override string ToString()
    {
        return $"NextModulePosition: {NextModulePosition}, LastDirection: {LastDirection}";
    }
}
