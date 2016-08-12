using UnityEngine;
using System.Collections;

public class Trampoline : MonoBehaviour {
    public bool isIntro;
    public bool isFever;
    private Transform myTransform;
    private Animator anim;
    private GameObject spring;
    private Transform springTransform;
    private Animator springAnim;
    private float jumpForce = 20f;
    private float speed = 0.01f;
    private float direction = 1f;
    private int feverPeriod = 10;
    private int duration = 30;
    private int feverCount;

    void Start () {
        myTransform = transform;
        anim = GetComponent<Animator>();

        spring = GameObject.Find("Spring");
        springTransform = spring.transform;
        springAnim = spring.GetComponent<Animator>();

        if (isIntro) jumpForce = 10f;
    }

    void FixedUpdate() {
        if (!isIntro) Move();
    }

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.tag == "Player") {
            if (!isIntro) {
                jumpForce += 1f;
                feverCount += 1;
                duration -= 1;
                if (myTransform.localScale.x > 0.5f) myTransform.localScale -= new Vector3(0.005f, 0, 0);
                if (speed < 0.25f) speed += 0.0025f;
                CheckFever();
                Broken();
            }

            GameObject o = collider.gameObject;
            o.GetComponent<Rigidbody2D>().velocity = new Vector2(0, jumpForce);
            Player player = o.GetComponent<Player>();
            player.Jump(jumpForce);
            anim.SetTrigger("SpringDown");
            springAnim.SetTrigger("SpringDown");
        }
    }

    public void Move() {
        if (Mathf.Abs(myTransform.position.x) > 2.8f) direction = -direction;
        myTransform.Translate(speed * direction, 0f, 0f);
        springTransform.Translate(speed * direction, 0f, 0f);
    }

    public void Stop() {
        speed = 0f;
    }

    public Vector3 GetPosition() {
        return myTransform.position;
    }

    public int GetDuration() {
        return duration;
    }

    public void SetDuration(int point) {
        duration = point;
    }

    public int GetFeverCount() {
        return feverCount;
    }

    public int GetFeverPeriod() {
        return feverPeriod;
    }

    private void CheckFever() {
        if (feverCount % feverPeriod == 0 || duration == 0) isFever = true;
        else isFever = false;
    }

    private void Broken() {
        if (duration <= 0) {
            Destroy(gameObject);
            Destroy(spring);
        }
    }
}
