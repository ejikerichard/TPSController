using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDetach : MonoBehaviour {

    public ArrowControl arrowControl;
    public Transform detachObject;
    public bool alignNormal = true;

    [HideInInspector]
    public float penetration;


    public void OnDestroyProjectile(Collision hit){
        detachObject.parent = hit.transform;
        if (alignNormal){
            Vector3 targetDir = detachObject.position - hit.transform.position;
            penetration = Vector3.Angle(detachObject.forward, targetDir);
        }
    }
}
