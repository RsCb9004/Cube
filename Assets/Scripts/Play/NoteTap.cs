using UnityEngine;

public class NoteTap: Note {

    public Sprite[] sprites;

    protected bool inQueue = false;

    protected virtual void Start() {

        GetComponent<SpriteRenderer>().sprite = sprites[note.style];
        GetPosition = NoteBehaviours.GetNotePositions[note.style];

        Update();
    }

    protected virtual void Update() {

        transform.localPosition =
            GetPosition(note.direction, Charter.GetTime(note.moment, note.speed));

        if(!inQueue && Charter.GetTime(note.moment) < Charter.Sec2Beat(Charter.Setting.goodRange)) {
            Judger.TapQueue[note.direction].Enqueue(this);
            inQueue = true;
        }
    }
}
