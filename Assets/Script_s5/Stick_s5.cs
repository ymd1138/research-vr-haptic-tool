using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// シーン5: 高速で衝突させたときの、判定のダブりを解決する
// 内容は、シーン1に近い。ソレノイドと通信している。
// シーン2,3,4のようにコントローラの位置は取得していない。

public class Stick_s5 : MonoBehaviour
{
    //SerialHandler.cs のクラス
    public SerialHandler serialHandler;
    [SerializeField] CapsuleCollider Col_1; // 棒の実体と同じサイズ
    [SerializeField] CapsuleCollider Col_2; // 棒より大きいサイズ

    public float delay = 1.5f; // 表示までの待機時間（秒）
    private float timer;

    // 一度Enterしてから次にEnterするときに、ソレノイドを動作させるか判定する
    // つまり、連続でソレノイドが動作しないように、一定時間読み込ませないようにする
    private bool isReady;

    void Start()
    {
        timer = 0.0f;
        isReady = false;
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
