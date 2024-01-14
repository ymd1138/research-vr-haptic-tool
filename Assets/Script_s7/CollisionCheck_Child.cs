using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionCheck_Child : MonoBehaviour
{
    private float time;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Wall")
        {
            Vector3 collisionPosition = collision.contacts[0].point;
            Debug.Log("Enter_" + this.gameObject.name + ": " + time + " / " + collisionPosition.ToString("F7"));
        }
    }
}
