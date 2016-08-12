using UnityEngine;
using System.Collections;

public class DeviceControl : MonoBehaviour {
    private const float W_16_BY_9 = 56.25f;

    void Start () {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if !UNITY_EDITOR
        // 가로 크기를 16*9 스크린 기준으로 맞춤
        Resolution resolution = Screen.currentResolution;
        float h = 100.0f/resolution.height;
        float w = resolution.width * h;
        float scale = W_16_BY_9/w;

        Camera camera = Camera.main;
        camera.orthographicSize *= scale;
#endif
    }
}
