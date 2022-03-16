using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementControl
{
    Velocity,   // The transform.position of the steering agent is automatically modified based on the value stored in Velocity
    Manual      // The transform.position of the agent is not modified and it is upon your code to perform this
}

[RequireComponent(typeof(MeshRenderer))]
public abstract class AbstractSteeringGameObject : MonoBehaviour
{
    protected readonly Vector3 debugLinesOffset = new Vector3(0.0f, 0.25f, 0.0f);

    [SerializeField]
    protected Color initColor = Color.white;

    [SerializeField]
    protected float maxSpeed = 3.0f;

    [SerializeField]
    protected MovementControl movementControl = MovementControl.Velocity;

    [SerializeField]
    protected TrailRenderer debugTrailRenderer;

    protected MeshRenderer meshRenderer;

    private Color _color = Color.clear;
    public virtual Color Color
    {
        get => _color;
        set
        {
            _color = value;
            UpdateMaterialsColors(value);
        }
    }

    public Vector3 Velocity = Vector3.zero;

    public virtual Vector3 LookDirection
    {
        get => transform.forward;
        set => transform.forward = value;
    }

    protected virtual void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (Color == Color.clear)
        {
            Color = initColor;
        }
    }

    protected virtual void Update() { }

    protected virtual void LateUpdate()
    {
        if (movementControl == MovementControl.Velocity)
        {
            transform.position += Velocity * Time.deltaTime;
        }

        Debug.DrawLine(transform.position + debugLinesOffset, 
            transform.position + debugLinesOffset + LookDirection.normalized, Color.cyan);
        Debug.DrawLine(transform.position + debugLinesOffset, 
            transform.position + debugLinesOffset + Velocity.normalized, Color.white);
    }
    
    public virtual void SetDebugObjectsState(bool newState)
    {
        if(!newState)
        {
            ClearTrail();
        }
        debugTrailRenderer.gameObject.SetActive(newState);
    }

    protected virtual void UpdateMaterialsColors(Color newColor)
    {
        if(meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        meshRenderer.material.SetColor("_BaseColor", newColor);
        debugTrailRenderer.startColor = newColor;
        debugTrailRenderer.endColor = newColor;
    }

    public void ClearTrail()
    {
        debugTrailRenderer.Clear();
    }
}
