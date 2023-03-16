using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour{

    public float delay;

    IEnumerator Start(){

        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
