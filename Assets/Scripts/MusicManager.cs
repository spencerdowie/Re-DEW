using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> playlist;
    private AudioSource audioSource;
    private Queue<AudioClip> musicQueue = new Queue<AudioClip>();
    private Coroutine musicCoroutine;
    [field: SerializeField]
    public bool Shuffle { get; private set; }
    private int currentSong = -1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        foreach (AudioClip clip in playlist)
        {
            musicQueue.Enqueue(clip);
        }

        //NextSong();
        //audioSource.Pause();
        if (audioSource.playOnAwake)
            StartCoroutine(PlayMusic());
    }

    public void StartBGM()
    {
        audioSource.Play();
        musicCoroutine = StartCoroutine(PlayMusic());
    }

    //public void NextSong()
    //{
    //    AudioClip nextSong = musicQueue.Dequeue();
    //    audioSource.clip = nextSong;
    //    musicQueue.Enqueue(nextSong);
    //}

    public void NextSong()
    {
        int lastSong = currentSong;
        int nextSong = currentSong;
        if (Shuffle)
        {
            int i = (currentSong + (playlist.Count - 1)) % playlist.Count;//curSong - 1 then wrapped around
            nextSong = Random.Range(1, playlist.Count);//Get random number of # songs - 1

            //create a gap in the possible next songs at curSong - 1
            //when it gets incremented it is at curSong
            if (nextSong == i)
                nextSong++;
        }
        currentSong = (nextSong + 1) % playlist.Count;

        if (lastSong == currentSong)
            Debug.Log("BAD");

        audioSource.clip = playlist[currentSong];
    }

    public void StopMusic()
    {
        StopCoroutine(musicCoroutine);
        audioSource.Stop();
    }

    IEnumerator PlayMusic()
    {
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

    //private void Update()
    //{
    //    if (Keyboard.current.spaceKey.wasPressedThisFrame)
    //    {
    //        NextSong();
    //    }
    //}
}
