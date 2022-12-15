using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cop : BTAgent
{
    public GameObject[] patrolPoints;
    public GameObject robber;


    public override void Start()
    {
        base.Start();

        Sequence selectPatrolPoint = new("Select Patrol Point");
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            Leaf pp = new("Go To " + patrolPoints[i].name, i, GoToPoint);
            selectPatrolPoint.AddChildren(pp);
        }

        Sequence chaseRobber = new("Chase");
        Leaf canSee = new("Can See Robber?", CanSeeRobber);
        Leaf chase = new("Chase Robber", ChaseRobber);

        chaseRobber.AddChildren(canSee);
        chaseRobber.AddChildren(chase);

        Inverter cantSeeRobber = new("Cant See Robber");
        cantSeeRobber.AddChildren(canSee);

        BehaviourTree patrolConditions = new();
        Sequence condition = new("Patrol Conditions");
        condition.AddChildren(cantSeeRobber);
        patrolConditions.AddChildren(condition);
        DepSequence patrol = new("Patrol Until", patrolConditions, agent);
        patrol.AddChildren(selectPatrolPoint);

        Selector beCop = new("be a Cop");
        beCop.AddChildren(patrol);
        beCop.AddChildren(chaseRobber);

        tree.AddChildren(beCop);
    }

    public Node.Status GoToPoint(int i)
    {
        Node.Status s = GoToLocation(patrolPoints[i].transform.position);
        return s;
    }
    public Node.Status CanSeeRobber()
    {
        return CanSee(robber.transform.position, "Robber", 5, 60);
    }
    Vector3 rl;
    public Node.Status ChaseRobber()
    {
        float chaseDistance = 10f;
        if(state == ActionState.IDLE)
        {
            rl = this.transform.position - (transform.position - robber.transform.position).normalized * chaseDistance;
        }
        return GoToLocation(rl);
    }
}
