using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInfoToggler : MonoBehaviour
{
    [SerializeField]
    private KeyCode keyToToggleDebugInfo = KeyCode.D;

    [SerializeField]
    private string tagToToggle = "Debug";

    private AbstractSteeringGameObject[] steeringObjectsToToggle;
    private GameObject[] generalObjecstToToggle;

    private bool isDebugShown = false;

    private void Start()
    {
        steeringObjectsToToggle = FindObjectsOfType<AbstractSteeringGameObject>();

        if (tagToToggle.Length > 0)
        {
            generalObjecstToToggle = GameObject.FindGameObjectsWithTag(tagToToggle);
        }

        UpdateState();
    }

    private void Update()
    {
        if(Input.GetKeyUp(keyToToggleDebugInfo))
        {
            isDebugShown = !isDebugShown;

            UpdateState();
        }
    }

    private void UpdateState()
    {
        for (int i = 0; i < steeringObjectsToToggle.Length; ++i)
        {
            steeringObjectsToToggle[i].SetDebugObjectsState(isDebugShown);
        }

        for (int i = 0; i < generalObjecstToToggle.Length; ++i)
        {
            generalObjecstToToggle[i].SetActive(isDebugShown);
        }
    }
}
