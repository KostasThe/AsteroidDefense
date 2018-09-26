using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
    [HideInInspector] public GameObject myTarget;
    
    void OnTriggerEnter(Collider objectHit)
    {
        if (objectHit.gameObject.tag == "Asteroid" && objectHit.gameObject == myTarget) //For the sake of simplicity our rocket will hit only the asteroid it was initially aiming to
        {
            SoundManager.Instance.source.PlayOneShot(SoundManager.Instance.explosion);
            objectHit.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
            GameObject explosion = ObjectPooler.Instance.GetPooledObject("Explosion");
            explosion.transform.position = this.transform.position;
            explosion.gameObject.SetActive(true);
        }
    }

    //Rotate rocket based on its velocity
    public void RotateRocket(Vector3 velocity)
    {
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
