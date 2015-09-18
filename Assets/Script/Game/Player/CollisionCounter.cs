using UnityEngine;
using System.Collections;

public class CollisionCounter : MonoBehaviour
{
    public int Counter;

    void Start()
    {
        Counter = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        Counter++;
    }

    void OnTriggerExit(Collider other)
    {
        Counter--;
    }
}