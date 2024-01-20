using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 棒の前方を表すベクトルを、ビジュアルで確認

public class Vector_forward : MonoBehaviour
{
    // Stick オブジェクトのスクリプトの値を読み込む
    SpeedMeasure sm;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = GameObject.Find("Stick (1)"); //オブジェクトを探す
        sm = obj.GetComponent<SpeedMeasure>(); //付いているスクリプトを取得
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = sm.forward;
    }
}
