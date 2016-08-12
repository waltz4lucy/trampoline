using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntroController : MonoBehaviour {
    void Start() {
        CreateBackgrounds();

        GameObject.Find("6. HighScoreLabel").GetComponent<UILabel>().text = "High Score\n" + PlayerPrefs.GetInt("HighScore");
        NotificationCenter center = NotificationCenter.Instance;
        center.AddObserver("ResetButton", this, "OnReset");
    }

    void OnDestroy() {
        NotificationCenter.Instance.Clear();
    }

    private void CreateBackgrounds() {
        string[] themes = { "Theme_ Castle", "Theme_ Grass"};
        int idx = Random.Range(0, themes.Length);

        GameObject background = (GameObject) Instantiate(Resources.Load("Prefabs/Theme/" + themes[idx]));
        background.transform.parent = GameObject.Find("BG").transform;

        for (int i = 0; i < 40; i++) {
            GameObject cloud = (GameObject) Instantiate(Resources.Load("Prefabs/Cloud" + Random.Range(1, 4)));
            cloud.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, (40f - i) / 40f);
            cloud.transform.parent = GameObject.Find("BG").transform;
            cloud.transform.position = new Vector3(Random.Range(-3, 3), (3f * i) + 8f, 0f);
        }

        for (int i = 40; i < 80; i++) {
            GameObject star = (GameObject) Instantiate(Resources.Load("Prefabs/Star" + Random.Range(1, 4)));
            star.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, (i - 40f) / 40f);
            star.transform.parent = GameObject.Find("BG").transform;
            star.transform.position = new Vector3(Random.Range(-3, 3), (3f * i) + 8f, 0f);
        }
    }

    // UI Events
    public void OnReset(object sender, Dictionary<string, object> userdata) {
        Application.LoadLevel("Main");
    }
}
