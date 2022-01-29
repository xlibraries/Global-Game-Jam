using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    public float lagSpeed = 5.0f;

    //Camera rotation variables
    public float speedY = 2.0f;
    private float yaw = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, Time.deltaTime * lagSpeed);
        yaw += speedY * Input.GetAxis("Mouse X");
        transform.eulerAngles = new Vector3(0, yaw, 0);
    }
}