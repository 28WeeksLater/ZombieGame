using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public EventHandler[] eventList;

    private static EventManager instance = null;
    public static EventManager Instance
    {
        get
        {
            if (instance == null)
                return null;

            else return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public void EventRestart()
    {
        foreach(var i in eventList)
        {
            if (!i.isClear)
            {
                if (i.isActivated)
                {
                    i._restart.Invoke();
                }
                return;
            }
        }
    }
}
