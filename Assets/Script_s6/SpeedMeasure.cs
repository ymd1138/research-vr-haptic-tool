using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Oculus.Interaction.Body.Input;
using UnityEngine;

// 棒を振る速さを計測して、データに書き出す。

public class SpeedMeasure : MonoBehaviour
{
    [SerializeField] private Transform TrackingSpace; // コントローラの位置取得用

    // ファイル読み書き
    private StreamWriter sw;
    private StreamWriter sw2;

    // 前回の位置
    private Vector3 previousPos = Vector3.zero;
    // 前回の回転
    private Quaternion previousRot = Quaternion.identity;
    // 角速度ベクトル
    private Vector3 angularVelocityVector = Vector3.zero;

    private float time;
    DateTime dt;
    DateTime collisionTime;

    /*** Pythonの計算で使う / 棒の上の点の位置をビジュアルで確認する用 ***/
    // 棒の先端の点の初期位置
    private Vector3 shift_top = new Vector3(0.0f, 0.60355f, 0.60355f);
    // 棒の根元の点の初期位置
    private Vector3 shift_bottom = new Vector3(0.0f, -0.10355f, -0.10355f);

    // 棒上の点P 
    public Vector3 P_top = Vector3.zero; // 先端の点
    public Vector3 P_bottom = Vector3.zero; // 根元の点


    // Start is called before the first frame update
    void Start()
    {
        dt = DateTime.Now;
        String dtString = dt.ToString("yyyyMMdd-HHmmss");

        // ファイル作成
        sw = new StreamWriter(@"OutputData/SaveData-" + dtString + ".csv", false, Encoding.UTF8);
        // sw = new StreamWriter(@"Output/SaveData" + dtString + ".csv", false, Encoding.GetEncoding("Shift_JIS"));
        // string[] s1 = { "time", "pos", "speed", "ac", "rot", "rotSpeed", "rotAc" };
        string[] s1 = { "time", "pos.x", "pos.y", "pos.z", "speed", "rot.x", "rot.y", "rot.z", "deg/flame", "deg/s", "rad/s", "angleVector.x", "angleVector.y", "angleVector.z",
                        "posP1.x", "posP1.y", "posP1.z", "posP2.x", "posP2.y", "posP2.z"};
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
    }


    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        // コントローラーの位置を取得
        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Vector3 rpos = TrackingSpace.TransformPoint(localPos);

        /*** 速度を計算 ***/
        // 前回の位置(lastPos)と今回の位置(rpos)の差を経過時間で割って、速度を求めている。
        float speed = Vector3.Distance(previousPos, rpos) / Time.deltaTime;
        //Debug.Log("Time: " + Time.deltaTime);

        // 前回の位置を更新
        previousPos = rpos;
        // Debug.Log("Speed: " + String.Format("{0:0.00}", speed));


        // コントローラーの角度を取得
        Vector3 localRot_euler = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles;
        Vector3 rrot_euler = TrackingSpace.TransformDirection(localRot_euler);

        /*** 角速度を計算 ***/
        // 現在のクォータニオン
        // この部分は transform.rotation で求めても同じ
        Quaternion rot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        // 回転の変化量を計算
        Quaternion deltaRot = Quaternion.Inverse(previousRot) * rot;
        // クォータニオンを角度と回転軸に変換
        // angle は、[deg/flame] (オイラー角/1フレーム)   
        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);

        // deg/s を求める
        float angularSpeed_euler = angle / Time.deltaTime;
        // 角速度 [rad/s] を計算
        float angularSpeed = (angle * Mathf.Deg2Rad) / Time.deltaTime;
        // 角速度ベクトル（xyz）を計算。意味がよく分からないので保留。クォータニオンが求まっているので、予測にはクォータニオンを利用する
        // angularVelocityVector = axis * angularSpeed;

        // 前回の角度を更新
        previousRot = rot;

        // 予測するときは、derlaRot (クォータニオン) のまま扱う

        //DirCalc(rot);

        // オイラー角で見て、どちらの方法(transform と OVRInput)を使っても値が同じであることを確認した
        // Debug.Log("world1: " + transform.eulerAngles + " / world2: " + rrot_euler + " / local1: " + transform.localEulerAngles + " / local2: " + localRot_euler);

        // 棒の先端の点を計算
        P_top = rpos + rot * shift_top;

        // 棒の根元の点を計算
        P_bottom = rpos + rot * shift_bottom;

        // ファイル書き込み
        SaveData(time.ToString(), rpos.x.ToString(), rpos.y.ToString(), rpos.z.ToString(), speed.ToString(),
                 rrot_euler.x.ToString(), rrot_euler.y.ToString(), rrot_euler.z.ToString(),
                 angle.ToString(), angularSpeed_euler.ToString(), angularSpeed.ToString(),
                 angularVelocityVector.x.ToString(), angularVelocityVector.y.ToString(), angularVelocityVector.z.ToString(),
                 P_top.x.ToString(), P_top.y.ToString(), P_top.z.ToString(),
                 P_bottom.x.ToString(), P_bottom.y.ToString(), P_bottom.z.ToString());
    }


    // コントローラの座標と回転から、棒の方向を求める
    public void DirCalc(Quaternion rot)
    {
        Vector3 stick = new Vector3(0.0f, 0.5f, 0.0f);

    }

    // ファイル書き込み関数
    public void SaveData(string txt1, string txt2, string txt3, string txt4, string txt5, string txt6, string txt7, string txt8,
    string txt9, string txt10, string txt11, string txt12, string txt13, string txt14, string txt15, string txt16, string txt17,
    string txt18, string txt19, string txt20)
    {
        string[] s1 = { txt1, txt2, txt3, txt4, txt5, txt6, txt7, txt8, txt9, txt10, txt11, txt12, txt13, txt14, txt15, txt16, txt17, txt18, txt19, txt20 };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 衝突対象の名前を取得して、ログに出力
        Debug.Log("Collision: " + collision.gameObject.name);
        GetComponent<Renderer>().material.color = Color.red;

        if (collision.gameObject.name == "Cube")
        {
            Debug.Log("Enter: " + time.ToString());
        }

    }
    void OnCollisionExit(Collision collision)
    { }

    private void OnApplicationQuit()
    {
        sw.Flush(); // StreamWriterのバッファに書き出し残しがないか確認
        sw.Close(); // アプリケーション終了時に書き込みを終了する
    }
}
