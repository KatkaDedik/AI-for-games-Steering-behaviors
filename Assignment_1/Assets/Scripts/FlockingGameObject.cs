using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingGameObject : AbstractSteeringGameObject
{
    [SerializeField]
    protected float neighbourRadius = 8.0f;

    public FlockSpawner Spawner { get; set; }

    List<FlockingGameObject> targets;
    [SerializeField] private float threshold = 2f;
    [SerializeField] private float decayCoefficient = -25f;
    [SerializeField] private float maxAcceleration = 5f;
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] private float alignDistance = 8f;
    public float flockingStrength = 0.5f;
    public Vector3 Flocking = Vector3.zero;

    protected override void Start()
    {
        base.Start();
        targets = new List<FlockingGameObject>();
        foreach (var target in Spawner.Agents)
        {
            if (target != this)
            {
                targets.Add(target);
            }
        }
        Velocity = new Vector3(Random.insideUnitSphere.x, 0.0f, Random.insideUnitSphere.y).normalized * maxSpeed;
    }

    protected override void Update()
    {
        base.Update();
        // TODO Task 3 Flocking
        //      Use methods below to compute individual steering behaviors.
        //      Information about other agents is stored in "Spawner.Agents".
        //      The variable "neighbourRadius" holds the radius which should be used for neighbour detection.
        //      Set the final velocity to "Velocity" property. The maximum speed of the agent is determined by "maxSpeed".
        //      In case you would prefer to modify the transform.position directly, you can change the movementControl to Manual (see AbstractSteeringGameObject class for info).
        //      Feel free to extend the codebase. However, make sure it is easy to find your solution.

        Flocking = (Flocking + (ComputeSeparation() + ComputeCohesion() + ComputeAlignment()).normalized * flockingStrength).normalized * flockingStrength;

        Velocity = Velocity * 0.5f + Flocking;
        float speed = Mathf.Clamp(Velocity.magnitude, 0, maxSpeed);
        var look = Velocity;
        Velocity = Velocity.normalized * speed;
        LookDirection += LookDirection * 0.98f + Velocity * 0.5f;
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected Vector3 ComputeSeparation()
    {
        Vector3 separation = Vector3.zero;

        foreach (var target in targets)
        {
            Vector3 direction = target.transform.position - transform.position;
            float distance = direction.magnitude;
            if (distance < threshold)
            {
                float strength = Mathf.Min(decayCoefficient / (distance * distance), maxAcceleration);
                direction.Normalize();
                separation += strength * direction;
            }
        }

        return separation * 2f;
    }

    protected Vector3 ComputeAlignment()
    {
        Vector3 alignment = Vector3.zero;
        int count = 0;
        foreach (var target in targets)
        {
            Vector3 targetDir = target.transform.position - transform.position;
            if (targetDir.magnitude < alignDistance)
            {
                alignment += target.Velocity;
                count++;
            }
        }

        if (count > 0)
        {
            alignment = alignment / count;
            if (alignment.magnitude > maxAcceleration)
            {
                alignment = alignment.normalized * maxAcceleration;
            }
        }
        return alignment * 1f;

    }

    protected Vector3 ComputeCohesion()
    {
        Vector3 cohesion = Vector3.zero;
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        foreach (var target in targets)
        {
            Vector3 targetDir = target.transform.position - transform.position;
            if (Vector3.Angle(targetDir, transform.forward) < viewAngle)
            {
                centerOfMass += target.transform.position;
                count++;
            }
        }
        if (count > 0)
        {
            centerOfMass = centerOfMass / count;
            cohesion = centerOfMass - transform.position;
            cohesion.Normalize();
            cohesion *= maxAcceleration;
        }

        return cohesion * 1.5f;
    }

    public override void SetDebugObjectsState(bool newState)
    {
        base.SetDebugObjectsState(newState);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, neighbourRadius);
    }
}
