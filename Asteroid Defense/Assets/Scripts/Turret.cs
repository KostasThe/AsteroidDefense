using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public static Turret Instance;
    private bool isDangerous; // is the asteroid going to hit our precious Earth?
    private const float missileSpeed = 10; // the constant speed of our rockets
    public GameObject missile;

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
        isDangerous = TrajectoryWithinSafetyZone(asteroidPosition, asteroidVelocity);
        if (isDangerous)
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
            missileClone.GetComponent<Rocket>().myTarget = targetedAsteroid;
            missileClone.transform.position = this.transform.position;
            missileClone.SetActive(true);
            missileClone.GetComponent<Rigidbody>().velocity = rocket_Velocity;
            missileClone.GetComponent<Rocket>().RotateRocket(rocket_Velocity);
            RotateTurret(rocket_Velocity);
        }
    }

    //Shoot a ray and check if it hits our safe zone
    private bool TrajectoryWithinSafetyZone(Vector3 asteroidPosition, Vector3 asteroidVelocity)
    {
        int layerMask = 1 << 8;
        RaycastHit hit;
        float raySize = CalculateRaySize(asteroidPosition);
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

    //All the match required to get the necessary Velocity Vector3.
    //Basically we want that: 1.length(final asteroid position - turret position) = distance that our rocket has travelled ==> (InitialAsteroidPosition + VelocityAsteroid * t - TurrentPosition) = SpeedOfRocket * t
    //                     2.final rocket position = final asteroid position ==> InitialPositionRocket + VelocityRocket * t = InitialPositionAsteroid + VelocityAsteroid * t
    //So we have two equations and two unknown values : time and VelocityRocket
    //By using Law of Cosines and Dot product we finally get a Quadratic formula which we solve for t (two values of t - choose the "right" one)
    //Finally we can now solve for VelocityRocket in Eq.2 since we now know t.
    public Vector3 CalculateMissileVelocity(Vector3 asteroidPosition, Vector3 asteroidVelocity)
    {
        //most of the following block aims to reduce precision errors
        float missileSpeedSq = missileSpeed * missileSpeed;
        float asteroidSpeedSq = asteroidVelocity.sqrMagnitude; // Insted of self multiplication in order to increase accuracy
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
                float unwantedNum = Mathf.Sqrt(discriminant);
                float t0 = 0.5f * (-b + unwantedNum) / a;
                float t1 = 0.5f * (-b - unwantedNum) / a;

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

    //Rotate the turret based on the velocity of the rocket
    public void RotateTurret(Vector3 rocketVelocity)
    {
        float angle = Mathf.Atan2(rocketVelocity.y, rocketVelocity.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
