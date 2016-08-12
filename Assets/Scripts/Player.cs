using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public bool isIntro;
    public bool isPeak;
    public bool isOffPeak;
    public bool isDead;
    public bool isUmbrella;
    private Transform myTransform;
    private Rigidbody2D myRigidbody;
    private Animator anim;
    private GameObject umbrella;
    private Animator umbrellaAnim;
    private GameObject umbrellaFx;
    private GameObject shield;
    private float speed = 15f;
    private int point;
    private int durationPoint;
    private int umbrellaQuantity = 10;
    private float lastVelocity;
    private float lastHeight;
    private AudioClip coinSound;
    private AudioClip toolboxSound;
    private AudioClip jumpSound;
    private AudioClip umbrellaSound;
    private AudioClip deathSound;

    void Start() {
        myTransform = transform;
        myRigidbody = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        umbrella = GameObject.Find("Umbrella");
        umbrellaAnim = umbrella.GetComponent<Animator>();

        umbrellaFx = Resources.Load<GameObject>("JMO Assets/Cartoon FX/CFX Prefabs/Explosions/CFX_SmokeExplosionAlt");
        umbrella.SetActive(false);

        shield = GameObject.Find("Shield");
        shield.SetActive(false);

        lastVelocity = GetVelocity().y;

        coinSound = Resources.Load<AudioClip>("Sounds/coin");
        toolboxSound = Resources.Load<AudioClip>("Sounds/toolbox");
        jumpSound = Resources.Load<AudioClip>("Sounds/jump");
        umbrellaSound = Resources.Load<AudioClip>("Sounds/umbrella");
        deathSound = Resources.Load<AudioClip>("Sounds/death");
    }

    void Update() {
        if (isDead) {
            return;
        }

        if (GetVelocity().y < 0 && lastVelocity > 0) {
            lastHeight = GetPosition().y;
            umbrella.SetActive(true);
            isPeak = true;
        } else if (GetVelocity().y > 0 && lastVelocity < 0 && !isUmbrella) {
            ClosedUmbrella();
            umbrella.SetActive(false);
            isOffPeak = true;
        } else {
            anim.SetFloat("Velocity", myRigidbody.velocity.y);
            isOffPeak = false;
            isPeak = false;
        }
        lastVelocity = GetVelocity().y;

        ShieldEffect();
    }

    void FixedUpdate() {
        if (isIntro) {
            return;
        }

        Move();
        WrapPosition();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (isDead) {
            return;
        }

        if (collider.tag == "Coin") {
            GetComponent<AudioSource>().clip = coinSound;
            GetComponent<AudioSource>().Play();
            point += 1;
            Destroy(collider.gameObject);
        } else if (collider.tag == "Toolbox") {
            GetComponent<AudioSource>().PlayOneShot(toolboxSound);
            durationPoint += 1;
            Destroy(collider.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (isDead) {
            return;
        }

        if (collision.gameObject.tag == "Ground") {
            Die();
        }
    }

    public void Move() {
        float v;
#if UNITY_EDITOR
        v = Input.GetAxis("Horizontal");
        if (Input.GetMouseButtonDown(0) && umbrellaQuantity > 0 && !isUmbrella) {
            StartCoroutine(OpenUmbrella());
        }
#else
        v = Input.acceleration.x;
        for (int i = 0; i < Input.touchCount; i++) {
            if (Input.GetTouch(i).phase == TouchPhase.Began && umbrellaQuantity > 0 && !isUmbrella) {
                StartCoroutine(OpenUmbrella());
                break;
            }
        }
#endif
        myTransform.Translate(Time.deltaTime * v * speed, 0f, 0f);
    }

    public void Jump(float jumpForce) {
        if (isDead) {
            return;
        }

        myRigidbody.velocity = new Vector2(0, jumpForce);
        GetComponent<AudioSource>().PlayOneShot(jumpSound);
    }

    public void Stop() {
        speed = 0f;
    }

    public Rigidbody2D GetRigidbody() {
        return myRigidbody;
    }

    public Vector3 GetVelocity() {
        return myRigidbody.velocity;
    }

    public Vector3 GetPosition() {
        return myTransform.position;
    }

    public float GetLastHeight() {
        return lastHeight;
    }

    public int GetPoint() {
        return point;
    }

    public int GetDurationPoint() {
        return durationPoint;
    }

    public void SetDurationPoint(int point) {
        durationPoint = point;
    }

    public int GetUmbrellaQuantity() {
        return umbrellaQuantity;
    }

    public void ClosedUmbrella() {
        // TODO Umbrella 객체 분리 예정
        myRigidbody.gravityScale = 1f;
        isUmbrella = false;
    }

    private void ShieldEffect() {
        if (lastVelocity < -25f) {
            shield.SetActive(true);
            shield.transform.localScale = -Vector3.one;
        } else {
            shield.SetActive(false);
        }
    }

    private void Die() {
        myRigidbody.velocity = new Vector2(0, 0);
        GetComponent<AudioSource>().PlayOneShot(deathSound);
        anim.SetTrigger("Hurt");
        isDead = true;
        shield.SetActive(false);
        ClosedUmbrella();
    }

    private void WrapPosition() {
        if (myTransform.position.x > 3f) {
            myTransform.position = new Vector3(-3f, myTransform.position.y, myTransform.position.z);
        } else if (myTransform.position.x < -3f) {
            myTransform.position = new Vector3(3f, myTransform.position.y, myTransform.position.z);
        }
    }

    private IEnumerator OpenUmbrella() {
        // TODO Umbrella 객체 분리 예정
        if (GetVelocity().y < 0f ) {
            isUmbrella = true;
            // umbrella.SetActive(true);
            umbrellaAnim.SetTrigger("UmbrellaOpen");
            GetComponent<AudioSource>().PlayOneShot(umbrellaSound);
            myRigidbody.velocity = new Vector2(0, Mathf.Abs(GetVelocity().y) * 0.1f);
            myRigidbody.gravityScale = 0.3f;
            umbrellaQuantity -= 1;

            GameObject fx = (GameObject) Instantiate(umbrellaFx);
            fx.transform.parent = myTransform;
            fx.transform.localPosition = Vector3.zero;
            fx.transform.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "Foreground";

            yield return new WaitForSeconds(3f);

            ClosedUmbrella();
        }
    }
}
