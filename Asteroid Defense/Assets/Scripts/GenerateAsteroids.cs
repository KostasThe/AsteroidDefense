using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAsteroids : MonoBehaviour {
    public static GenerateAsteroids Instance;

    //public GameObject asteroid;
    public Rigidbody m_Rigidbody;

    public Vector3 spawnPos;
    public Vector3 mouseCurrLoc;

    private bool locked = false;
    private float startTime;
    private float endTime;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Multiple instances of GenerateAsteroid!");
            Destroy(this);
            return;
        }
        if (Instance == null)
            Instance = this;
    }

    //flag locked is used in order not to spawn multiple objects and holding down the mouse button
    void Update () {
        if(Input.GetMouseButton(0) && !locked)
        {
            locked = true;
            spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            spawnPos.z = 0.0f; 
            startTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0))
        {
            float endTime = Time.time;
            float duration = endTime - startTime;

            mouseCurrLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mouseCurrLoc - spawnPos;

            float distance = direction.magnitude;
            float power = distance / duration;

            Vector3 m_Velocity = direction.normalized * power;
            m_Velocity = new Vector3(m_Velocity.x, m_Velocity.y, 0.0f);
            m_Velocity = Vector3.ClampMagnitude(m_Velocity, 10);
            GameObject targetedAsteroid = CreateAsteroid();
            
            m_Rigidbody.velocity = m_Velocity; // simpl. w/o m_Velocity
            Turret.Instance.ShootAsteroid(targetedAsteroid, spawnPos, m_Rigidbody.velocity);
            locked = false;
        }
	}

    private GameObject CreateAsteroid()
    {
        GameObject asteroidClone = ObjectPooler.Instance.GetPooledObject("Asteroid");
        asteroidClone.transform.position = spawnPos;
        asteroidClone.SetActive(true);
        m_Rigidbody = asteroidClone.GetComponent<Rigidbody>();
        return asteroidClone;
    }
}
