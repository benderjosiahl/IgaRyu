using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public static Transform[] points;

    public List<Transform> waypoints;

    void Awake()
    {
        //points = new Transform[transform.childCount];
        for (int i = 0; i < waypoints.Count; i++)
        {
            //waypoints.Count = transform.GetChild(i);
        }

    }
}
