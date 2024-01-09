using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// シーン5: 高速で衝突させたときの、判定のダブりを解決する
// 内容は、シーン1に近い。ソレノイドと通信している。

public class Stick_s6 : MonoBehaviour
{
    //SerialHandler.cs のクラス
    public SerialHandler serialHandler;
    [SerializeField] CapsuleCollider Col_1; // 棒の実体と同じサイズ
    [SerializeField] CapsuleCollider Col_2; // 棒より大きいサイズ
    // コントローラの位置取得用
    [SerializeField] private Transform TrackingSpace;


    public float delay = 1.5f; // 表示までの待機時間（秒）
    private float timer;

    // 一度Enterしてから次にEnterするときに、ソレノイドを動作させるか判定する
    // つまり、連続でソレノイドが動作しないように、一定時間読み込ませないようにする
    private bool isReady;

    Vector3 lastPos;

    void Start()
    {
        timer = 0.0f;
        isReady = false;
        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        lastPos = TrackingSpace.TransformPoint(localPos);

        Col_1.enabled = true;
        Col_2.enabled = false;
    }

    void Update()
    {
        if (isReady) // 一度Enterしたら通る
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                timer = 0.0f;
                isReady = false;
                Debug.Log("isReady false");
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Vector3 nowPos = TrackingSpace.TransformPoint(localPos);
        float speed = Vector3.Distance(lastPos, nowPos) / Time.deltaTime;
        //Debug.Log("Time: " + Time.deltaTime);
        lastPos = nowPos;
        //Debug.Log("Speed: " + String.Format("{0:0.00}", speed));

        if (speed > 2.0f)
        {
            Col_1.enabled = false;
            Col_2.enabled = true;
        }
        else
        {
            Col_1.enabled = true;
            Col_2.enabled = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 衝突対象の名前を取得して、ログに出力
        Debug.Log("Collision: " + collision.gameObject.name);
        GetComponent<Renderer>().material.color = Color.red;

        if (!isReady)
        {
            if (collision.gameObject.name == "Cube")
            {
                Debug.Log("Enter");
                isReady = true;

                try
                {
                    serialHandler.Write("1");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }
        }
    }


    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exit");
        GetComponent<Renderer>().material.color = Color.white;

        try
        {
            serialHandler.Write("0");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
