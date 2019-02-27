using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CritterStates { Wandering, Following, Jumping};
public class MyCritter : MonoBehaviour
{
    private FSM<MyCritter> _fsm;
    private TaskManager _tm;
    #region wander variables
    public float wanderWidth;
    public float wanderHeight;
    public float wanderSpeed;
    #endregion
    #region particle system variables
    public ParticleSystem particles;
    private ParticleSystem.MainModule psMain;
    public Gradient psHappy;
    public Gradient psSad;
    public Gradient psScared;
    public Gradient psTired;
    #endregion
    public float movementSpeed;
    public float jumpSpeed;
    public int jumpLevel;
    public int jumpCount;
    public float distanceToFollowThreshold;
    public float visibilityRange;
    public GameObject armor;
    public GameObject nearestEnemy;
    private GameObject[] enemies;
    public float followTime;
    public bool tired;
    // Start is called before the first frame update
    void Start()
    {
        CritterInit();
        _tm = new TaskManager();
        //initializing my new FSM
        _fsm = new FSM<MyCritter>(this);
        //setting it's initial state
        _fsm.TransitionTo<Wandering>();
    }

    // Update is called once per frame
    void Update()
    {
        _tm.Update();
        _fsm.Update();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.gameObject;
        if (other.gameObject.tag == "Wall") wanderSpeed *= -1;
        var hitObj = other.GetComponent<Hittable>();
        if (hitObj != null)
        {
            if (hitObj.deadly) ((CritterState)_fsm.CurrentState).OnDeath();
            else if (armor.activeSelf) hitObj.SendMessage("HitByArmor");
        }
    }

    private void CritterInit()
    {
        psMain = particles.main;
        psMain.startColor = psSad;
        Services.Touch.MaxFingers = jumpLevel;
    }

    private void WanderAround()
    {
        if (tired) psMain.startColor = psTired;
        else psMain.startColor = psSad;
        float xPos = transform.position.x;
        float yPos = transform.position.y;
        Vector3 newPos = new Vector3(Random.Range(xPos - wanderWidth, xPos + wanderWidth), Random.Range(yPos - wanderHeight, yPos + wanderHeight), transform.position.z);
        var directionToDestination = (newPos - transform.position).normalized;
        var impulseToDestination = directionToDestination * wanderSpeed;
        GetComponent<Rigidbody2D>().AddForce(impulseToDestination, ForceMode2D.Impulse);
    }

    private void StayOnFinger()
    {
        Vector3 currentPos = transform.position;
        float step = Mathf.Abs(movementSpeed) * Time.deltaTime;
        psMain = particles.main;
        psMain.startColor = psHappy;
        transform.position = Vector3.MoveTowards(currentPos, Services.Touch.primaryTouchPos, step);
    }

    private void JumpToFinger()
    {
        Vector3 currentPos = transform.position;
        psMain.startColor = psHappy;
        float step = Mathf.Abs(jumpSpeed) * Time.deltaTime;
        transform.position = Vector3.MoveTowards(currentPos, Services.Touch.primaryTouchPos, step);
    }

    private void MoveAwayFromEnemy()
    {
        psMain.startColor = psScared;
        transform.position = Vector3.MoveTowards(transform.position, nearestEnemy.transform.position, -wanderSpeed * Time.deltaTime);
    }

    private void FindNearbyEnemy()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            if (Vector3.Distance(transform.position, enemies[i].transform.position) < visibilityRange)
            {
                if (nearestEnemy == null) nearestEnemy = enemies[i];
                else if (Vector3.Distance(transform.position, enemies[i].transform.position) <
                         Vector3.Distance(transform.position, nearestEnemy.transform.position)) nearestEnemy = enemies[i];
                Debug.Log("ENEMY NEARBY");
            }
            else
            {
                Debug.Log("Clear of enemy");
                nearestEnemy = null;
            }
        }
    }

    private bool OnFinger()
    {
        return Vector3.Distance(transform.position, Services.Touch.primaryTouchPos) <= distanceToFollowThreshold;
    }

    ///////////////////////////////////////////////////////////////////////
    // STATES
    ///////////////////////////////////////////////////////////////////////
    private class CritterState : FSM<MyCritter>.State
    {
        public void OnDeath()
        {
            Parent.TransitionTo<Dead>();
        }
    }

    private class Wandering : CritterState
    {
        float tiredEnterTime;
        public override void OnEnter()
        {
            Services.Touch.MaxFingers = Context.jumpLevel;
            if (Context.tired)
            {
                tiredEnterTime = Time.time;
            }
        }

        public override void Update()
        {
            Context.FindNearbyEnemy();
            if (Context.tired && ((Time.time - tiredEnterTime) > Context.followTime)) Context.tired = false;
            if (Services.Touch.touchCount > 0 && !Context.tired)
            {
                Debug.Log("Transitioning to Jump");
                TransitionTo<Jumping>();
                return;
            }
            else if(Context.nearestEnemy != null)
            {
                TransitionTo<Fleeing>();
            }
            else Context.WanderAround();
        }
    }

    private class Jumping : CritterState
    {
        public override void OnEnter()
        {
            Context.armor.SetActive(true);
            Context.jumpCount++;
            if (Context.jumpCount > Context.jumpLevel)
            {
                Context.jumpCount = 0;
                Context.tired = true;
            }
        }
        public override void Update()
        {
            Debug.Log("Jumping");
            if (Context.OnFinger() && !Context.tired)
            {
                TransitionTo<Following>();
                return;
            }
            else if (Services.Touch.touchCount == 0 || Context.tired)
            {
                TransitionTo<Wandering>();
                return;
            }
            else Context.JumpToFinger();
        }
        public override void OnExit()
        {
            Context.armor.SetActive(false);
        }
    }

    private class Following : CritterState
    {
        private float enterTime;

        public override void OnEnter()
        {
            enterTime = Time.time;
        }
        public override void Update()
        {
            Debug.Log("Following");
            if ((Time.time - enterTime) > Context.followTime)
            {
                Context.tired = true;
                TransitionTo<Wandering>();
            }
            else if (Services.Touch.touchCount == 0) TransitionTo<Wandering>();
            else if ((Services.Touch.currentFinger.phase == TouchPhase.Began || Services.Touch.currentFinger.phase == TouchPhase.Ended) && Services.Touch.touchCount != 0)
            {
                TransitionTo<Jumping>();
                return;
            }
            else Context.StayOnFinger();
        }
    }

    private class Fleeing : CritterState
    {
        public override void Update()
        {
            Context.FindNearbyEnemy();
            if (Context.nearestEnemy == null) TransitionTo<Wandering>();
            else if (Services.Touch.touchCount > 0) TransitionTo<Jumping>();
            else Context.MoveAwayFromEnemy();
        }
    }

    private class Dead : CritterState
    {
        public override void Update()
        {
            Destroy(Context.gameObject);
        }
    }
}
