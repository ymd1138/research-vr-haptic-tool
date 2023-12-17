using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick_s3 : MonoBehaviour
{
    
    void OnCollisionEnter(Collision collision)
    {
        // 衝突対象の名前を取得して、ログに出力
        Debug.Log("Collision: " + collision.gameObject.name);
        GetComponent<Renderer>().material.color = Color.red;

    }
    

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("CollisionExit");
        GetComponent<Renderer>().material.color = Color.white;
    }


/*
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Cube")
        {
            // 衝突対象の名前を取得して、ログに出力
            Debug.Log("Collision: ");
            //Debug.Log("Collision: " + collision.gameObject.name);
            try
            {
                GetComponent<Renderer>().material.color = Color.red;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }
*/
}
