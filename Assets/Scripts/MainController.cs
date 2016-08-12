using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainController : MonoBehaviour {
    private GameObject[] coinTemplete = new GameObject[4];
    private GameObject[] coins = new GameObject[5];
    private GameObject toolbox;
    private GameObject trampolinePositionGuide;
    private Player player;
    private Trampoline trampoline;
    private GameObject resetButton;
    private GameObject hiddenButton;
    private UILabel durationLabel;
    private UILabel coinLabel;
    private UILabel umbrellaLabel;
    // private UILabel distanceLabel;
    private UILabel messageLabel;
    private UILabel highscoreLabel;
    private UISlider feverSlider;
    private Camera mainCamera;

    void Start() {
        trampolinePositionGuide = GameObject.Find("TrampolineGuide");
        trampolinePositionGuide.SetActive(false);

        resetButton = GameObject.Find("ResetButton");
        resetButton.SetActive(false);

        hiddenButton = GameObject.Find("HiddenButton");
        hiddenButton.SetActive(false);

        player = GameObject.Find("Player").GetComponent<Player>();
        trampoline = GameObject.Find("Trampoline").GetComponent<Trampoline>();

        durationLabel = GameObject.Find("1. DurationLabel").GetComponent<UILabel>();
        durationLabel.text = trampoline.GetDuration().ToString();

        coinLabel = GameObject.Find("2. CoinLabel").GetComponent<UILabel>();
        coinLabel.text = player.GetPoint().ToString();

        umbrellaLabel = GameObject.Find("3. UmbrellaLabel").GetComponent<UILabel>();
        umbrellaLabel.text = player.GetUmbrellaQuantity().ToString();

        // distanceLabel = GameObject.Find("4. DistanceLabel").GetComponent<UILabel>();
        // distanceLabel.text = player.GetPosition().y.ToString();

        messageLabel = GameObject.Find("5. MessageLabel").GetComponent<UILabel>();
        messageLabel.text = "";

        highscoreLabel = GameObject.Find("6. HighScoreLabel").GetComponent<UILabel>();
        highscoreLabel.text = "";

        feverSlider = GameObject.Find("FeverSlider").GetComponent<UISlider>();

        mainCamera = Camera.main;

        InitCoinTemplete();
        for (int i = 0; i < coins.Length; i++) CreateCoins(i, (i + 2) * 5f);
        CreateBackgrounds();

        NotificationCenter center = NotificationCenter.Instance;
        center.AddObserver("ResetButton", this, "OnReset");
        center.AddObserver("HiddenButton", this, "OnBack");
    }

    void Update() {
        if (player.isDead) {
            GameOver();

            return;
        }

        int durationPoint = player.GetDurationPoint();
        int duration = trampoline.GetDuration();
        if (durationPoint > 0) {
            trampoline.SetDuration(duration + durationPoint);
            player.SetDurationPoint(0);
        }

        durationLabel.text = duration.ToString();
        coinLabel.text = player.GetPoint().ToString();
        umbrellaLabel.text = player.GetUmbrellaQuantity().ToString();
        // distanceLabel.text = Mathf.Floor(player.GetPosition().y).ToString();
        float value = (trampoline.GetFeverCount() % trampoline.GetFeverPeriod()) * 0.1f;
        feverSlider.value = Mathf.Lerp(feverSlider.value, value, 0.1f);

        if (trampoline.GetDuration() == 0) {
            messageLabel.text = "Last";
        } else if (trampoline.isFever) {
            messageLabel.text = "Fever";
        } else {
            messageLabel.text = "";
        }

        TraceTrampolineGuide();
        ManageBackgrounds();
        ManageToolbox();
        ManageCoins();
    }

    void FixedUpdate() {
        MoveCamera();
    }

    void OnDestroy() {
        NotificationCenter.Instance.Clear();
    }

    private void MoveCamera() {
        float yPos = player.GetPosition().y;
        float yVel = player.GetVelocity().y;
        float ySpan = 4f; // 카메라 대비 캐릭터 최대, 최소 높이 값
        float wrapSpan = 4f; // 카메라 최소 높이
        float followSpeed = 80f; // 카메라 스무스 이동 속도 보정 값

        if (yVel > 0f) {
            mainCamera.transform.position = Vector3.Slerp(mainCamera.transform.position, new Vector3(mainCamera.transform.position.x,  yPos + ySpan, mainCamera.transform.position.z), Mathf.Abs(yVel) / followSpeed);
        } else {
            mainCamera.transform.position = Vector3.Slerp(mainCamera.transform.position, new Vector3(mainCamera.transform.position.x,  Mathf.Max(yPos - ySpan, wrapSpan), mainCamera.transform.position.z), Mathf.Abs(yVel) / followSpeed);
        }
    }

    private void TraceTrampolineGuide() {
        if (!trampoline) {
            return;
        }

        float scale = 130f; // 비율 구하는 공식 뽑아야함
        float xPos = trampoline.GetPosition().x * scale;

        if (mainCamera.transform.position.y > 6f) {
            trampolinePositionGuide.transform.localPosition = new Vector3(xPos, trampolinePositionGuide.transform.localPosition.y, trampolinePositionGuide.transform.localPosition.z);
            trampolinePositionGuide.SetActive(true);
        } else {
            trampolinePositionGuide.SetActive(false);
        }
    }

    private void InitCoinTemplete() {
        for (int i = 0; i < coinTemplete.Length; i++) {
            GameObject templete = Resources.Load<GameObject>("Prefabs/CoinTemplete" + i);
            coinTemplete[i] = templete;
        }
    }

    private void CreateCoins(int idx, float position) {
        Destroy(coins[idx]);

        GameObject coin;
        if (trampoline.isFever) {
            coin = (GameObject) Instantiate(coinTemplete[0]);
        } else {
            coin = (GameObject) Instantiate(coinTemplete[Random.Range(1, 4)]);
        }
        coin.transform.parent = GameObject.Find("Coins").transform;
        coin.transform.position = new Vector3(Random.Range(-2, 2), position, 0f);
        if (trampoline.isFever) coin.transform.position = new Vector3(0f, position, 0f);
        coins[idx] = coin;
    }

    private void ManageCoins() {
        if (player.isDead) {
            return;
        }

        // TODO 코인 풀매니저 필요
        float yPos = player.GetPosition().y;
        float yVel = player.GetVelocity().y;
        float coinTempleteHeight = 5f;

        for (int i = 0; i < coins.Length; i++) {
            float cPos = coins[i].transform.position.y;
            float distance = cPos - yPos;
            if (player.isOffPeak) {
                CreateCoins(i, (i + 2) * 5f);
            } else if (distance < coinTempleteHeight * -2f && yVel > 0f && yPos > coins.Length * 4) {
                CreateCoins(i, coinTempleteHeight * coins.Length + cPos);
            } else if (distance > coinTempleteHeight * 2f && yVel < 0f && yPos > coins.Length * 4) {
                CreateCoins(i, coinTempleteHeight * -coins.Length + cPos);
            }
        }
    }

    private void CreateToolbox(float position) {
        Destroy(toolbox);

        toolbox = (GameObject) Instantiate(Resources.Load<GameObject>("Prefabs/ToolBoxTemplete1"));
        toolbox.transform.parent = GameObject.Find("Coins").transform;
        toolbox.transform.position = new Vector3(0f, position, 0f);
    }

    private void ManageToolbox() {
        if (player.isDead) {
            return;
        }

        float rand = Random.Range(0, 10);
        float lastHeight = player.GetLastHeight();

        if (player.isOffPeak && rand > 7f) CreateToolbox(Mathf.Max(30f, lastHeight) * 0.8f);
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

    private void ManageBackgrounds() {
        // Change Sky Color
        Color colorFrom = new Color(216f / 255f, 247f / 255f, 247f / 255f, 255f / 255f);
        Color colorTo = new Color(16f / 255f, 77f / 255f, 144f / 255f, 255f / 255f);
        float yPos = player.GetPosition().y;
        float damping = 0.01f;
        mainCamera.backgroundColor = Color.Lerp(colorFrom, colorTo, Mathf.Max(0, yPos - 10f) * damping);

        // TODO Change Sky Objects
    }

    private void GameOver() {
        player.Stop();
        trampoline.Stop();
        resetButton.SetActive(true);
        hiddenButton.SetActive(true);
        messageLabel.text = "Game\nOver";

        float saturation = mainCamera.GetComponent<ColorCorrectionCurves>().saturation;
        if (saturation > 0f) mainCamera.GetComponent<ColorCorrectionCurves>().saturation -= 0.01f;

        float pitch = GameObject.Find("AudioPlayer").GetComponent<AudioSource>().pitch;
        if (pitch > 0f) GameObject.Find("AudioPlayer").GetComponent<AudioSource>().pitch -= 0.01f;

        int highScore = PlayerPrefs.GetInt("HighScore");
        int score = player.GetPoint();
        if (score > highScore) {
            PlayerPrefs.SetInt("HighScore", score);
            highscoreLabel.text = "High Score\n" + score;
        } else {
            highscoreLabel.text = "High Score\n" + highScore;
        }
    }

    // UI Events
    public void OnReset(object sender, Dictionary<string, object> userdata) {
        Application.LoadLevel("Main");
    }

    public void OnBack(object sender, Dictionary<string, object> userdata) {
        Application.LoadLevel("Intro");
    }
}
