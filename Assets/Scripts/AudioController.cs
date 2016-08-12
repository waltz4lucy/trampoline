using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioController : MonoBehaviour {
    public AudioClip[] bgms;
    private int idx;

    void Update() {
        if (!GetComponent<AudioSource>().isPlaying) {
            GetComponent<AudioSource>().clip = bgms[idx % bgms.Length];
            GetComponent<AudioSource>().Play();
            idx += 1;
        }
    }
}
