using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public float speed = 10.0f;
    public Transform PickupLocation;
    
    private Rigidbody rb;
    private GameObject mirrorToPick = null;
    private bool pickedUp = false;

    void Start()
    {
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 lookDirection = cam.transform.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        var forward = transform.forward;
        var right = transform.right;


        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        Debug.DrawRay(cam.transform.position, forward, Color.green);
        right.Normalize();
        Debug.DrawRay(cam.transform.position, right, Color.red);

        var desiredMoveDirection = forward * z + right * x;
        rb.velocity = -desiredMoveDirection.normalized * speed;

        if(Input.GetMouseButtonDown(0) && mirrorToPick && !pickedUp) 
        {
            mirrorToPick.gameObject.transform.SetParent(transform);
            mirrorToPick.gameObject.transform.localPosition = PickupLocation.localPosition;
            mirrorToPick.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0,-90,0));
            pickedUp = true;
        } else if(Input.GetMouseButtonDown(0) && pickedUp) {
            mirrorToPick.transform.parent = null;
            pickedUp = false;
            mirrorToPick = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((other.tag == "Mirror" || other.tag=="Splitter") && !pickedUp)
        {
            mirrorToPick = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.tag == "Mirror" || other.tag == "Splitter") && !pickedUp)
        {
            mirrorToPick = null;
        }
    }

}
