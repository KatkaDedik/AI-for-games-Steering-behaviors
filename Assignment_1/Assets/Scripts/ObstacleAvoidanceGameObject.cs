using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sphere
{
    public Vector3 WorldCenter { get; set; }

    public float Radius { get; set; }
}

public enum LastObsticleSide
{
    None,
    Left,
    Right
}

public class ObstacleAvoidanceGameObject : AbstractSteeringGameObject
{
    [SerializeField]
    protected LayerMask obstacleLayer;

    [SerializeField]
    protected float destinationTolerance = 1.5f;

    [SerializeField]
    protected Transform destinationsParent;

    protected Vector3 currentDestination;

    protected Sphere[] obstacles;

    protected Vector3[] destinationLocations;

    protected Vector3 DesiredDirection => (currentDestination - transform.position).normalized;

    public GameObject ball;

    [SerializeField] private float LookAhead = 1f;
    [SerializeField] private float avoidDistance = 1f;
    [SerializeField] private float directionMemory = 0.5f;
    Vector3 Direction = Vector3.zero;
    [SerializeField]private LastObsticleSide lastSide = LastObsticleSide.None;
    private Vector3 steer = Vector3.zero;

    protected override void Start()
    {
        base.Start();

        currentDestination = transform.position;

        // It is not best practice to use LINQ in Unity but it is not a big deal in this case.
        // For the curious ones, the reason to rather avoid LINQ in Unity is bad performance & garbage collection.
        obstacles = FindObjectsOfType<SphereCollider>()
            .Where(x => obstacleLayer == (obstacleLayer | (1 << x.gameObject.layer)))
            .Select(x => new Sphere
            {
                WorldCenter = x.transform.TransformPoint(x.center),
                Radius = x.bounds.extents.x
            })
            .ToArray();

        destinationLocations = destinationsParent.GetComponentsInChildren<Transform>()
            .Where(x => x != destinationsParent)
            .Select(x => x.position)
            .ToArray();
    }

    protected override void Update()
    {
        base.Update();
        CheckDestinationUpdate();

        // TODO Task 2 Obstacle Avoidance 
        //      Information about sphere obstacles is stored in "obstacles" array.
        //      The variable "desiredDirection" holds information about the direction in which the agent wants to move.
        //      Set the final velocity to "Velocity" property. The maximum speed of the agent is determined by "maxSpeed".
        //      In case you would prefer to modify the transform.position directly, you can change the movementControl to Manual (see AbstractSteeringGameObject class for info).
        //      Feel free to extend the codebase. However, make sure it is easy to find your solution.
        
        Vector3 forwardRightDirection = transform.rotation * (Vector3.forward + Vector3.right / 4).normalized;
        Vector3 forwardLeftDirection = transform.rotation * (Vector3.forward + Vector3.left / 4).normalized;

        Vector3 rightDirection = transform.rotation * (Vector3.forward + Vector3.right * 2).normalized;
        Vector3 leftDirection = transform.rotation * (Vector3.forward + Vector3.left * 2).normalized;
        RaycastHit hit;

        if (lastSide == LastObsticleSide.Left)
        {
            if (Physics.Raycast(transform.position, forwardLeftDirection, out hit, LookAhead * 2, obstacleLayer))
            {
                steer += rightDirection * (LookAhead * 2 - hit.distance) * avoidDistance;
            }
            else if (Physics.Raycast(transform.position, leftDirection, out hit, LookAhead, obstacleLayer))
            {
                steer += forwardRightDirection * (LookAhead - hit.distance) * avoidDistance;
            }
            else
            {
                lastSide = LastObsticleSide.None;
            }
        }
        else if (lastSide == LastObsticleSide.Right)
        {
            if (Physics.Raycast(transform.position, forwardRightDirection, out hit, LookAhead * 2, obstacleLayer))
            {
                steer += leftDirection * (LookAhead * 2 - hit.distance) * avoidDistance;
            }
            else if (Physics.Raycast(transform.position, rightDirection, out hit, LookAhead, obstacleLayer))
            {
                steer += forwardLeftDirection * (LookAhead - hit.distance) * avoidDistance;
            }
            else
            {
                steer += DesiredDirection;
                lastSide = LastObsticleSide.None;
            }
        }
        else if (lastSide == LastObsticleSide.None)
        {
            if (Physics.Raycast(transform.position, forwardRightDirection, out hit, LookAhead * 2, obstacleLayer))
            {
                steer += leftDirection * (LookAhead * 2 - hit.distance) * avoidDistance;
                lastSide = LastObsticleSide.Right;
            }
            else if (Physics.Raycast(transform.position, forwardLeftDirection, out hit, LookAhead * 2, obstacleLayer))
            {
                steer += rightDirection * (LookAhead * 2 - hit.distance) * avoidDistance;
                lastSide = LastObsticleSide.Left;
            }
            else if (Physics.Raycast(transform.position, rightDirection, out hit, LookAhead, obstacleLayer))
            {
                steer += forwardLeftDirection * (LookAhead - hit.distance) * avoidDistance;
                lastSide = LastObsticleSide.Right;
            }
            else if (Physics.Raycast(transform.position, leftDirection, out hit, LookAhead, obstacleLayer))
            {
                steer += forwardRightDirection * (LookAhead - hit.distance) * avoidDistance;
                lastSide = LastObsticleSide.Left;
            }
            else
            {
                steer += DesiredDirection;
            }
        }

        Debug.DrawLine(transform.position, transform.position + rightDirection.normalized * LookAhead, Color.red);
        Debug.DrawLine(transform.position, transform.position + leftDirection.normalized * LookAhead, Color.blue);
        Debug.DrawLine(transform.position, transform.position + forwardRightDirection.normalized * LookAhead * 2, Color.red + Color.white);
        Debug.DrawLine(transform.position, transform.position + forwardLeftDirection.normalized * LookAhead * 2, Color.cyan);

        steer.Normalize();
        Direction += (steer + DesiredDirection * 0.1f) / directionMemory;
        Direction.Normalize();
        transform.position += Direction * Time.deltaTime * maxSpeed;
        LookDirection.Normalize();
        LookDirection = LookDirection * 0.3f + Direction * 0.7f;
        
    }



    protected override void LateUpdate()
    {
        base.LateUpdate();

        Debug.DrawLine(transform.position + debugLinesOffset,
            transform.position + debugLinesOffset + DesiredDirection.normalized, Color.black);
    }

    protected void CheckDestinationUpdate()
    {
        if (Vector3.SqrMagnitude(currentDestination - transform.position) <= destinationTolerance * destinationTolerance)
        {
            currentDestination = destinationLocations[Random.Range(0, destinationLocations.Length)];
            steer += DesiredDirection * 2f;
            ball.transform.position = currentDestination;
        }
    }

    public override void SetDebugObjectsState(bool newState)
    {
        base.SetDebugObjectsState(newState);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(currentDestination, currentDestination +
            new Vector3(0.0f, 2.0f, 0.0f));
    }
}
