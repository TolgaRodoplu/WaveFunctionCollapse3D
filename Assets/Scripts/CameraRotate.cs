using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    Vector3 pointToLook;
    public float rotateSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        var gen = FindObjectOfType<Generator>();
        pointToLook = new Vector3(gen.dimX, gen.dimY, gen.dimZ);


        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(pointToLook, Vector3.up, rotateSpeed * Time.deltaTime);
        transform.LookAt(pointToLook);
    }
}
