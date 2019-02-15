﻿using UnityEngine;
using BehaviorTree;
using Selector = BehaviorTree.Selector<PlayerAI>;
using Sequence = BehaviorTree.Sequence<PlayerAI>;
using Tree = BehaviorTree.Tree<PlayerAI>;

public class PlayerAI : MonoBehaviour
{
    private Tree<PlayerAI> tree;
    public float wanderWidth;
    public float wanderHeight;
    public float wanderSpeed;
    public float movementSpeed;
    public bool wandering = false;
    private bool fleeing = false;
    public float visibilityRange;
    private TaskManager tm;
    private GameObject[] enemies;
    [SerializeField]
    public GameObject nearestEnemy;

    void Start()
    {
        tm = new TaskManager();
        tree = new Tree(new Selector(
            //Do we see a finger?
            new Sequence(
                new FingersAvailable(),
                new JumpToNearestFinger()
                ),
            //Is an enemy nearby?
            new Sequence(
                new NoFingersAvailable(),
                new IsEnemyNear(),
                new Flee()
                ),
            new Wander()
            ));
    }

    private void Update()
    {
        tm.Update();
        tree.Update(this);
    }

    private void WanderAround()
    {
        if (!wandering)
        {
            wandering = true;
            float xPos = transform.position.x;
            float yPos = transform.position.y;
            Vector3 newPos = new Vector3(Random.Range(xPos - wanderWidth,xPos + wanderWidth), Random.Range(yPos - wanderHeight, yPos + wanderHeight), transform.position.z);
            float step = Mathf.Abs(wanderSpeed) * Time.deltaTime;
            //transform.position = Vector3.MoveTowards(transform.position, newPos, step);
            WanderAround startWander = new WanderAround(gameObject, transform.position, newPos, wanderSpeed);
            tm.Do(startWander);
        }
    }

    private void MoveAwayFromEnemy()
    {
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
                fleeing = false;
            }
        }
    }

    private void Walk()
    {
        Vector3 currentPos = transform.position;
        float step = Mathf.Abs(movementSpeed) * Time.deltaTime;
        transform.position = Vector3.MoveTowards(currentPos, Services.Touch.primaryTouch, step);
    }

    ///////////////////////////////
    ///////////NODES///////////////
    ///////////////////////////////

    /////////CONDITIONS///////////
    private class FingersAvailable : Node<PlayerAI>
    {
        public override bool Update(PlayerAI context)
        {
            return Services.Touch.touchCount > 0;
        }
    }

    private class NoFingersAvailable : Node<PlayerAI>
    {
        public override bool Update(PlayerAI context)
        {
            return Services.Touch.touchCount == 0;
        }
    }

    private class IsEnemyNear : Node<PlayerAI>
    {
        public override bool Update(PlayerAI context)
        {
            context.FindNearbyEnemy();
            if (context.nearestEnemy == null) return false;
            else
            {
                return Vector3.Distance(context.transform.position, context.nearestEnemy.transform.position) < context.visibilityRange;
            }
        }
    }

    //////////ACTIONS//////////
    private class Wander : Node<PlayerAI>
    {
        public override bool Update(PlayerAI context)
        {
            context.WanderAround();
            Debug.Log("wandering");
            return true;
        }
    }

    private class JumpToNearestFinger : Node<PlayerAI>
    {
        public override bool Update(PlayerAI context)
        {
            context.Walk();
            Debug.Log("I love you, finger");
            return true;
        }
    }

    private class Flee : Node<PlayerAI>
    {
        public override bool Update(PlayerAI context)
        {
            context.MoveAwayFromEnemy();
            context.fleeing = true;
            Debug.Log("fleeing");
            return true;
        }
    }

}
