using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAsteroids : MonoBehaviour {
    public static GenerateAsteroids Instance;
    public Rigidbody asteroidRigidbody;
    public Vector3 spawnPos;
    public Vector3 mouseCurrLoc;

    private bool locked = false; // flag locked is used in order not to spawn multiple objects and holding down the mouse button
    private bool valid = true;
    private float startTime;
    private Vector3 velocity;
    
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

    void Update () {
        if(Input.GetMouseButton(0) && !locked)
        {
            locked = true;
            valid = CanSpawnAsteroids();
            if (valid)
            {
                spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                spawnPos.z = 0.0f;
                startTime = Time.time;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (valid)
            {
                PrepareAsteroid();
                GameObject targetedAsteroid = CreateAsteroid();
                SetAsteroidMovement(targetedAsteroid);
                Turret.Instance.ShootAsteroid(targetedAsteroid, spawnPos, asteroidRigidbody.velocity);
            }
            locked = false;
        }
	}

    private void PrepareAsteroid()
    {
        float endTime = Time.time;
        float duration = endTime - startTime; // the duration in seconds between the mouse button clicked and mouse button released(drag)

        mouseCurrLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mouseCurrLoc - spawnPos;

        float distance = direction.magnitude; // the distance of the drag
        float power = distance / duration;    // calculate the power by taking into account the distance and duration of the drag

        velocity = direction.normalized * power;
        velocity = new Vector3(velocity.x, velocity.y, 0.0f);
        velocity = Vector3.ClampMagnitude(velocity, 10);
    }

    private GameObject CreateAsteroid()
    {
        GameObject asteroidClone = ObjectPooler.Instance.GetPooledObject("Asteroid");
        asteroidClone.transform.position = spawnPos;
        asteroidClone.SetActive(true);
        asteroidRigidbody = asteroidClone.GetComponent<Rigidbody>();
        return asteroidClone;
    }

    private void SetAsteroidMovement(GameObject targetedAsteroid)
    {
        asteroidRigidbody.velocity = velocity;
        if (velocity == Vector3.zero)
            targetedAsteroid.SetActive(false);
    }

    //is false when we try to create an asteroid upon another object (e.g. safezone, another asteroid..) 
    private bool CanSpawnAsteroids()
    {
        Vector3 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, Vector3.forward, out hit))
            return false;
        return true;
    }
}
