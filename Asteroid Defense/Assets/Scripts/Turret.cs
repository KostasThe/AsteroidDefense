using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public static Turret Instance;
    
    private bool is_Dangerous;

    private const float missileSpeed = 10;

    public GameObject missile;
    public Vector3 asteroidPos;
    public Vector3 asteroidVel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Multiple instances of Turret!");
            Destroy(this);
            return;
        }
        if (Instance == null)
            Instance = this;
    }

    public void ShootAsteroid(GameObject targetedAsteroid, Vector3 asteroidPosition, Vector3 asteroidVelocity)
    {
        //initTime = Time.time; // NN?
        is_Dangerous = TrajectoryWithinSafetyZone(asteroidPosition, asteroidVelocity);
        if (is_Dangerous)
        {
            Vector3 missileVelocity = CalculateMissileVelocity(asteroidPosition, asteroidVelocity);
            FireRocket(targetedAsteroid, missileVelocity);
        }
    }

    private void FireRocket(GameObject targetedAsteroid, Vector3 rocket_Velocity)
    {
        GameObject missileClone = ObjectPooler.Instance.GetPooledObject("Rocket");
        if (missileClone != null)
        {
            missileClone.GetComponent<Rocket>().m_Target = targetedAsteroid;
            missileClone.transform.position = this.transform.position;
            missileClone.SetActive(true);
            missileClone.GetComponent<Rigidbody>().velocity = rocket_Velocity;
        }
    }

    private bool TrajectoryWithinSafetyZone(Vector3 asteroidPosition, Vector3 asteroidVelocity)
    {
        int layerMask = 1 << 8;
        RaycastHit hit;
        float raySize = CalculateRaySize(asteroidPosition);

        Debug.DrawRay(asteroidPosition, asteroidVelocity.normalized * raySize, Color.yellow, 10.0f); // Debug.ray must remove 
        if (Physics.Raycast(asteroidPosition, asteroidVelocity.normalized * raySize, out hit, Mathf.Infinity, layerMask))
            return true;
        return false;
    }
    //Calculate the length of the ray, by finding the distance between the Earth and the asteroid in order to ensure the ray will be enough to properly check if it hits the safe zone
    private float CalculateRaySize(Vector3 asteroidSpawnPos)
    {
        Vector3 direction = this.transform.position - asteroidSpawnPos;
        return direction.magnitude;
    }

    public Vector3 CalculateMissileVelocity(Vector3 asteroidPosition, Vector3 asteroidVelocity)
    {
        float missileSpeedSq = missileSpeed * missileSpeed;
        float asteroidSpeedSq = asteroidVelocity.sqrMagnitude;
        float asteroidSpeed = Mathf.Sqrt(asteroidSpeedSq);
        Vector3 asteroidToTurret = this.transform.position - asteroidPosition;
        float asteroidToTurretDistSq = asteroidToTurret.sqrMagnitude;
        float asteroidToTurretDist = Mathf.Sqrt(asteroidToTurretDistSq);
        Vector3 asteroidToTurretDir = asteroidToTurret;
        asteroidToTurretDir.Normalize();

        Vector3 asteroidVelocityDir = asteroidVelocity;
        asteroidVelocityDir.Normalize();

        float cosTheta = Vector3.Dot(asteroidToTurretDir, asteroidVelocityDir);

        //bool validSolution = true;  if we want to do something in case there's no valid solution; currently in our simulation not needed but just left it for the sake of logical flow
        float t = 0;
        if (Mathf.Approximately(missileSpeedSq, asteroidSpeedSq))
        {
            if (cosTheta > 0)
                t = 0.5f * asteroidToTurretDist / (asteroidSpeed * cosTheta);
            else
            {
                //validSolution = false;
                Debug.Log("Did not find a solution; if cos(theta) is <= 0 that would mean B goes backward or leads to a division by zero (infinity)");
            }
        }
        else
        {
            float a = missileSpeedSq - asteroidSpeedSq;
            float b = 2.0f * asteroidToTurretDist * asteroidSpeed * cosTheta;
            float c = -asteroidToTurretDistSq;
            float discriminant = b * b - 4.0f * a * c;

            if (discriminant < 0)
            {
                //validSolution = false;
                Debug.Log("Did not find solution; asteroid speed is faster or equal than missile's");
            }
            else
            {
                float uglyNumber = Mathf.Sqrt(discriminant);
                float t0 = 0.5f * (-b + uglyNumber) / a;
                float t1 = 0.5f * (-b - uglyNumber) / a;

                //Assign the lowest positive time to t to aim at the earliest hit
                t = Mathf.Min(t0, t1);
                if (t < Mathf.Epsilon)
                    t = Mathf.Max(t0, t1);
                if (t < Mathf.Epsilon)
                {
                    //validSolution = false;
                    Debug.Log("Time can't go backwards; solution not valid");
                }
            }
        }
        Vector3 missileVelocity = asteroidVelocity + (-asteroidToTurret / t);
        return missileVelocity;
    }
}