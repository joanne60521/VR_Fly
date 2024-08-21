using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class move : MonoBehaviour
{
    public Transform CameraPosition;

    public float currentValue = 0.0f;
    public float lastCheckedValue = 0.0f;
    public float timer = 0.0f;
    public float checkInterval = 0.5f;

    void Start()
    {
        
    }

    void Update()
    {
        currentValue = CameraPosition.position.y;

        // 每隔一秒檢查一次變化
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0.0f;
            CheckValueChange();
        }
    }


// 偵測高度有正負改變的瞬間，之後改成手把速度
    void CheckValueChange()
    {
        if (currentValue * lastCheckedValue < 0) {
            Debug.Log("Turned.");
        }
        lastCheckedValue = currentValue;

    }
}
