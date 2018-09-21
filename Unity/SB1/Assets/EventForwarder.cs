using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventForwarder : MonoBehaviour
{
    public GameObject[] listeners;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RelayEvent(collision, "OnTriggerEnter2D");

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        RelayEvent(collision, "OnTriggerExit2D");
    }

    void RelayEvent<T>(T type, string name)
    {
        foreach(var go in listeners)
        {
            go.SendMessage(name, type, SendMessageOptions.DontRequireReceiver);
        }
    }
}
