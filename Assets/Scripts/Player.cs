using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    float totalDistance = 1;
    public Vector3 TargetMoveTo;
    public float Speed = 10;
    public bool isMoving = false;
    public float FinishLine = 52.5f;
    public bool isAlive = true;
    public int Health = 100;
    bool restarting = false;
    public PostProcessVolume postProcess;
    AudioSource audioSource;
    public AudioClip[] effectClips;

    void Start()
    {
        TargetMoveTo = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        var currentPosition = transform.position;
        currentPosition.y = TargetMoveTo.y;
        var distance = Vector3.Distance(currentPosition, TargetMoveTo);
        transform.position = Vector3.MoveTowards(currentPosition, TargetMoveTo, Speed * totalDistance * Time.deltaTime);
        isMoving = distance > 0.01f;
        PostProcess();
        if (!isAlive && !restarting) {
            restarting = true;
            StartCoroutine(Restart(2f));
        }
    }

    IEnumerator Restart(float time)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);
        asyncLoad.allowSceneActivation = false;

        yield return new WaitForSeconds(time);

        asyncLoad.allowSceneActivation = true;
    }
    void PostProcess() {
        DepthOfField depthOfField;
        if (postProcess.profile.TryGetSettings(out depthOfField))
        {
            depthOfField.enabled.value = isMoving;
        }
        ColorGrading colorGrading;
        if (postProcess.profile.TryGetSettings(out colorGrading))
        {
            if (!isAlive)
                colorGrading.brightness.value = Mathf.Lerp(colorGrading.brightness.value, -100f, 0.01f);
            colorGrading.colorFilter.value.g = Mathf.Lerp(colorGrading.colorFilter.value.g, (float)Health/100f, 0.1f);
            colorGrading.colorFilter.value.b = Mathf.Lerp(colorGrading.colorFilter.value.b, (float)Health/100f, 0.1f);
        }
    }

    public void MoveTo(Vector3 pos)
    {
        TargetMoveTo = new Vector3(pos.x, TargetMoveTo.y, pos.z);
        totalDistance = Vector3.Distance(transform.position, TargetMoveTo);

        if (pos.z > FinishLine) {
            float time = PlayerPrefs.GetFloat("Time");
            if (MainManager.Timer < time || time == 0f)
            {
                PlayerPrefs.SetFloat("Time", MainManager.Timer);
                MainManager.isRecord = true;
            }
            MainManager.GameFinished = true;
            MainManager.UI.PlayFinish();
        }
    }

    public void OnShot() {
        if(!isMoving)
            Health -= 10;
        isAlive = Health > 0;
    }

    public void TriggerRestart() {
        if (restarting)
            return;
        restarting = true;
        isAlive = false;
        StartCoroutine(Restart(2f));
    }

    public void PlaySound(int i) {
        audioSource.PlayOneShot(effectClips[i]);
    }
}
