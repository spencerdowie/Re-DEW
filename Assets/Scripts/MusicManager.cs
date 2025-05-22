using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> playlist;
    [SerializeField]
    private AudioSource audioSource;
    private Queue<AudioClip> musicQueue = new Queue<AudioClip>();
    private Coroutine musicCoroutine;

    private void Awake()
    {
        foreach (AudioClip clip in playlist)
        {
            musicQueue.Enqueue(clip);
        }

        NextSong();
        audioSource.Pause();
    }

    public void StartBGM()
    {
        musicCoroutine = StartCoroutine(PlayMusic());
    }

    public void NextSong()
    {
        AudioClip nextSong = musicQueue.Dequeue();
        audioSource.clip = nextSong;
        musicQueue.Enqueue(nextSong);
    }

    public void StopMusic()
    {
        StopCoroutine(musicCoroutine);
        audioSource.Stop();
    }

    IEnumerator PlayMusic()
    {
        audioSource.Play();

        while (musicQueue.Count > 0)
        {
            if (!audioSource.isPlaying && !AudioListener.pause)
            {
                NextSong();
                audioSource.Play();
            }
            yield return null;
        }
    }
}
