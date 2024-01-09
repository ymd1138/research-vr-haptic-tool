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

    private StreamWriter sw;

    Vector3 lastPos;
    private float time;

    DateTime dt;
    DateTime collisionTime;


    float StickOffset = Convert.ToSingle(Math.Sqrt(0.25 * 0.25 * 2));


    // Start is called before the first frame update
    void Start()
    {
        dt = DateTime.Now;
        //String dtString = dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() + "-" + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString();
        String dtString = dt.ToString("yyyyMMdd-HHmmss");

        // ファイル作成
        sw = new StreamWriter(@"Output/SaveData-" + dtString + ".csv", false, Encoding.UTF8);
        //        sw = new StreamWriter(@"Output/SaveData" + dtString + ".csv", false, Encoding.GetEncoding("Shift_JIS"));
        // string[] s1 = { "time", "pos", "speed", "ac", "rot", "rotSpeed", "rotAc" };
        string[] s1 = { "time", "pos.x", "pos.y", "pos.z", "speed", "rot.x", "rot.y", "rot.z", };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        //  コントローラーの位置を取得
        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Vector3 rpos = TrackingSpace.TransformPoint(localPos);

        // 速度を計算
        Vector3 nowPos = TrackingSpace.TransformPoint(localPos);
        float speed = Vector3.Distance(lastPos, nowPos) / Time.deltaTime;
        //Debug.Log("Time: " + Time.deltaTime);
        lastPos = nowPos;
        // Debug.Log("Speed: " + String.Format("{0:0.00}", speed));

        //  コントローラーの角度を取得
        Vector3 localRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles;
        Vector3 rrot = TrackingSpace.TransformDirection(localRot);

        Quaternion rot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        DirCalc(rot);

        // 角速度
        //Vector3 localRotSpeed = OVRInput.GetLocalControllerAngularAcceleration(OVRInput.Controller.RTouch).eulerAngles;

        // ファイル書き込み
        SaveData(time.ToString(), rpos.x.ToString(), rpos.y.ToString(), rpos.z.ToString(), speed.ToString(), rrot.x.ToString(), rrot.y.ToString(), rrot.z.ToString());
    }

    // コントローラの座標と回転から、棒の方向を求める
    public void DirCalc(Quaternion rot)
    {
        Vector3 stick = new Vector3(0.0f, 0.5f, 0.0f);

    }

    // ファイル書き込み関数
    public void SaveData(string txt1, string txt2, string txt3, string txt4, string txt5, string txt6, string txt7, string txt8)
    {
        string[] s1 = { txt1, txt2, txt3, txt4, txt5, txt6, txt7, txt8 };
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
