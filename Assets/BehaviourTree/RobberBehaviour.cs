using System.Collections;
using UnityEngine;

public class RobberBehaviour : BTAgent
{
    public GameObject diamond;
    public GameObject van;
    public GameObject backdoor;
    public GameObject frontdoor;
    public GameObject painting;
    public GameObject cop;

    public GameObject[] art;

    GameObject pickup;

    [Range(0, 1000)]
    public int money = 800;
    Leaf goToFrontdoor;
    Leaf goToBackdoor;
    public override void Start()
    {
        base.Start();
        Leaf hasGotMoney = new("Has Got Money", HasMoney);
        Leaf goToDiamond = new("Go To Diamond", GoToDiamond, 1);
        Leaf goToPainting = new("Go To Painting", GoToPainting, 2);

        RandomSelector selectObject = new("Select Object to Steal");
        for (int i = 0; i < art.Length; i++)
        {
            Leaf gta = new("Go To " + art[i].name, i, GoToArt);
            selectObject.AddChildren(gta);
        }

        goToFrontdoor = new("Go To FrontDoor", GoToFrontDoor, 1);
        goToBackdoor = new("Go To BackDoor", GoToBackDoor, 2);
        Leaf goToVan = new("Go To Van", GoToVan);
        PSelector opendoor = new("Open Door");

        Sequence runAway = new("Run Away");
        Leaf canSee = new("Can See Cop?", CanSeeCop);
        Leaf flee = new("Flee From Cop", FleeFromCop);

        Inverter invertMoney = new("Invert Money");
        invertMoney.AddChildren(hasGotMoney);

        opendoor.AddChildren(goToFrontdoor);
        opendoor.AddChildren(goToBackdoor);

        runAway.AddChildren(canSee);
        runAway.AddChildren(flee);

        Inverter cantSeeCop = new("Cant See Cop");
        cantSeeCop.AddChildren(canSee);

        Leaf isOpen = new("Is Open?",IsOpen);
        Inverter isClose = new("Is Closed");
        isClose.AddChildren(isOpen);

        BehaviourTree stealConditions = new();
        Sequence conditions = new("Stealing Conditions");
        conditions.AddChildren(invertMoney);
        conditions.AddChildren(isClose);
        conditions.AddChildren(cantSeeCop);
        stealConditions.AddChildren(conditions);
        DepSequence steal = new("Steal Something", stealConditions, agent);

        //steal.AddChildren(invertMoney);
        steal.AddChildren(opendoor);
        steal.AddChildren(selectObject);
        steal.AddChildren(goToVan);

        Selector stealWithFallback = new("Steal With Fallback");
        stealWithFallback.AddChildren(steal);
        stealWithFallback.AddChildren(goToVan);
        Selector beThief = new("Be a thief");
        beThief.AddChildren(stealWithFallback);
        beThief.AddChildren(runAway);

        tree.AddChildren(beThief);

        tree.PrintTree();

        StartCoroutine(nameof(DecreaseMoney));
    }

    IEnumerator DecreaseMoney()
    {
        while (true)
        {
            money = Mathf.Clamp(money - 20, 0, 1000);
            yield return new WaitForSeconds(Random.Range(1, 5));
        }
    }


    public Node.Status CanSeeCop()
    {
        return CanSee(cop.transform.position, "Cop", 10, 90);

    }
    public Node.Status FleeFromCop()
    {
        return Flee(cop.transform.position, 10);

    }
    public Node.Status HasMoney()
    {
        if (money < 500)
            return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }
    public Node.Status GoToDiamond()
    {
        if (!diamond.activeSelf)
            return Node.Status.FAILURE;
        Node.Status s = GoToLocation(diamond.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            diamond.transform.parent = this.gameObject.transform;
            pickup = diamond;
        }
        return s;
    }
    public Node.Status GoToPainting()
    {
        if (!painting.activeSelf)
            return Node.Status.FAILURE;
        Node.Status s = GoToLocation(painting.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            painting.transform.parent = this.gameObject.transform;
            pickup = painting;
        }
        return s;
    }
    public Node.Status GoToArt(int i)
    {
        if (!art[i].activeSelf)
            return Node.Status.FAILURE;
        Node.Status s = GoToLocation(art[i].transform.position);
        if (s == Node.Status.SUCCESS)
        {
            art[i].transform.parent = this.gameObject.transform;
            pickup = art[i];
        }
        return s;
    }
    public Node.Status GoToBackDoor()
    {
        Node.Status s = GoToDoor(backdoor);
        if (s == Node.Status.FAILURE)
            goToBackdoor.sortOrder = 10;
        else
            goToBackdoor.sortOrder = 1;
        return s;

    }
    public Node.Status GoToFrontDoor()
    {
        Node.Status s = GoToDoor(frontdoor);
        if (s == Node.Status.FAILURE)
            goToFrontdoor.sortOrder = 10;
        else
            goToFrontdoor.sortOrder = 1;
        return s;
    }
    public Node.Status GoToVan()
    {
        Node.Status s = GoToLocation(van.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            if (pickup != null)
            {
                money += 300;
                pickup.SetActive(false);
                pickup = null;
            }
        }
        return s;
    }

}
