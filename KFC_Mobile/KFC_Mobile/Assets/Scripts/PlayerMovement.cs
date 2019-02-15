using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private TaskManager tm;
    private Vector3 currentPos;
    public Vector3 primaryTouch = Vector3.zero;
    public Vector3 secondaryTouch = Vector3.zero;
    public bool dashing;
    public bool walking;
    public float dashSpeed;
    #region oldMovement
    //private Animator myAnim;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    tm = new TaskManager();
    //    myAnim = GetComponent<Animator>();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    tm.Update();
    //    if (Input.touchCount > 0 && !dashing)
    //    {
    //        AssignPrimaryTouch();
    //        //primaryTouch = GetTouchPos(0);
    //        if (Input.touchCount == 2)
    //        {
    //            //secondaryTouch = GetTouchPos(1);
    //            //DestoryIcons("DashIcon");
    //            //GameObject dashIcon = Instantiate(Services.PrefabDB.DashIcon, GetTouchPos(1), Quaternion.identity);
    //        }
    //        if (secondaryTouch != Vector3.zero && Input.touchCount == 1) Dash();
    //        else Walk();
    //    }
    //    else if (Input.touchCount == 0)
    //    {
    //        SetAnimationBool("Walking", false);
    //        walking = false;
    //        //DestoryIcons("WalkIcon");
    //        //DestoryIcons("DashIcon");
    //    }
    //}

    //private void AssignPrimaryTouch()
    //{
    //    if (Input.touchCount == 1) primaryTouch = GetTouchPos(0);
    //    else if (Input.touchCount == 2)
    //    {
    //        primaryTouch = GetTouchPos(0);
    //        secondaryTouch = GetTouchPos(1);
    //    }
    //}

    //private void Dash()
    //{
    //    QuickDash dash = new QuickDash(gameObject, currentPos, secondaryTouch, dashSpeed);
    //    //ActionTask destroyIcon = new ActionTask(() =>
    //    //{
    //    //    DestoryIcons("DashIcon");
    //    //});
    //    //dash.Then(destroyIcon);
    //    tm.Do(dash);
    //}

    ////private void DestoryIcons(string tag)
    ////{
    ////    GameObject[] icons = GameObject.FindGameObjectsWithTag(tag);
    ////    foreach (GameObject g in icons) Destroy(g);
    ////}


    //private void Walk()
    //{
    //    //DestoryIcons("WalkIcon");
    //    //GameObject touchIcon = Instantiate(Services.PrefabDB.TouchIcon, GetTouchPos(0), Quaternion.identity);
    //    walking = true;
    //    currentPos = transform.position;
    //    float step = Mathf.Abs(speed) * Time.deltaTime;
    //    transform.position = Vector3.MoveTowards(currentPos, primaryTouch, step);
    //    FlipX(primaryTouch);
    //    SetAnimationBool("Walking", true);
    //}

    //private Vector3 GetTouchPos(int touchIndex)
    //{
    //    Vector3 pos;
    //    Touch touch = Input.GetTouch(touchIndex);
    //    pos = Camera.main.ScreenToWorldPoint(touch.position);
    //    pos.z = 0;
    //    return pos;
    //}

    //public void FlipX(Vector3 pos)
    //{
    //    if (currentPos.x > pos.x) GetComponent<SpriteRenderer>().flipX = true;
    //    else GetComponent<SpriteRenderer>().flipX = false;
    //}

    //private void SetAnimationBool(string name, bool b)
    //{
    //    //myAnim.SetBool(name, b);
    //}
    #endregion
 
}
