using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryManager
{
    private static BoundaryManager instance;
    public static BoundaryManager Instance
    { 
        get
        {
            if (instance == null)
                instance = new BoundaryManager();
            return instance;
        }
    }

    private List<Boundary> boundaryList = new List<Boundary>();

    public static void RegisterBoundary(Boundary self)
    {
        if (Instance.boundaryList.Contains(self))
            return;

        Instance.boundaryList.Add(self);
    }

    public static Transform GetBoundaryAtPoint(Vector3 position)
    {
        foreach (var boundary in Instance.boundaryList)
        {
            if (boundary.Collider.bounds.Contains(position))
                return boundary.transform;
        }

        throw new System.Exception("The requested point is outside all boundaries");
    }
}
