using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wheel : MonoBehaviour
{
    [SerializeField] private GameObject wheel;

    // Update is called once per frame
    void Update()
    {
        if (wheel != null)
            wheel.transform.Rotate(0f, 0f, 0.1f);
    }
}


