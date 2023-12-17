using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_s3 : MonoBehaviour
{
    //SerialHandler.csのクラス
    public SerialHandler serialHandler;
    public GameObject collisionPointTest;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Stick")
        {

            Vector3 hitPos;
            foreach (ContactPoint contact in collision.contacts)
            {
                hitPos = contact.point;
                Instantiate(collisionPointTest, hitPos, Quaternion.identity);
            }
        }
    }

}

