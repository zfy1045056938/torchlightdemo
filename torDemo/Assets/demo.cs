using UnityEngine;
using System.Collections;
using System;

public class demo :  MonoBehaviour
{

    public int id=0;
    void Awake(){
        
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("test" + id);
    }
}
