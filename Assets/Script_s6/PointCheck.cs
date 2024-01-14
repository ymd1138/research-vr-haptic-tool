using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// クォータニオンの計算など正しいか、ビジュアルで確認
// 棒の上の点に、球を表示させる

public class PointCheck : MonoBehaviour
{
    // Stick オブジェクトのスクリプトの値を読み込む
    SpeedMeasure sm;
    private int sphere;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = GameObject.Find("Stick"); //オブジェクトを探す
        sm = obj.GetComponent<SpeedMeasure>(); //付いているスクリプトを取得

        if (this.gameObject.name == "Sphere1")
        {
            sphere = 1;
        }
        else if (this.gameObject.name == "Sphere2")
        {
            sphere = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sphere == 1)
        {
            transform.position = sm.P_top;
        }
        else if (sphere == 2)
        {
            transform.position = sm.P_bottom;
        }
    }
}
