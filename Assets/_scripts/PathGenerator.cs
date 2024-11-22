using System.Collections;
using UnityEngine;
using System.Collections.Generic;






//Generates Paths throuhg the modules cubes
public class PathGenerator 
{
    private readonly IMapGenerator _mapGenerator;
    private readonly IObjectPool _pool;
    private int _mapWidth;
    private int _mapHeight;
  
    System.Random random = new System.Random();

    public  PathGenerator(IMapGenerator mapGenerator, IObjectPool pool)
    {
        this._mapGenerator = mapGenerator;
        this._pool = pool;
        this._mapWidth = mapGenerator.MapWidth;
        this._mapHeight = mapGenerator.MapHeight;
       
    }


        public IEnumerator GeneratePath(GameObject[,] cubes, int moduleIndex, CurrentDirection exitDirection)
    {
        Debug.Log("module index - " + moduleIndex);
        Vector2Int lastExit = GetInitialExit(moduleIndex);
        CurrentDirection curDirection = GetInitialDirection(moduleIndex);
        CurrentDirection origDirection = curDirection;
        HashSet<CurrentDirection> usedDirections = new HashSet<CurrentDirection> { curDirection };

        int curX = lastExit.x;
        int curZ = lastExit.y;
        int maxSteps = _mapWidth + _mapHeight;

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
            
            //Creates new branch when there is a turn and a probabilty
            if (curDirection == CurrentDirection.LEFT || curDirection == CurrentDirection.RIGHT)
            {
                

                CurrentDirection branchDirection = GetBranchDirection(curDirection, origDirection);

                if (moduleIndex % 3 == 0 && moduleIndex != 0)
                {
               
                    bool isBranch = ThreeUniqueDirections(curDirection, origDirection, branchDirection);
                    CoroutineManager.Instance.StartManagedCoroutine(GenerateBranch(cubes, curX, curZ, branchDirection, isBranch));

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
    private CurrentDirection GetBranchDirection(CurrentDirection curDirection, CurrentDirection origDirection)
    {
        // Determine the third direction by excluding curDirection and origDirection
        if (curDirection != CurrentDirection.LEFT && origDirection != CurrentDirection.LEFT)
        {
            return CurrentDirection.LEFT;
        }
        else if (curDirection != CurrentDirection.RIGHT && origDirection != CurrentDirection.RIGHT)
        {
            return CurrentDirection.RIGHT;
        }
        else
        {
            return CurrentDirection.DOWN; 
        }
    }
    private bool ThreeUniqueDirections(CurrentDirection curDirection, CurrentDirection origDirection, CurrentDirection branchDirection)
    {
        return curDirection != origDirection &&
               curDirection != branchDirection &&
               origDirection != branchDirection;
    }
    //Gets where was the last exit of the path in the last module
    private Vector2Int GetInitialExit(int moduleIndex)
    {
        return moduleIndex == 0
            ? new Vector2Int(_mapWidth / 2, _mapHeight / 2)
            : _mapGenerator.LastExit;
    }
    //Gets where was the direction exit of the path in the last module
    private CurrentDirection GetInitialDirection(int moduleIndex)
    {
        return moduleIndex == 0
            ? CurrentDirection.DOWN
            : _mapGenerator.LastDirection;
    }

    //Open the path setting off the cubes on de chunk grid
    public void ClearTile(GameObject[,] cubes, int x, int z)
    {
        if (cubes[x, z] != null)
        {
            _pool.ReturnObject(cubes[x, z]);
            cubes[x, z] = null;
        }
    }
    
    //Detects the center
    private bool IsAtCenter(int x, int z)
    {
        return z == _mapHeight / 2 && x == _mapWidth / 2;
    }

    bool isRepeating = false;

    //Avoids repetition of last direction 
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
    //Chooses direction
    private CurrentDirection GetDirectionFromRandomValue(int value, int curX, int curZ, CurrentDirection curDirection)
    {
        bool canGoLeft = value == 1 && curX > 1 && curDirection != CurrentDirection.RIGHT;
        bool canGoRight = value == 2 && curX < _mapWidth - 2 && curDirection != CurrentDirection.LEFT;
        bool canGoDown = value == 0 && curZ < _mapHeight - 1;

        if (canGoLeft) return CurrentDirection.LEFT;
        if (canGoRight) return CurrentDirection.RIGHT;
        if (canGoDown) return CurrentDirection.DOWN;

        return curDirection;
    }

    //Cleans the direction history
    private void ResetDirectionHistory(HashSet<CurrentDirection> usedDirections, CurrentDirection curDirection)
    {
        usedDirections.Clear();
        usedDirections.Add(curDirection);
    }
    //moves through the grid
    public void MoveToNextTile(ref int curX, ref int curZ, CurrentDirection curDirection)
    {
        if (curDirection == CurrentDirection.LEFT)
        {
            curX = Mathf.Max(0, curX - 1);
        }
        else if (curDirection == CurrentDirection.RIGHT)
        {
            curX = Mathf.Min(_mapWidth - 1, curX + 1);
        }
        else if (curDirection == CurrentDirection.DOWN)
        {
            curZ++;
        }
    }
    //Detects the end of the module
    public bool HasReachedBoundary(int curX, int curZ)
    {
        return curZ == _mapHeight - 1 || curX == 0 || curX == _mapWidth - 1;
    }
    //Sends the info of the path ending to create a new one
    private void FinalizePath(GameObject[,] cubes, int curX, int curZ, CurrentDirection curDirection)
    {
        ClearTile(cubes, curX, curZ);

        _mapGenerator.LastExit = new Vector2Int(curX, curZ);
        _mapGenerator.LastDirection = curDirection;

        _mapGenerator.DecideNextModulePosition(curX, curZ, curDirection);
    }
    //Generates a new branch on the path
    private IEnumerator GenerateBranch(GameObject[,] cubes, int startX, int startZ, CurrentDirection direction, bool isBranch)
    {
     
        
        int curX = startX;
        int curZ = startZ;

        while (!HasReachedBoundary(curX, curZ))
        {
            ClearTile(cubes, curX, curZ);

            MoveToNextTile(ref curX, ref curZ, direction );

            if (HasReachedBoundary(curX, curZ))
            {
                ClearTile(cubes, curX, curZ);
            
               
               if(isBranch)
                {
                  //FinalizePath(cubes, curX, curZ, direction);    Fix Bug
                }
                
         
                yield break;
            }

            yield return new WaitForSeconds(0.07f);
        }
    }
}