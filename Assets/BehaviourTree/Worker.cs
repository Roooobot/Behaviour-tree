using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : BTAgent
{
    public GameObject office;

    public override void Start()
    {
        base.Start();
        Leaf patronStillWaiting = new("Is Patron Waiting?", PatronWaiting);
        Leaf allocatePatron = new("Allocate Patron", AllocatePatron);
        Leaf goToPatron = new("Go To Patron", GoToPatron);
        Leaf goToOffice = new("Go To Office", GoToOffice);


        Sequence getPatron = new("Find a Patron");
        getPatron.AddChildren(allocatePatron);

        BehaviourTree waiting= new();
        waiting.AddChildren(patronStillWaiting);

        DepSequence moveToPatron = new("Moving To Patron", waiting, agent);
        moveToPatron.AddChildren(goToPatron);

        getPatron.AddChildren(moveToPatron);

        Selector beWorker = new("Be a Worker");
        beWorker.AddChildren(getPatron);
        beWorker.AddChildren(goToOffice);

        tree.AddChildren(beWorker);

    }
    GameObject patron;
    public Node.Status PatronWaiting()
    {
        if (patron == null)
            return Node.Status.FAILURE;
        if (patron.GetComponent<ArtLover>().isWaiting)
            return Node.Status.SUCCESS;
        return Node.Status.FAILURE;
    }

    public Node.Status AllocatePatron()
    {
        if (BlackBoard.Instance.patrons.Count == 0)
            return Node.Status.FAILURE;
        patron = BlackBoard.Instance.patrons.Pop();
        if(patron == null)
            return Node.Status.FAILURE; 
        return Node.Status.SUCCESS;
    }
    public Node.Status GoToPatron()
    {
        if (patron == null)
            return Node.Status.FAILURE;
        Node.Status s = GoToLocation(patron.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            patron.GetComponent<ArtLover>().ticket = true;
            patron = null;
        }
        return s;
    }
    public Node.Status GoToOffice()
    {
        Node.Status s = GoToLocation(office.transform.position);
        patron = null;
        return s;
    }
}
