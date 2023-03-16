using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectContainer : MonoBehaviour{

    static ObjectContainer instance;
    public static Transform root{
        get{
            if(!instance){

                instance = new GameObject("Object Container", typeof(ObjectContainer)).GetComponent<ObjectContainer>();
            }
            return instance.transform;
        }
    }
}
