using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArtLover : BTAgent
{
    public GameObject[] art;
    public GameObject home;
    public GameObject frontdoor;
    [Range(0, 1000)]
    public int boredom = 0;

    public bool ticket = false;
    public bool isWaiting = false;

    public override void Start()
    {
        base.Start();
        RandomSelector selectObject = new("Select Object to View");
        for (int i = 0; i < art.Length; i++)
        {
            Leaf gta = new("Go To " + art[i].name, i, GoToArt);
            selectObject.AddChildren(gta);
        }
        Leaf goToFrontDoor = new("Go To Front Door", GoToFrontDoor);
        Leaf goToHome = new("Go To Home", GoToHome);
        Leaf isBored = new("Is Bored?", IsBored);
        Leaf isOpen = new("Is Open?", IsOpen);

        Sequence ViewArt = new("View Art");
        ViewArt.AddChildren(isOpen);
        ViewArt.AddChildren(isBored);
        ViewArt.AddChildren(goToFrontDoor);

        Leaf noTicket = new("Wait for Ticket", NoTicket);
        Leaf isWaiting = new("Waiting for Ticket", IsWaiting);

        BehaviourTree waitForTicket = new();
        waitForTicket.AddChildren(noTicket);

        Loop getTicket = new("Ticket", waitForTicket);
        getTicket.AddChildren(isWaiting);

        ViewArt.AddChildren(getTicket);

        BehaviourTree whileBored = new();
        whileBored.AddChildren(isBored);
        Loop lookAtPaintings = new("Look", whileBored);
        lookAtPaintings.AddChildren(selectObject);

        ViewArt.AddChildren(lookAtPaintings);
        ViewArt.AddChildren(goToHome);

        BehaviourTree galleryOpenCondition = new();
        galleryOpenCondition.AddChildren(isOpen);
        DepSequence bePatron = new("Be An Art Patron",galleryOpenCondition,agent);
        bePatron.AddChildren(ViewArt);

        Selector viewArtWithFallback = new("View Art With Fallback");
        viewArtWithFallback.AddChildren(bePatron);
        viewArtWithFallback.AddChildren(goToHome);

        tree.AddChildren(viewArtWithFallback);

        StartCoroutine(nameof(IncreaseBordom));
    }

    IEnumerator IncreaseBordom()
    {
        while (true)
        {
            boredom = Mathf.Clamp(boredom + 20, 0, 1000);
            yield return new WaitForSeconds(Random.Range(1, 5));
        }
    }

    public Node.Status GoToArt(int i)
    {
        if (!art[i].activeSelf)
            return Node.Status.FAILURE;
        Node.Status s = GoToLocation(art[i].transform.position);
        if(s == Node.Status.SUCCESS)
        {
            boredom = Mathf.Clamp(boredom - 100, 0, 1000);
        }
        return s;
    }
    public Node.Status GoToFrontDoor()
    {
        Node.Status s = GoToDoor(frontdoor);
        return s;
    }
    public Node.Status GoToHome()
    {
        Node.Status s = GoToLocation(home.transform.position);
        isWaiting = false;
        return s;
    }
    Node.Status IsBored()
    {
        if (boredom < 100)
            return Node.Status.FAILURE;
        else
            return Node.Status.SUCCESS;
    }
    public Node.Status NoTicket()
    {
        if (ticket || IsOpen() == Node.Status.FAILURE)
        {
            return Node.Status.FAILURE;
        }
        else
            return Node.Status.SUCCESS;
    }
    public Node.Status IsWaiting()
    {
        if (BlackBoard.Instance.RegisterPatron(this.gameObject))
        {
            isWaiting = true;
            return Node.Status.SUCCESS;
        }
        else
            return Node.Status.FAILURE;
    }

}
