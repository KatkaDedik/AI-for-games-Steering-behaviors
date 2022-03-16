using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField]
    protected float yPosToForce = 0.0f;

    [SerializeField]
    protected bool moveOnlyWithMouseClick = false;

    protected virtual void Update()
    {
        if (!moveOnlyWithMouseClick || Input.GetMouseButtonUp(0))
        {
            // Camera.main can't be cached in Awake/Start since the cameras are swapped during runtime
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Ground")))
            {
                transform.position = new Vector3(hit.point.x, yPosToForce, hit.point.z);
            }
        }
    }
}
