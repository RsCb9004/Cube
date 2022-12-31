using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GamePause: MonoBehaviour {

    public static bool Pausable { get; set; }
    public static bool Paused { get; private set; }

    public Text title;
    public Text tip;

    public VideoPlayer video;

    private Canvas canvas;
    private Animator anim;

    private void Start() {

        title.text = Lang.Uage.play.pauseTitle;
        tip.text = string.Format(Lang.Uage.play.pauseTip, "D", "F", "Esc");

        canvas = GetComponent<Canvas>();
        anim = GetComponent<Animator>();
        Paused = canvas.enabled = false;
    }

    private void Update() {
        if(Paused) {
            if(Input.GetButtonDown("Pause")) {
                Time.timeScale = 1;
                if(video.isPaused) video.Play();
                Paused = canvas.enabled = false;
            }
            if(Input.GetButtonDown("OK")) {
                Time.timeScale = 1;
                Loader.LoadScene("MainPage");
            }
            if(Input.GetButtonDown("Cancel")) {
                Time.timeScale = 1;
                Loader.LoadScene("Play");
            }
        }
        else if(Pausable) {
            if(Input.GetButtonDown("Pause")) {
                Time.timeScale = 0;
                if(video.isPlaying) video.Pause();
                Paused = canvas.enabled = true;
                anim.SetTrigger("Pause");
            }
        }
    }
}
