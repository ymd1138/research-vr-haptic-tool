using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// クォータニオンの計算など正しいか、ビジュアルで確認
// 棒の上の点に、球を表示させる

public class PointCheck : MonoBehaviour
{
    // Stick オブジェクトのスクリプトの値を読み込む
    SpeedMeasure sm;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = GameObject.Find("Stick"); //オブジェクトを探す
        sm = obj.GetComponent<SpeedMeasure>(); //付いているスクリプトを取得
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = sm.point_P;
    }
}
