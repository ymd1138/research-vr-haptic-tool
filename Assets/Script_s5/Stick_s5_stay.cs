using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// シーン5: 高速で衝突させたときの、判定のダブりを解決する
// 内容は、シーン1に近い。ソレノイドと通信している。
// OnCollisionStay を使ったバージョン

public class Stick_s5_stay : MonoBehaviour
{
    //SerialHandler.cs のクラス
    public SerialHandler serialHandler;
    [SerializeField] CapsuleCollider Col_1; // 棒の実体と同じサイズ
    [SerializeField] CapsuleCollider Col_2; // 棒より大きいサイズ
    // コントローラの位置取得用
    [SerializeField] private Transform TrackingSpace;


    public float delay = 1.5f; // 表示までの待機時間（秒）
    public float speedBorder = 1.5f;
    private float timer;

    // 一度Enterしてから次にEnterするときに、ソレノイドを動作させるか判定する
    // つまり、連続でソレノイドが動作しないように、一定時間読み込ませないようにする
    private bool isReady; // falseのときは動作させない
    private bool isSolenoidON; // ソレノイドがONかどうか判定
    private bool isStay; // 離れたかを判定

    private int stayTime; // Stayの時間を計測
    private int lastTime;

    Vector3 lastPos;

    void Start()
    {
        timer = 0.0f;
        isReady = true;
        isSolenoidON = false;
        isStay = false;
        stayTime = 0;
        lastTime = 0;

        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        lastPos = TrackingSpace.TransformPoint(localPos);

        Col_1.enabled = true;
        Col_2.enabled = false;
    }

    void Update()
    {
        // Stayしているかの判定
        if (isStay)
        {
            if (stayTime == lastTime)
            {
                // Stayしていない、つまり棒が離れたということ。
                isStay = false;
                stayTime = 0;
                lastTime = 0;
                Debug.Log("ExitExit");

                if (isSolenoidON)
                {
                    isSolenoidON = false;
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
            else
            {
                lastTime = stayTime;
            }
        }

        if (!isReady) // 一度Enterしたら（falseなら）通る
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                timer = 0.0f;
                isReady = true;
                Debug.Log("isReady");
            }
        }
    }

    void FixedUpdate()
    {
        // 速度を計算
        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Vector3 nowPos = TrackingSpace.TransformPoint(localPos);
        float speed = Vector3.Distance(lastPos, nowPos) / Time.deltaTime;
        //Debug.Log("Time: " + Time.deltaTime);
        lastPos = nowPos;
        // Debug.Log("Speed: " + String.Format("{0:0.00}", speed)); // 速度を表示

        // 速度が一定以上であれば、大きいコライダーがONになる
        if (speed > speedBorder)
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

    void OnCollisionStay(Collision collision)
    {
        stayTime++;

        // stayフラグをON
        if (!isStay)
        {
            isStay = true;
        }

        // ソレノイドのフラグがOFFであれば、ONにする。Enterの代わり。
        if (!isSolenoidON)
        {
            isSolenoidON = true;

            // 衝突対象の名前を取得して、ログに出力
            Debug.Log("Collision: " + collision.gameObject.name);
            GetComponent<Renderer>().material.color = Color.red;

            if (isReady)
            {
                if (collision.gameObject.name == "Cube")
                {
                    Debug.Log("Enter");
                    isReady = false;

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
    }

}
