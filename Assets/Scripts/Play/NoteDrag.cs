using UnityEngine;

public class NoteDrag: Note {

    public Sprite[] sprites;

    private bool inQueue = false;

    private void Start() {

        GetComponent<SpriteRenderer>().sprite = sprites[note.style];
        GetPosition = NoteBehaviours.GetNotePositions[note.style];

        Update();
    }

    private void Update() {

        transform.localPosition =
            GetPosition(note.direction, Charter.GetTime(note.moment, note.speed));

        if(!inQueue && Charter.InTime(note.moment)) {
            Judger.DragQueue[note.direction].Enqueue(this);
            inQueue = true;
        }
    }
}
