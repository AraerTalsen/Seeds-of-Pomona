using System.Collections.Generic;
public static class TopologicalSort 
{
    private static readonly Dictionary<int, bool> circularDepData = new Dictionary<int, bool>();
    private static readonly List<IsoSpriteSorting> circularDepStack = new List<IsoSpriteSorting>(64);

    private static readonly HashSet<int> visited = new HashSet<int>();
    private static readonly List<IsoSpriteSorting> allSprites = new List<IsoSpriteSorting>(64);

    //Sorts all sprites by nearest order to the camera, after first breaking infinite dependency loops
    public static List<IsoSpriteSorting> Sort(List<IsoSpriteSorting> staticSprites, List<IsoSpriteSorting> movableSprites, List<IsoSpriteSorting> sorted) 
    {
        allSprites.Clear();
        allSprites.AddRange(movableSprites);
        allSprites.AddRange(staticSprites);

        int allSpriteCount = allSprites.Count;

        for (int i = 0; i < 5; i++) 
        {
            circularDepStack.Clear();
            circularDepData.Clear();
            bool removedDependency = false;
            for (int j = 0; j < allSpriteCount; j++) 
            {
                if (RemoveCircularDependencies(allSprites[j], circularDepStack, circularDepData)) 
                {
                    removedDependency = true;
                }
            }
            if (!removedDependency) 
            {
                break;
            }
        }

        visited.Clear();
        for (int i = 0; i < allSpriteCount; i++) 
        {
            Visit(allSprites[i], sorted, visited);
        }

        return sorted;
    }

    //Visits each sprite either iteratively or recursively and adds it to the sorted list when it has visited all of its dependencies (finds the sprite at
    //the back of the overlapping pile)
    private static void Visit(IsoSpriteSorting item, List<IsoSpriteSorting> sorted, HashSet<int> visited) 
    {
        int id = item.GetInstanceID();
        if (!visited.Contains(id)) 
        {
            visited.Add(id);

            List<IsoSpriteSorting> dependencies = item.VisibleMovingDependencies;
            int dcount = dependencies.Count;
            for (int i = 0; i < dcount; i++) 
            {
                Visit(dependencies[i], sorted, visited);
            }
            dependencies = item.VisibleStaticDependencies;
            dcount = dependencies.Count;
            for (int i = 0; i < dcount; i++) 
            {
                Visit(dependencies[i], sorted, visited);
            }

            sorted.Add(item);
        }
    }

    //Attempt to remove any circular dependencies of overlapping sprites if they exist
    private static bool RemoveCircularDependencies(IsoSpriteSorting item, List<IsoSpriteSorting> _circularDepStack, Dictionary<int, bool> _circularDepData) 
    {
        _circularDepStack.Add(item);
        bool removedDependency = false;

        int id = item.GetInstanceID();
        bool alreadyVisited = _circularDepData.TryGetValue(id, out bool inProcess);
        if (alreadyVisited) 
        {
            if (inProcess) 
            {
                RemoveCircularDependencyFromStack(_circularDepStack);
                removedDependency = true;
            }
        } 
        else 
        {
            _circularDepData[id] = true;

            List<IsoSpriteSorting> dependencies = item.VisibleMovingDependencies;
            for (int i = 0; i < dependencies.Count; i++) 
            {
                if (RemoveCircularDependencies(dependencies[i], _circularDepStack, _circularDepData)) 
                {
                    removedDependency = true;
                }
            }
            dependencies = item.VisibleStaticDependencies;
            for (int i = 0; i < dependencies.Count; i++) 
            {
                if (RemoveCircularDependencies(dependencies[i], _circularDepStack, _circularDepData)) 
                {
                    removedDependency = true;
                }
            }

            _circularDepData[id] = false;
        }

        _circularDepStack.RemoveAt(_circularDepStack.Count - 1);
        return removedDependency;
    }

    //In isometric view, it’s possible to have three or more sprites that "overlap" each other in a loop (A is behind B, B is behind C, and C is behind A).
    //This finds the loop and identifies which two dependent sprites in it have the greatest horizontal distance. The theory is that breaking the loop at
    //the sprites with the greatest horizontal distance will be least noticable to the player
    private static void RemoveCircularDependencyFromStack(List<IsoSpriteSorting> _circularReferenceStack) 
    {
        if (_circularReferenceStack.Count > 1) 
        {
            //Iterating the list in reverse order, find the index of the element that is a repetition of the last
            IsoSpriteSorting startingSorter = _circularReferenceStack[_circularReferenceStack.Count - 1];
            int repeatIndex = 0;
            for (int i = _circularReferenceStack.Count - 2; i >= 0; i--) 
            {
                IsoSpriteSorting sorter = _circularReferenceStack[i];
                if (sorter == startingSorter) 
                {
                    repeatIndex = i;
                    break;
                }
            }

            //Starting at the repeatIndex, iterate forward and compare each element to its neighbor if they are both point-types, save the element's index
            //with the greatest x-axis distance from its neighbor
            int weakestDepIndex = -1;
            float longestDistance = float.MinValue;
            for (int i = repeatIndex; i < _circularReferenceStack.Count - 1; i++) 
            {
                IsoSpriteSorting sorter1a = _circularReferenceStack[i];
                IsoSpriteSorting sorter2a = _circularReferenceStack[i + 1];
                if (sorter1a.sortType == IsoSpriteSorting.SortType.Point && sorter2a.sortType == IsoSpriteSorting.SortType.Point) 
                {
                    float dist = UnityEngine.Mathf.Abs(sorter1a.AsPoint.x - sorter2a.AsPoint.x);
                    if (dist > longestDistance) 
                    {
                        weakestDepIndex = i;
                        longestDistance = dist;
                    }
                }
            }
            //Same as above, except only do it if no neighbors were point-types, and this time compare them regardless of type
            if (weakestDepIndex == -1) 
            {
                for (int i = repeatIndex; i < _circularReferenceStack.Count - 1; i++) 
                {
                    IsoSpriteSorting sorter1a = _circularReferenceStack[i];
                    IsoSpriteSorting sorter2a = _circularReferenceStack[i + 1];
                    float dist = UnityEngine.Mathf.Abs(sorter1a.AsPoint.x - sorter2a.AsPoint.x);
                    if (dist > longestDistance) 
                    {
                        weakestDepIndex = i;
                        longestDistance = dist;
                    }
                }
            }
            IsoSpriteSorting sorter1 = _circularReferenceStack[weakestDepIndex];
            IsoSpriteSorting sorter2 = _circularReferenceStack[weakestDepIndex + 1];
            sorter1.VisibleStaticDependencies.Remove(sorter2);
            sorter1.VisibleMovingDependencies.Remove(sorter2);
        }
    }
}
