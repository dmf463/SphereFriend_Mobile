using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public Vector3 primaryTouchPos;
    public Touch currentFinger;
    public bool currentFingerTouching;
    public Vector3 previousFinger; 
    private int _maxFingers;
    public int MaxFingers
    {
        get
        {
            Debug.Assert(_maxFingers > 0);
            if (_maxFingers >= 5) _maxFingers = 5;
            return _maxFingers;
        }
        set
        {
            _maxFingers = value;
        }
    }
    public int touchCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        touchCount = Input.touchCount;
        AssignPrimaryTouch();
    }

    private void AssignPrimaryTouch()
    {
        if (Input.touchCount > 0)
        {
            int touchInt = touchCount - 1;
            if (touchCount > _maxFingers) touchCount = _maxFingers;
            primaryTouchPos = GetTouchPos(touchInt);
            currentFinger = GetTouch(touchInt);
        }
    }
    
    private Vector3 GetTouchPos(int touchIndex)
    {
        Vector3 pos;
        Touch touch = Input.GetTouch(touchIndex);
        pos = Camera.main.ScreenToWorldPoint(touch.position);
        pos.z = 0;
        return pos;
    }

    private Touch GetTouch(int touchIndex)
    {
        return Input.GetTouch(touchIndex);
    }
}
