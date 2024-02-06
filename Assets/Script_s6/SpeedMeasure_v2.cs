using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Oculus.Interaction.Body.Input;
using UnityEngine;

// 棒を振る速さを計測して、データに書き出す。
// 本実験用。v2

public class SpeedMeasure_v2 : MonoBehaviour
{
    [SerializeField] private Transform TrackingSpace; // コントローラの位置取得用
    public GameObject collisionPointSphere; // 衝突位置確認用の球

    // 時間
    public float time;
    public float deltaTime;
    DateTime dt;

    /*** ファイル読み書き ***/
    private StreamWriter sw2; // 衝突位置、時間データ用
    private string filePath1;
    private string filePath2;

    // 前回の位置（コントローラ）
    private Vector3 previousPos = Vector3.zero;
    // 前回の位置（先端の点）
    private Vector3 previousTopPos = Vector3.zero;
    // 前回の回転
    private Quaternion previousRot = Quaternion.identity;
    // 角速度ベクトル
    private Vector3 angularVelocityVector = Vector3.zero;

    // 動かす方法を変える
    public Rigidbody rb;
    private Vector3 shift_hand = new Vector3(0.0f, 0.25f, 0.25f); // 持つ位置のオフセット
    public Vector3 handPos = Vector3.zero; // コントローラの位置
    private Quaternion shift_quat  = Quaternion.Euler(45.0f, 0.0f, 0.0f); // 持つ角度のオフセット


    /*** Pythonの計算で使う / 棒の上の点の位置をビジュアルで確認する用 ***/
    // 棒の先端の点の初期位置（オフセット）
    private Vector3 shift_top = new Vector3(0.0f, 0.60355f, 0.60355f);
    // 棒の根元の点の初期位置（オフセット）
    private Vector3 shift_bottom = new Vector3(0.0f, -0.10355f, -0.10355f);

    public Vector3 P_bottom = Vector3.zero; // 棒の根元の点
    public Vector3 P_top = Vector3.zero; // 棒の先端の点

    // コントローラーの前方を表すベクトル
    public Vector3 forward_initial = new Vector3(0.0f, -1.0f, 1.0f); // 初期位置（オフセット）
    public Vector3 forward = Vector3.zero; // 書き出すベクトル


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        dt = DateTime.Now;
        String dtString = dt.ToString("yyyyMMdd-HHmmss");

