
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject Ethan;

    private Vector3 offset;

    void Start ()
    {
        offset = transform.position - Ethan.transform.position;
    }

    void LateUpdate ()
    {
        transform.position = Ethan.transform.position + offset;
    }

}