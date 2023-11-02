using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public void toRed() {
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }

    public void toWhite() {
        gameObject.GetComponent<Renderer>().material.color = Color.white;
    }
}