        // ファイル作成
        filePath1 = @"OutputData\SaveData-" + dtString + ".csv";
        filePath2 = @"OutputData\SaveData-" + dtString + "-collision.csv";
        string[] s1 = { "forward_x", "forward_y", "forward_z",
                        "P_bottom_x", "P_bottom_y", "P_bottom_z", "P_top_x", "P_top_y", "P_top_z",
                        "time", "pos_x", "pos_y", "pos_z", "rot_x", "rot_y", "rot_z",
                        "quaternion_w","quaternion_x", "quaternion_y", "quaternion_z",
                        "speed_p_top", "rad/s", "speed",
                        "velocity_x", "velocity_y", "velocity_z", "speed_ovr",
                        "angularVelocity_x", "angularVelocity_y", "angularVelocity_z"
                        };
        string s2 = string.Join(",", s1);
        System.IO.File.AppendAllText(filePath1, s2 + Environment.NewLine);
        sw2 = new StreamWriter(filePath2, false, Encoding.UTF8);
        string[] s3 = { "collisionTime", "collisionPos_x", "collisionPos_y", "collisionPos_z" };
        string s4 = string.Join(",", s3);
        sw2.WriteLine(s4);
    }


    void FixedUpdate()
    {
        deltaTime = Time.deltaTime;
        time += deltaTime;

        // コントローラーの角度を取得
        Vector3 localRot_euler = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles;
        Vector3 rrot_euler = TrackingSpace.TransformDirection(localRot_euler);

        /*** 角速度を計算 ***/
        // 現在のクォータニオン
        // この部分は transform.rotation で求めても同じ
        Quaternion quaternion = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        // 回転の変化量を計算
        Quaternion deltaRot = Quaternion.Inverse(previousRot) * quaternion;

        // クォータニオンを角度と回転軸に変換
        // angle は、[deg/flame] (オイラー角/1フレーム)   
        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
        // deg/s を求める
        // float angularSpeed_euler = angle / deltaTime;
        // 角速度 [rad/s] を計算
        float angularSpeed = (angle * Mathf.Deg2Rad) / deltaTime;

        // 前回の角度を更新
        previousRot = quaternion;

        //rb.MoveRotation(rb.rotation * deltaRot);
        rb.MoveRotation(quaternion * shift_quat);


        // コントローラーの位置を取得
        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Vector3 rpos = TrackingSpace.TransformPoint(localPos);
        Debug.Log("localPos: " + rpos.ToString("F7"));
        Debug.Log("rpos: " + rpos.ToString("F7"));


        // handPos = rpos + rb.rotation * deltaRot * shift_hand;
        handPos = rpos + quaternion * shift_hand;
        rb.MovePosition(handPos);

        Vector3 velocity = TrackingSpace.TransformPoint(OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch));
        Vector3 angularVelocity = TrackingSpace.TransformPoint(OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch));
        Debug.Log("localVelocity: " + OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch).ToString("F7"));
        Debug.Log("velocity: " + velocity.ToString("F7"));

        float speed_ovr = velocity.magnitude;

        System.Threading.Thread thread = new System.Threading.Thread(() =>
        {
            
            /*** 速度を計算 ***/
            /* 棒の先端の点の速度を計算する */
            // 棒の先端の点を求める
            P_top = rpos + quaternion * shift_top;
            // 前回の位置と今回の位置の差を経過時間で割って、速度を求めている。
            float speed_p_top = Vector3.Distance(previousTopPos, P_top) / deltaTime;
            //Debug.Log("Time: " + Time.deltaTime);
            // コントローラの速度
            float speed = Vector3.Distance(previousPos, rpos) / deltaTime;

            // 前回の位置を更新
            previousPos = rpos;
            previousTopPos = P_top;
            // Debug.Log("Speed: " + String.Format("{0:0.00}", speed));

            // 棒の根元の点を計算
            P_bottom = rpos + quaternion * shift_bottom;

            // 棒の前方ベクトルを計算
            // forward = rpos + rot * forward_initial; // 前方ベクトル確認用の球の位置
            forward = quaternion * forward_initial;

            // ファイル書き込み
            SaveData1(filePath1,
                        forward.x.ToString(), forward.y.ToString(), forward.z.ToString(),
                        P_bottom.x.ToString(), P_bottom.y.ToString(), P_bottom.z.ToString(),
                        P_top.x.ToString(), P_top.y.ToString(), P_top.z.ToString(),
                        time.ToString(), rpos.x.ToString(), rpos.y.ToString(), rpos.z.ToString(),
                        rrot_euler.x.ToString(), rrot_euler.y.ToString(), rrot_euler.z.ToString(),
                        quaternion.w.ToString(), quaternion.x.ToString(), quaternion.y.ToString(), quaternion.z.ToString(),
                        speed_p_top.ToString(), angularSpeed.ToString(), speed.ToString(),
                        velocity.x.ToString(), velocity.y.ToString(), velocity.z.ToString(), speed_ovr.ToString(),
                        angularVelocity.x.ToString(), angularVelocity.y.ToString(), angularVelocity.z.ToString()
                        );
        });
        thread.Start();
    }

    // ファイル書き込み関数
    public void SaveData1(string filePath, string txt1, string txt2, string txt3, string txt4, string txt5, string txt6, string txt7, string txt8,
                        string txt9, string txt10, string txt11, string txt12, string txt13, string txt14, string txt15, string txt16, string txt17,
                        string txt18, string txt19, string txt20, string txt21, string txt22, 
                        string txt23, string txt24, string txt25, string txt26, string txt27, string txt28, string txt29, string txt30
                        )
    {
        string[] s1 = { txt1, txt2, txt3, txt4, txt5, txt6, txt7, txt8, txt9, txt10, txt11, txt12, txt13, txt14, txt15, txt16, txt17, txt18, txt19, txt20, txt21, txt22,
                        txt23, txt24, txt25, txt26, txt27, txt28, txt29, txt30
                         };
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
        //GetComponent<Renderer>().material.color = Color.red;

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
        sw2.Flush(); // StreamWriterのバッファに書き出し残しがないか確認
        sw2.Close(); // アプリケーション終了時に書き込みを終了する
    }
}
