using System.Collections;
using UnityEngine;

public class BranchPathGenerator
{
    private readonly IMapGenerator mapGenerator;
    private readonly IObjectPool pool;

    public BranchPathGenerator(IMapGenerator mapGenerator, IObjectPool pool)
    {
        this.mapGenerator = mapGenerator;
        this.pool = pool;
    }

    public IEnumerator GenerateBranch(GameObject[,] cubes, int startX, int startZ, CurrentDirection direction)
    {
        int curX = startX;
        int curZ = startZ;

        while (!HasReachedBoundary(curX, curZ))
        {
            ClearTile(cubes, curX, curZ);

            MoveToNextTile(ref curX, ref curZ, direction);

            if (HasReachedBoundary(curX, curZ))
            {
                yield break;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    private void ClearTile(GameObject[,] cubes, int x, int z)
    {
        if (cubes[x, z] != null)
        {
            pool.ReturnObject(cubes[x, z]);
            cubes[x, z] = null;
        }
    }

    private void MoveToNextTile(ref int curX, ref int curZ, CurrentDirection curDirection)
    {
        if (curDirection == CurrentDirection.LEFT)
        {
            curX = Mathf.Max(0, curX - 1);
        }
        else if (curDirection == CurrentDirection.RIGHT)
        {
            curX = Mathf.Min(mapGenerator.MapWidth - 1, curX + 1);
        }
        else if (curDirection == CurrentDirection.DOWN)
        {
            curZ++;
        }
    }

    private bool HasReachedBoundary(int curX, int curZ)
    {
        return curZ == mapGenerator.MapHeight - 1 || curX == 0 || curX == mapGenerator.MapWidth - 1;
    }
}
