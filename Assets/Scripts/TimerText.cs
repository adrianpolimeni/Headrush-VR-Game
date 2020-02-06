using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerText : MonoBehaviour
{
    Text timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = GetComponentsInChildren<Text>()[0];

    }

    // Update is called once per frame
    void Update()
    {
        timer.text = MainManager.Timer.ToString("0.00");
    }
}
