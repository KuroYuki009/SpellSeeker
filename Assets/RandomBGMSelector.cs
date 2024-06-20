using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBGMSelector : MonoBehaviour
{
    AudioSource audioSource;

    public List<AudioClip> bgmClipList;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        RandomBGM_Start();
    }

    public void RandomBGM_Start()
    {
        int i = Random.Range(0, bgmClipList.Count);
        audioSource.clip = bgmClipList[i];
        audioSource.loop = true;
        audioSource.Play();
    }
}
