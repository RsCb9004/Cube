using UnityEngine;

public static class NoteBehaviours {

    public delegate Vector2 GetNotePosition(int direction, float time);
    public delegate float GetNoteRotation(int direction, float time);

    public static readonly Vector2[] Vectors = {
        Vector2.up, Vector2.right, Vector2.down, Vector2.left
    };
    private static readonly float[] Angles = {
        0.5f * Mathf.PI, 0, -0.5f * Mathf.PI, -Mathf.PI
    };

    public static readonly GetNotePosition[] GetNotePositions = {
        GetBluePosition, GetYellowPosition, GetRedPosition
    };
    public static readonly GetNoteRotation[] GetNoteRotations = {
        GetBlueRotation, GetRedRotation, GetYellowRotation
    };

    private static Vector2 DirectionVector(float direction) =>
        new Vector2(Mathf.Cos(direction), Mathf.Sin(direction));

    public static Vector2 GetBluePosition(int direction, float time) =>
        (time + 0.5f) * Charter.DistancePerBeat * Vectors[direction];
    public static Vector2 GetRedPosition(int direction, float time) =>
        (time - 0.5f) * Charter.DistancePerBeat * -Vectors[direction];
    public static Vector2 GetYellowPosition(int direction, float time) {
        if(time > 2) return
            (time - 0.5f) * Charter.DistancePerBeat * -Vectors[direction];
        else if(time > 1) return
            1.5f * Charter.DistancePerBeat * DirectionVector(
                Mathf.Lerp(Angles[direction], Angles[direction] + Mathf.PI, time - 1)
            );
        else return
            (time + 0.5f) * Charter.DistancePerBeat * Vectors[direction];
    }

    public static float GetBlueRotation(int direction, float time) =>
        Angles[direction] * Mathf.Rad2Deg;
    public static float GetRedRotation(int direction, float time) =>
        (Angles[direction] + Mathf.PI) * Mathf.Rad2Deg;
    public static float GetYellowRotation(int direction, float time) {
        if(time > 2) return
            (Angles[direction] + Mathf.PI) * Mathf.Rad2Deg;
        else if(time > 1) return
            Mathf.Lerp(Angles[direction] + Mathf.PI, Angles[direction], time - 1) * Mathf.Rad2Deg;
        else return
            Angles[direction] * Mathf.Rad2Deg;
    }
}
