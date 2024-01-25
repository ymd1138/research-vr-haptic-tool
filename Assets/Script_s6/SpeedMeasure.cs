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
    public GameObject collisionPointSphere;

    /*** ファイル読み書き ***/
    private StreamWriter sw; // メインデータ
    private StreamWriter sw2; // 衝突位置、時間データ

    // 前回の位置
    private Vector3 previousPos = Vector3.zero;
    // 前回の回転
    private Quaternion previousRot = Quaternion.identity;
    // 角速度ベクトル
    private Vector3 angularVelocityVector = Vector3.zero;

    public float time;
    DateTime dt;
    DateTime collisionTime;

    /*** Pythonの計算で使う / 棒の上の点の位置をビジュアルで確認する用 ***/
    // 棒の先端の点の初期位置
    private Vector3 shift_top = new Vector3(0.0f, 0.60355f, 0.60355f);
    // 棒の根元の点の初期位置
    private Vector3 shift_bottom = new Vector3(0.0f, -0.10355f, -0.10355f);

    // 棒上の点P 
    public Vector3 P_bottom = Vector3.zero; // 根元の点
    public Vector3 P_top = Vector3.zero; // 先端の点

    // コントローラーの前方を表すベクトル
    public Vector3 forward_initial = new Vector3(0.0f, -1.0f, 1.0f);
    public Vector3 forward = Vector3.zero;


    private string filePath1;
    private string filePath2;
    // Start is called before the first frame update
    void Start()
    {
        dt = DateTime.Now;
        String dtString = dt.ToString("yyyyMMdd-HHmmss");

        filePath1 = @"OutputData\SaveData-" + dtString + ".csv";
        filePath2 = @"OutputData\SaveData-" + dtString + "-collision.csv";
        // ファイル作成
        //sw = new StreamWriter(filePath1, false, Encoding.UTF8);
        // sw = new StreamWriter(@"Output/SaveData" + dtString + ".csv", false, Encoding.GetEncoding("Shift_JIS"));
        // string[] s1 = { "time", "pos", "speed", "ac", "rot", "rotSpeed", "rotAc" };
        string[] s1 = { "time", "pos_x", "pos_y", "pos_z", "speed", "rot_x", "rot_y", "rot_z", "deg/flame", "deg/s", "rad/s",
                        "P_bottom_x", "P_bottom_y", "P_bottom_z", "P_top_x", "P_top_y", "P_top_z",
                        "forward_x", "forward_y", "forward_z"};
        //"angleVector_x", "angleVector_y", "angleVector_z",
        string s2 = string.Join(",", s1);
        System.IO.File.AppendAllText(filePath1, s2 + Environment.NewLine);
        //sw.WriteLine(s2);
        sw2 = new StreamWriter(filePath2, false, Encoding.UTF8);
        string[] s3 = { "collisionTime", "collisionPos_x", "collisionPos_y", "collisionPos_z" };
        string s4 = string.Join(",", s3);
        sw2.WriteLine(s4);
    }

    // Update is called once per frame
    void FixedUpdate()
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


        System.Threading.Thread thread = new System.Threading.Thread(() =>
        {
            // 前回の角度を更新
            previousRot = rot;

            // 予測するときは、derlaRot (クォータニオン) のまま扱う

            // オイラー角で見て、どちらの方法(transform と OVRInput)を使っても値が同じであることを確認した
            // Debug.Log("world1: " + transform.eulerAngles + " / world2: " + rrot_euler + " / local1: " + transform.localEulerAngles + " / local2: " + localRot_euler);

            // 棒の根元の点を計算
            P_bottom = rpos + rot * shift_bottom;
            // 棒の先端の点を計算
            P_top = rpos + rot * shift_top;

            // 棒の前方ベクトルを計算
            // forward = rpos + rot * forward_initial; // 前方ベクトル確認用の球の位置
            forward = rot * forward_initial;


            // ファイル書き込み
            SaveData1(filePath1, time.ToString(), rpos.x.ToString(), rpos.y.ToString(), rpos.z.ToString(), speed.ToString(),
                     rrot_euler.x.ToString(), rrot_euler.y.ToString(), rrot_euler.z.ToString(),
                     angle.ToString(), angularSpeed_euler.ToString(), angularSpeed.ToString(),
                     P_bottom.x.ToString(), P_bottom.y.ToString(), P_bottom.z.ToString(),
                     P_top.x.ToString(), P_top.y.ToString(), P_top.z.ToString(),
                     forward.x.ToString(), forward.y.ToString(), forward.z.ToString());
            // angularVelocityVector.x.ToString(), angularVelocityVector.y.ToString(), angularVelocityVector.z.ToString(),

        });
        thread.Start();
    }

    // 予測方法1: 前からの変化量のみ（等速運動とみなす）
    public void Method1()
    {

    }

    // ファイル書き込み関数
    public void SaveData1(string filePath, string txt1, string txt2, string txt3, string txt4, string txt5, string txt6, string txt7, string txt8,
                        string txt9, string txt10, string txt11, string txt12, string txt13, string txt14, string txt15, string txt16, string txt17,
                        string txt18, string txt19, string txt20)
    {
        string[] s1 = { txt1, txt2, txt3, txt4, txt5, txt6, txt7, txt8, txt9, txt10, txt11, txt12, txt13, txt14, txt15, txt16, txt17, txt18, txt19, txt20};
        string s2 = string.Join(",", s1);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.AppendAllText(filePath, s2 + Environment.NewLine);
        }
        else
        {
            System.IO.File.WriteAllText(filePath, s2 + Environment.NewLine);
        }
    }

    // 衝突位置用のファイル書き込み関数
    public void SaveData2(string txt1, string txt2, string txt3, string txt4)
    {
        string[] s3 = { txt1, txt2, txt3, txt4 };
        string s4 = string.Join(",", s3);
        sw2.WriteLine(s4);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 衝突対象の名前を取得して、ログに出力
        Debug.Log("Collision: " + collision.gameObject.name);
        GetComponent<Renderer>().material.color = Color.red;

        if (collision.gameObject.tag == "Cube")
        {
            // どちらの方法でも同じ値だった
            /*
            Vector3 hitPos = collision.contacts[0].point;
            Debug.Log("Enter: " + time + " / " + hitPos.ToString("F7"));
            */
            // 推奨されているこちらの方法で位置を取得する
            Vector3 hitPos = collision.GetContact(0).point;
            Instantiate(collisionPointSphere, hitPos, Quaternion.identity);
            SaveData2(time.ToString(), hitPos.x.ToString(), hitPos.y.ToString(), hitPos.z.ToString());
            Debug.Log("Enter_" + this.gameObject.name + ": " + time + " / " + hitPos.ToString("F7"));
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("CollisionExit");
        GetComponent<Renderer>().material.color = Color.white;
    }

    private void OnApplicationQuit()
    {
        //sw.Flush(); // StreamWriterのバッファに書き出し残しがないか確認
        //sw.Close(); // アプリケーション終了時に書き込みを終了する

        sw2.Flush();
        sw2.Close();
    }
}
