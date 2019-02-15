using UnityEngine;
using BehaviorTree;

public class PlayerAI : MonoBehaviour
{
    private Tree<PlayerAI> tree;

    void Start()
    {
        tree = new Tree<PlayerAI>(new Selector<PlayerAI>(
            new Sequence<PlayerAI>(
                new NoFingersAvailable(),
                new Wander()
                ),
            new Wander()
            ));
    }

    private void Update()
    {
        tree.Update(this);
    }

    ///////////////////////////////
    ///////////NODES///////////////
    ///////////////////////////////

    /////////CONDITIONS///////////
    private class NoFingersAvailable : BehaviorTree.Node<PlayerAI>
    {
        public override bool Update(PlayerAI context)
        {
            return true;
        }
    }

    //////////ACTIONS//////////
    private class Wander : BehaviorTree.Node<PlayerAI>
    {
        public override bool Update(PlayerAI context)
        {
            Debug.Log("okeydoke artichoke");
            return true;
        }
    }

}

