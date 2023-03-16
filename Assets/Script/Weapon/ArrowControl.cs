using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowControl : MonoBehaviour{

    public ArrowLifeSettings arrowLifeSettings;

    Rigidbody myBody;
    protected Vector3 previousPosition;
    public LayerMask hitLayers;
    public ProjectileCastColliderEvent onCastCollider;
    public ProjectileCastColliderEvent onDestroyProjectile;
    private bool hitSomething = false;
    public float angleBetween;


    void Start(){
        myBody = GetComponent<Rigidbody>();
        previousPosition = transform.position;
    }

   void Update(){
        if(!hitSomething){
            transform.rotation = Quaternion.LookRotation(myBody.velocity);
        }
    }

    void FixedUpdate(){
        //RaycastHit hitInfo;
        //if(Physics.Linecast(previousPosition, transform.position + transform.forward * 0.5f, out hitInfo, hitLayers)){
        //    if (!hitInfo.collider)
        //        return;

        //    if(hitInfo.collider){
        //        Vector3 targetDir = hitInfo.transform.position - transform.position;
        //        angleBetween = Vector3.Angle(transform.forward, targetDir);
        //        myBody.isKinematic = true;
        //        hitSomething = true;

        //        //onCastCollider.Invoke(hitInfo);
        //        //onDestroyProjectile.Invoke(hitInfo);
        //        //Destroy(gameObject);
        //    }
        //}
    }

    [System.Serializable]
    public class ProjectileCastColliderEvent : UnityEngine.Events.UnityEvent<Collision> { }

    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.tag == "Untagged")
        {
            //transform.parent = collision.transform;
            myBody.isKinematic = true;
            myBody.velocity = Vector3.zero;
            hitSomething = true;

            Vector3 targetDir = collision.transform.position - transform.position;
            angleBetween = Vector3.Angle(transform.forward, targetDir);

            onCastCollider.Invoke(collision);
            onDestroyProjectile.Invoke(collision);
            Destroy(gameObject);

            //transform.rotation = Quaternion.LookRotation(collision.transform.position);
        }
    }
}
