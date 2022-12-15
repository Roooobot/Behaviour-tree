using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class BlackBoard:MonoBehaviour
{
    public float timeOfDay;
    public TMP_Text clock;
    public Stack<GameObject> patrons = new();
    public int openTime = 5;
    public int closeTime = 20;

    static BlackBoard instance;
    public static BlackBoard Instance
    {
        get
        {
            if (!instance)
            {
                BlackBoard[] blackBoards = GameObject.FindObjectsOfType<BlackBoard>();
                if(blackBoards != null)
                {
                    if(blackBoards.Length == 1)
                    {
                        instance =blackBoards[0];
                        return instance;
                    }
                }
                GameObject go = new GameObject("BlackBoard",typeof(BlackBoard));
                instance = go.GetComponent<BlackBoard>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
        set
        {
            instance = value as BlackBoard;
        }
    }

    private void Start()
    {
        StartCoroutine(nameof(UpdateClock));
    }

    IEnumerator UpdateClock()
    {
        while (true)
        {
            timeOfDay++;
            if(timeOfDay>23)
                timeOfDay=0;
            clock.text = timeOfDay + ":00";
            if (timeOfDay == closeTime)
                patrons.Clear();
            yield return new WaitForSeconds(1);
        }
    }

    public bool RegisterPatron(GameObject p)
    {
        patrons.Push(p);

        return true;
    }
    public void DeregisterPatron()
    {
        //patron = null;
    }
}
