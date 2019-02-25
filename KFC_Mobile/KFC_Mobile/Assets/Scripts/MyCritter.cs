using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CritterStates { Wandering, Following, Jumping};
public class MyCritter : MonoBehaviour
{
    private FSM<MyCritter> _fsm;
    private TaskManager _tm;
    #region wander variables
    public bool wandering;
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
    #endregion
    public float movementSpeed;
    public float jumpSpeed;
    public int fingerLevel;
    public float distanceThreshold;
    public float visibilityRange;
    public GameObject armor;
    public GameObject nearestEnemy;
    private GameObject[] enemies;
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
        Services.GM.text.text = _fsm.CurrentState.ToString();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.gameObject;
        if (armor.activeSelf)
        {
            other.SendMessage("HitByArmor");
        }
    }

    private void CritterInit()
    {
        psMain = particles.main;
        psMain.startColor = psSad;
        Services.Touch.MaxFingers = fingerLevel;
    }

    private void WanderAround()
    {
        if (!wandering)
        {
            wandering = true;
            float xPos = transform.position.x;
            float yPos = transform.position.y;
            Vector3 newPos = new Vector3(Random.Range(xPos - wanderWidth, xPos + wanderWidth), Random.Range(yPos - wanderHeight, yPos + wanderHeight), transform.position.z);
            float step = Mathf.Abs(wanderSpeed) * Time.deltaTime;
            //transform.position = Vector3.MoveTowards(transform.position, newPos, step);
            WanderAround startWander = new WanderAround(gameObject, transform.position, newPos, wanderSpeed);
            _tm.Do(startWander);
        }
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
        return Vector3.Distance(transform.position, Services.Touch.primaryTouchPos) <= distanceThreshold;
    }

    ///////////////////////////////////////////////////////////////////////
    // STATES
    ///////////////////////////////////////////////////////////////////////

    private class Wandering : FSM<MyCritter>.State
    {
        public override void OnEnter()
        {
            Context.psMain.startColor = Context.psSad;
            Services.Touch.MaxFingers = Context.fingerLevel;
        }

        public override void Update()
        {
            Context.FindNearbyEnemy();
            if (Services.Touch.touchCount > 0)
            {
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

    private class Jumping : FSM<MyCritter>.State
    {
        public override void OnEnter()
        {
            Context.armor.SetActive(true);
        }
        public override void Update()
        {
            Debug.Log("Jumping");
            if (Context.OnFinger())
            {
                TransitionTo<Following>();
                return;
            }
            else if (Services.Touch.touchCount == 0)
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

    private class Following : FSM<MyCritter>.State
    {
        public override void Update()
        {
            Debug.Log("Following");
            if (Services.Touch.touchCount == 0) TransitionTo<Wandering>();
            else if((Services.Touch.currentFinger.phase == TouchPhase.Began || Services.Touch.currentFinger.phase == TouchPhase.Ended) && Services.Touch.touchCount != 0)
            {
                TransitionTo<Jumping>();
                return;
            }
            else Context.StayOnFinger();
        }
    }

    private class Fleeing : FSM<MyCritter>.State
    {
        public override void Update()
        {
            Context.FindNearbyEnemy();
            if (Context.nearestEnemy == null) TransitionTo<Wandering>();
            else if (Services.Touch.touchCount > 0) TransitionTo<Jumping>();
            else Context.MoveAwayFromEnemy();
        }
    }
}
