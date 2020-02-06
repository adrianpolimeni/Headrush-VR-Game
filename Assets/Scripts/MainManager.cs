using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static GameObject Player;
    public static UIDisplay UI;
    Spawner Spawner;
    public static float Timer;
    public static bool GameStarted;
    public static bool GameFinished;
    public static bool isRecord;
    PlayerPrefs playerPrefs;

    void Awake()
    {
        Timer = 0f;
        GameStarted = false;
        GameFinished = false;
        isRecord = false;
        Player = GameObject.FindGameObjectWithTag("Player");
        UI = GetComponent<UIDisplay>();
    }

    void Update()
    {
        if (GameStarted && !GameFinished) {
            Timer += Time.deltaTime;
        }
    }
}
