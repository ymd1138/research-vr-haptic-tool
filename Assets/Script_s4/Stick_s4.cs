using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick_s4 : MonoBehaviour
{
    public GameObject collisionPointTest;
    public GameObject GodStick;

    [SerializeField] private Transform rightControllerTransform;
    [SerializeField] private Transform TrackingSpace;


    void OnCollisionEnter(Collision collision)
    {
        Vector3 hitPos; // 衝突位置


        // 衝突対象の名前を取得して、ログに出力
        Debug.Log("Collision: " + collision.gameObject.name);
        GetComponent<Renderer>().material.color = Color.red;

        if (collision.gameObject.name == "Cube")
        {
            // 未割り当てを防ぐために、ゼロベクトルを代入しておく
            hitPos = Vector3.zero;
            // 衝突位置を取得して、球を生成する
            foreach (ContactPoint contact in collision.contacts)
            {
                hitPos = contact.point;
                //Instantiate(collisionPointTest, hitPos, Quaternion.identity);
            }
            
            //  右手座標の取得
            Debug.Log("RPos: " + rightControllerTransform.position);
            //  右手回転の取得
            Debug.Log("RAngle: " + rightControllerTransform.eulerAngles);

            //  右手
            //  コントローラーの位置を取得
            Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            Vector3 rpos = TrackingSpace.TransformPoint(localPos);

            //  コントローラーの角度を取得
            Vector3 localRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles;
            Vector3 rrot = TrackingSpace.TransformDirection(localRot);

            Debug.Log("RPos2: " + rpos);
            Debug.Log("RAngle2: " + rrot);


            // ベクトル計算
            Vector3 vec = (hitPos - rpos).normalized;
            //Debug.Log("vec: " + vec);
            //Debug.Log("Q: " + Quaternion.Euler(vec));

            var offsetDistance = Mathf.Sqrt(0.25f * 0.25f * 2);
            Quaternion offsetAngle = Quaternion.Euler(45.0f, 0.0f, 0.0f);

            // 棒を生成
            Instantiate(GodStick, rpos + offsetDistance * vec, Quaternion.Euler(rrot) * offsetAngle);

        }
    }


    void OnCollisionExit(Collision collision)
    {
        Debug.Log("CollisionExit");
        GetComponent<Renderer>().material.color = Color.white;
    }
}
