using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrivalGameObject : AbstractSteeringGameObject
{
    [SerializeField]
    protected GameObject objectToFollow;
    public Vector3 direction = Vector3.zero;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        // TODO Task 1 Arrival 
        //      Add your code here (the object to follow is determined by the value of "objectToFollow" variable).
        //      Set the final velocity to "Velocity" property. The maximum speed of the agent is determined by "maxSpeed".
        //      If you want to change the rotation of the agent, you can use, for example, "LookDirection" property.
        //      In case you would prefer to modify the transform.position directly, you can change the movementControl to Manual (see AbstractSteeringGameObject class for info).
        //      Feel free to extend the codebase. However, make sure it is easy to find your solution.

        Vector3 addDirection = objectToFollow.transform.position - transform.position;
        direction = direction * 0.9f + addDirection;
        float velocity = Mathf.Clamp(addDirection.magnitude, 0, maxSpeed);
        direction = direction.normalized * velocity;
        transform.position += direction * Time.deltaTime;
        transform.LookAt(objectToFollow.transform);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }
}
