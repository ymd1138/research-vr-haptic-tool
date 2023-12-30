using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick_s3 : MonoBehaviour
{
    public GameObject collisionPointTest; // 生成する球を設定

    void OnCollisionEnter(Collision collision)
    {
        // 衝突対象の名前を取得して、ログに出力
        Debug.Log("Collision: " + collision.gameObject.name);
        GetComponent<Renderer>().material.color = Color.red;

        /*
        if (collision.gameObject.name == "Cube")
        {
            // 衝突位置を取得して、球を生成する
            Vector3 hitPos;
            foreach (ContactPoint contact in collision.contacts)
            {
                hitPos = contact.point;
                Instantiate(collisionPointTest, hitPos, Quaternion.identity);
            }
        }
        */
    }


    void OnCollisionExit(Collision collision)
    {
        Debug.Log("CollisionExit");
        GetComponent<Renderer>().material.color = Color.white;
    }
}
