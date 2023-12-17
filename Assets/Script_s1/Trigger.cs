using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    //SerialHandler.csのクラス
    public SerialHandler serialHandler;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Stick")
        {
            Debug.Log("Enter");
            try
            {
                serialHandler.Write("1");
                GetComponent<Renderer>().material.color = Color.green;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Stick")
        {
            Debug.Log("Exit");
            try
            {
                serialHandler.Write("0");
                GetComponent<Renderer>().material.color = Color.white;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }
}
