using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 모든 자식 GameObject를 뒤지면서 관련 이벤트를 등록한다.
public class EventRegister : MonoBehaviour {
    private AudioClip clip;

    void Start () {
        clip = (AudioClip) Resources.Load("Sounds/UI/UI_button_normal");
        Register(transform);
    }

    void Register(Transform transform) {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++) {
            Transform t = transform.GetChild(i);
            GameObject g = t.gameObject;

            Collider collider = g.GetComponent<Collider>();
            if (collider != null) {
                g.AddComponent<ButtonEventComponent>();
                // RegisterAudio(g);
                // RegisterScale(g);
            }

            Register(t);
        }
    }

    void RegisterAudio(GameObject o) {
        UIPlaySound sound = o.GetComponent<UIPlaySound>();
        if (null != sound && null == sound.audioClip) {
            sound.audioClip = clip;
        } else {
            sound = o.AddComponent<UIPlaySound>();
            sound.audioClip = clip;
        }
    }

    void RegisterScale(GameObject o) {
        UIButtonScale scale = o.GetComponent<UIButtonScale>();
        if (null != scale) {
            scale.pressed = new Vector3(1.1f, 1.1f, 1.1f);
        } else {
            scale = o.AddComponent<UIButtonScale>();
            scale.pressed = new Vector3(1.1f, 1.1f, 1.1f);
        }
    }
}
