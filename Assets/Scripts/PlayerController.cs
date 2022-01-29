using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public float zSpeed = 10.0f;
    public float xSpeed = 10.0f;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDirection = cam.transform.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(xSpeed * x, 0, zSpeed * z);
        //transform.Translate(movement * Time.deltaTime);
        rb.velocity = Vector3.Lerp(rb.velocity, movement, Time.deltaTime);
    }
}
