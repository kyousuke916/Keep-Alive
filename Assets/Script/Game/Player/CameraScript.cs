using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour 
{
    public Transform lookAtObj;

    void Update () 
    {
        transform.LookAt(lookAtObj);    
    }
}