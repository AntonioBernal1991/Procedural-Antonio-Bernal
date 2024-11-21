using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PathGenerator
{
    private readonly IMapGenerator mapGenerator;
    private readonly IObjectPool pool;
    private int mapWidth;
    private int mapHeight;

    public PathGenerator(IMapGenerator mapGenerator, IObjectPool pool)
    {
        this.mapGenerator = mapGenerator;
        this.pool = pool;
        this.mapWidth = mapGenerator.MapWidth;
        this.mapHeight = mapGenerator.MapHeight;
    }

    public IEnumerator GeneratePath(GameObject[,] cubes, int moduleIndex, CurrentDirection exitDirection)
    {
        Vector2Int lastExit = GetInitialExit(moduleIndex);
        CurrentDirection curDirection = GetInitialDirection(moduleIndex);

        HashSet<CurrentDirection> usedDirections = new HashSet<CurrentDirection> { curDirection };

        int curX = lastExit.x;
        int curZ = lastExit.y;
        int maxSteps = mapWidth + mapHeight;

        for (int step = 0; step < maxSteps; step++)
        {
            ClearTile(cubes, curX, curZ);

            if (IsAtCenter(curX, curZ))
            {
                curDirection = DetermineNextDirection(curX, curZ, curDirection, usedDirections);

                // Reset direction history if all directions are used
                if (usedDirections.Count == 3)
                {
                    ResetDirectionHistory(usedDirections, curDirection);
                }
            }
            int randomDBot = Random.Range(0, 100);
            // Si gira, inicia una rama adicional
            if (curDirection == CurrentDirection.LEFT || curDirection == CurrentDirection.RIGHT  )
            {
                CurrentDirection branchDirection = curDirection == CurrentDirection.LEFT
                    ? CurrentDirection.RIGHT
                    : CurrentDirection.LEFT;

                if (moduleIndex  % 5 == 0 && moduleIndex != 0)
                {
                    MapGenerator3D.Instance.ExecuteCoroutine(GenerateBranch(cubes, curX, curZ, branchDirection));
                }
                   
            }

            MoveToNextTile(ref curX, ref curZ, curDirection);

            if (HasReachedBoundary(curX, curZ))
            {
                FinalizePath(cubes, curX, curZ, curDirection);
                yield break;
            }

            yield return new WaitForSeconds(0.07f);
        }
    }

    private Vector2Int GetInitialExit(int moduleIndex)
    {
        return moduleIndex == 0
            ? new Vector2Int(mapWidth / 2, mapHeight / 2)
            : mapGenerator.LastExit;
    }

    private CurrentDirection GetInitialDirection(int moduleIndex)
    {
        return moduleIndex == 0
            ? CurrentDirection.DOWN
            : mapGenerator.LastDirection;
    }

    private void ClearTile(GameObject[,] cubes, int x, int z)
    {
        if (cubes[x, z] != null)
        {
            pool.ReturnObject(cubes[x, z]);
            cubes[x, z] = null;
        }
    }

    private bool IsAtCenter(int x, int z)
    {
        return z == mapHeight / 2 && x == mapWidth / 2;
    }

    bool isRepeating= false;

    private CurrentDirection DetermineNextDirection(int curX, int curZ, CurrentDirection curDirection, HashSet<CurrentDirection> usedDirections)
    {
        CurrentDirection randomDirection = GetDirectionFromRandomValue(Random.Range(0, 3), curX, curZ, curDirection);

        if (randomDirection == curDirection)
        {
            if (!isRepeating)
            {
                isRepeating = true;
                usedDirections.Add(randomDirection);
                return randomDirection;
            }
            else
            {
                return DetermineNextDirection(curX, curZ, randomDirection, usedDirections);
            }
        }
        else
        {
            isRepeating = false;
            usedDirections.Add(randomDirection);
            return randomDirection;
        }
    }

    private CurrentDirection GetDirectionFromRandomValue(int value, int curX, int curZ, CurrentDirection curDirection)
    {
        bool canGoLeft = value == 1 && curX > 1 && curDirection != CurrentDirection.RIGHT;
        bool canGoRight = value == 2 && curX < mapWidth - 2 && curDirection != CurrentDirection.LEFT;
        bool canGoDown = value == 0 && curZ < mapHeight - 1;

        if (canGoLeft) return CurrentDirection.LEFT;
        if (canGoRight) return CurrentDirection.RIGHT;
        if (canGoDown) return CurrentDirection.DOWN;

        return curDirection;
    }

    private void ResetDirectionHistory(HashSet<CurrentDirection> usedDirections, CurrentDirection curDirection)
    {
        usedDirections.Clear();
        usedDirections.Add(curDirection);
    }

    private void MoveToNextTile(ref int curX, ref int curZ, CurrentDirection curDirection)
    {
        if (curDirection == CurrentDirection.LEFT)
        {
            curX = Mathf.Max(0, curX - 1);
        }
        else if (curDirection == CurrentDirection.RIGHT)
        {
            curX = Mathf.Min(mapWidth - 1, curX + 1);
        }
        else if (curDirection == CurrentDirection.DOWN)
        {
            curZ++;
        }
    }

    private bool HasReachedBoundary(int curX, int curZ)
    {
        return curZ == mapHeight - 1 || curX == 0 || curX == mapWidth - 1;
    }

    private void FinalizePath(GameObject[,] cubes, int curX, int curZ, CurrentDirection curDirection)
    {
        ClearTile(cubes, curX, curZ);

        mapGenerator.LastExit = new Vector2Int(curX, curZ);
        mapGenerator.LastDirection = curDirection;

        mapGenerator.DecideNextModulePosition(curX, curZ, curDirection);
    }

    private IEnumerator GenerateBranch(GameObject[,] cubes, int startX, int startZ, CurrentDirection direction)
    {
        int curX = startX;
        int curZ = startZ;

        while (!HasReachedBoundary(curX, curZ))
        {
            ClearTile(cubes, curX, curZ);

            MoveToNextTile(ref curX, ref curZ, direction);

            if (HasReachedBoundary(curX, curZ))
            {
                ClearTile(cubes, curX, curZ);
                yield return new WaitForSeconds(0.07f);
                
                yield break;
            }

            yield return new WaitForSeconds(0.07f);
        }
    }
}
