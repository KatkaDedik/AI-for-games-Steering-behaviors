using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    [SerializeField]
    private KeyCode keyToSwapCameras = KeyCode.C;

    [SerializeField]
    private Camera defaultOrtographicCamera;

    [SerializeField]
    private Camera perspectiveCamera;

    private void Update()
    {
        if(Input.GetKeyUp(keyToSwapCameras))
        {
            defaultOrtographicCamera.gameObject.SetActive(!defaultOrtographicCamera.gameObject.activeSelf);
            perspectiveCamera.gameObject.SetActive(!defaultOrtographicCamera.gameObject.activeSelf);
        }
    }
}
