using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Test : MonoBehaviour
{
    private float Timer = 0f;
    private float Limit = 1f;
    private int n = 1;
    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log("Awake");
    }
    void Start()
    {
        Debug.Log("Start");  
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        Debug.Log($"{n}:{Time.deltaTime}");
        n++;
        if (Timer > Limit) ;
        {
            Debug.Log("1 Secound");
            Timer = 0f;
        }
    }
}
