using UnityEngine;

public class StrikeFX: MonoBehaviour {

    public SpriteRenderer lightB;
    public SpriteMask darkB;

    private const int minSortingOrder = -32767;
    private const int maxSortingOrder = 32767;
    private static int[] count;

    public static void Init() {
        count = new int[4] {
            minSortingOrder, minSortingOrder, minSortingOrder, minSortingOrder
        };
    }

    public void SetSortLayer(int direction) {

        lightB.sortingOrder = count[direction]++;
        darkB.frontSortingOrder = lightB.sortingOrder;
        darkB.backSortingOrder = lightB.sortingOrder - 1;

        if(count[direction] >= maxSortingOrder)
            count[direction] = minSortingOrder;
    }

    private static Vector2 GetPosition(float rotation, float distance) {
        float sin = Mathf.Sin(rotation), cos = Mathf.Cos(rotation), tan = Mathf.Tan(rotation);
        return distance * new Vector2(
            1 / (Mathf.Sign(cos) * tan + Mathf.Sign(sin)),
            tan / (Mathf.Sign(cos) * tan + Mathf.Sign(sin))
        );
    }

    public Transform[] particles;

    public float particleDistance;

    private float[] particleRotations;

    private void Start() {
        particleRotations = new float[particles.Length];
        for(int i = 0; i < particles.Length; i++)
            particleRotations[i] = Random.Range(0, 2 * Mathf.PI);
    }

    private void Update() {
        for(int i = 0; i < particles.Length; i++)
            particles[i].localPosition = GetPosition(particleRotations[i], particleDistance);
    }
}
