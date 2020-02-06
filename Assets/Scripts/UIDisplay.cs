using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplay : MonoBehaviour
{
    private string[] StartSequence = {"3", "2", "1", "Go",""};
    public Text UIText;
    public Text Score;
    int index = 0;
    int timer = 0;
    public bool isCounting = false;
    bool isFinished = false;
    MainManager Manager;

    void Start()
    {
        Manager = GetComponent<MainManager>();
        float bestTime = PlayerPrefs.GetFloat("Time");
        if (bestTime == 0f)
            Score.text = "Grab the Blaster to Start";
        else
            Score.text = "Best: " + bestTime.ToString("0.00") + " Seconds";
    }

    public void PlayStart() {
        if(!isCounting)
            InvokeRepeating("StartUI", 0f, 1f);
    }

    public void PlayFinish() {
        if (isFinished)
            return;
        UIText.text = "Game!";
        timer = 0;
        if (MainManager.isRecord)
            Score.text = "New Record: ";
        else
            Score.text = "Completed: ";
        Score.text += MainManager.Timer.ToString("0.00") + " Seconds" +
            "\nPress A to Play Again";
        isFinished = true;
    }

    void StartUI() {
        UIText.text = StartSequence[index++];
        UIText.fontSize = 0;
        timer = 0;
        Score.text = "";
        isCounting = true;
        if (index == StartSequence.Length) {
            isCounting = false;
            CancelInvoke();
            MainManager.GameStarted = true;
        }
    }

    void Update()
    {
        if (isCounting)
        {
            UIText.fontSize = 3 * (50 - Mathf.Abs(timer++ - 50));
            UIText.enabled = UIText.fontSize > 10;
        }
        if (isFinished) {
            UIText.fontSize = (int) Mathf.Lerp((float)UIText.fontSize, 60f, 1f);
            Score.fontSize = (int) Mathf.Lerp((float)Score.fontSize, 30f, 1f);
        }
    }
}
