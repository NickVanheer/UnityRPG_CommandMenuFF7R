using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RPGMenuSection : MonoBehaviour
{
    [Header("Core section info")]
    public string Name;
    GameObject rootObject;

    public void Start()
    {
       if(rootObject == null)
         rootObject= new GameObject("Section Object");

        this.transform.parent = rootObject.transform;
    }
}
