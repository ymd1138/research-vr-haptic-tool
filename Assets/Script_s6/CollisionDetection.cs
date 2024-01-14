using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    SpeedMeasure sm;

    void Start()
    {
        sm = GameObject.Find("Stick").GetComponent<SpeedMeasure>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Cube")
        {
            Vector3 collisionPosition = collision.contacts[0].point;
            Debug.Log("Enter_" + this.gameObject.name + ": " + sm.time + " / " + collisionPosition.ToString("F7"));
        }
    }
}
