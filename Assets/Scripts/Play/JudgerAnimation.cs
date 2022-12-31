using System.Collections;
using UnityEngine;

public class JudgerAnimation: MonoBehaviour {

    const float Smoother = 0.7f;
    private static readonly float[] rotations = {
        0, -90, -180, -270
    };

    public Animator[] anims;
    private Animation anim;

    public GameObject strike;
    public AudioClip hitSound;

    public Transform focus;

    private void Start() {
        StrikeFX.Init();
        anim = GetComponent<Animation>();
    }

    private void Update() {
        if(Judger.Focus == -1) focus.gameObject.SetActive(false);
        else {
            focus.gameObject.SetActive(true);
            focus.rotation = Charter.Rot2Quat(rotations[Judger.Focus]);
        }
    }

    public IEnumerator Move(Vector2 position, float time) {
        Vector2 curVel = Vector2.zero;
        while(time > 0) {
            float deltaTime = Charter.Sec2Beat(Time.deltaTime);
            time -= deltaTime;
            transform.position = Vector2.SmoothDamp(
                transform.position, position, ref curVel,
                time * Smoother, int.MaxValue, deltaTime
            );
            yield return null;
        }
        transform.position = position;
    }

    public IEnumerator Rotate(float rotation, float time) {
        float curVel = 0;
        while(time > 0) {
            float deltaTime = Charter.Sec2Beat(Time.deltaTime);
            time -= deltaTime;
            transform.rotation = Charter.Rot2Quat(Mathf.SmoothDamp(
                transform.rotation.eulerAngles.z, rotation, ref curVel,
                time * Smoother, int.MaxValue, deltaTime
            ));
            yield return null;
        }
        transform.rotation = Charter.Rot2Quat(rotation);
    }

    public void Highlight(int direction) => anims[direction].SetTrigger("Hit");

    public void Unlight(int direction) => anims[direction].SetTrigger("Unhit");

    public void Strike(int direction, bool mute = false) {

        StrikeFX effect = Instantiate(strike, transform).GetComponent<StrikeFX>();
        effect.SetSortLayer(direction);
        effect.transform.localPosition = Charter.DistancePerBeat / 2 * NoteBehaviours.Vectors[direction];

        if(!mute) AudioSource.PlayClipAtPoint(hitSound, Vector3.zero);
    }

    public IEnumerator End() {
        yield return new WaitForSeconds(Charter.MoveBackTime);
        anim.Play();
    }
}
