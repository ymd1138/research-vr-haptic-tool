using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Stick")
        {
            Debug.Log("Enter");
            GetComponent<Renderer>().material.color = Color.green;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Stick")
        {
            Debug.Log("Exit");
            GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
