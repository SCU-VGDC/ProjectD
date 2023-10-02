using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager singleton;

    Queue<msg> EventQueue;
    public int QueueSize;

    Dictionary<string, Action> OnEventCommand = new Dictionary<string, Action>();

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        
    }

    private void CoolWayToSetUpFunctions()
    {

    }

    private void Update()
    {
        if(QueueSize != 0)
        {
            msg Popped = EventQueue.Dequeue();
            ResolveEvent(Popped);
        }
    }

    private void ResolveEvent(msg NextPop)
    {
        QueueSize--;


    }

    public void AddEvent(msg sendMessage)
    {
        QueueSize++;
        EventQueue.Enqueue(sendMessage);
    }
}

public class msg
{
    string data;
    GameObject Sender;
}
