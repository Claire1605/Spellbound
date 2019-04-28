using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    public float maxWanderDuration = 10.0f;
    public float maxWanderDistance = 10.0f;
    public float nearThreshold = 0.5f;

    private float wanderDuration;
    private float wanderTime;
    private Vector3 wanderDestination;
    private Vector3 home;
    
	void Start ()
    {
        home = transform.position;
    }
	
	void Update ()
    {
        wanderTime += Time.deltaTime;

        if (wanderTime > wanderDuration || Vector3.Distance(transform.position, wanderDestination) < nearThreshold)
        {
            NewDestination();
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(wanderDestination - transform.position, Vector3.up), Time.deltaTime);
            transform.position = transform.position + transform.forward * Time.deltaTime;
        }
    }

    void NewDestination()
    {
        wanderDestination = home + Random.insideUnitSphere * maxWanderDistance;
        wanderDestination.y = home.y;
        wanderTime = 0.0f;
        wanderDuration = Random.Range(0.0f, maxWanderDuration);
    }
}
