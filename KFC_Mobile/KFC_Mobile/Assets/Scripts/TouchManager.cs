using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public Vector3 primaryTouch;
    public int touchCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AssignPrimaryTouch();
        touchCount = Input.touchCount;
    }

    private void AssignPrimaryTouch()
    {
        if (Input.touchCount == 1) primaryTouch = GetTouchPos(0);
    }

    private Vector3 GetTouchPos(int touchIndex)
    {
        Vector3 pos;
        Touch touch = Input.GetTouch(touchIndex);
        pos = Camera.main.ScreenToWorldPoint(touch.position);
        pos.z = 0;
        return pos;
    }
}
