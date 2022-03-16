using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInfiniteArea : MonoBehaviour
{
    private AbstractSteeringGameObject[] steeringGameObjects;

    private Bounds bounds;

    private void Start()
    {
        bounds = GetComponent<MeshRenderer>().bounds;
        ForceObjectsUpdate();
    }

    // Similar functionality can be achieved with colliders & OnTriggerExit but 
    // this approach is omitted to ensure that the objects you will be using
    // are as simple as possible in case you need to modify them somehow.
    private void Update()
    {
        for(int i = 0; i < steeringGameObjects.Length; ++i)
        {
            Vector3 newPosition = steeringGameObjects[i].transform.position;
            bool outOfBounds = false;

            if (newPosition.x > bounds.max.x)
            {
                newPosition.x = bounds.min.x;
                outOfBounds = true;
            }
            else if(newPosition.x < bounds.min.x)
            {
                newPosition.x = bounds.max.x;
                outOfBounds = true;
            }

            if(newPosition.z > bounds.max.z)
            {
                newPosition.z = bounds.min.z;
                outOfBounds = true;
            }
            else if(newPosition.z < bounds.min.z)
            {
                newPosition.z = bounds.max.z;
                outOfBounds = true;
            }

            steeringGameObjects[i].transform.position = newPosition;

            if(outOfBounds) 
            {
                steeringGameObjects[i].ClearTrail();
            }
        }
    }

    public void ForceObjectsUpdate()
    {
        steeringGameObjects = FindObjectsOfType<AbstractSteeringGameObject>();
    }
}
