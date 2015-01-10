using UnityEngine;
using System.Collections;

public class MapObject
{
    public int ID
    {
        get
        {
            return this.BoronObject.GetHashCode();
        }
    }
    public Vector3 Position 
    { 
        get
        {
            return this.BoronObject.transform.position;
        }
    }
    protected MonoBehaviour BoronObject
    {
        get;
        private set;
    }

    public MapObject(MonoBehaviour brnObj)
    {
        this.BoronObject = brnObj;
    }
}
