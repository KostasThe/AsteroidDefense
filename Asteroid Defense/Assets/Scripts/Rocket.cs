using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
    public GameObject m_Target;
    
    void OnTriggerEnter(Collider objectHit)
    {
        if (objectHit.gameObject.tag == "Asteroid" && objectHit.gameObject == m_Target) //For the sake of simplicity our rocket will hit only the asteroid it was initially aiming to
        {
            this.gameObject.SetActive(false);
            objectHit.gameObject.SetActive(false);
        }
    }
}
