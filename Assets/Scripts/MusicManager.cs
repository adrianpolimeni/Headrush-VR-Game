using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    AudioSource[] tracks;
    Player player;
    void Start()
    {
        tracks = GetComponents<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    }

    void Update()
    {
        if (MainManager.GameStarted)
            FadeTrack(tracks[2], 1f, 2f);
        if (player.isMoving)
            FadeTrack(tracks[1], 1f, 0.3f);
        if (MainManager.UI.isCounting)
            FadeTrack(tracks[3], 1f, 2f);
        float progress = player.transform.position.z / player.FinishLine;
        if (progress > 1f)
        {
            FadeTrack(tracks[3], 0f, 1f);
            FadeTrack(tracks[4], 0f, 1f);
            FadeTrack(tracks[1], 0f, 1f);
            FadeTrack(tracks[5], 0.8f, 2f);
            tracks[6].enabled = true;
        }
        else {
            FadeTrack(tracks[4], progress*0.8f, 0.3f);
            FadeTrack(tracks[5], progress*0.8f, 0.3f);
            tracks[6].enabled = false;
        }
    }

    void FadeTrack(AudioSource track, float fadeTo, float seconds) {
        track.volume = Mathf.Lerp(track.volume, fadeTo, Time.deltaTime/seconds);
    }
}
