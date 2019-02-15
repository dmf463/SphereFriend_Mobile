using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;
    private Vector3 playerPos;
    public GameObject rightWall;
    public GameObject leftWall;
    public GameObject topWall;
    public GameObject bottomWall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfCameraShouldMove();
    }

    void CheckIfCameraShouldMove()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(touch.position);

            if (targetPos.x < leftWall.transform.position.x) MoveCamera();
            else if (targetPos.x > rightWall.transform.position.x) MoveCamera();
            else if (targetPos.y > topWall.transform.position.y) MoveCamera();
            else if (targetPos.y < bottomWall.transform.position.y) MoveCamera();
        }
    }

    void MoveCamera()
    {
        playerPos = player.transform.position;
        float speed = player.GetComponent<PlayerMovement>().speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerPos.x, playerPos.y, -10), speed);
    }
}
