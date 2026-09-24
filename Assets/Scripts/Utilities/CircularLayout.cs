using UnityEngine;

public class CircularLayout : MonoBehaviour
{
    public float radius = 100f;  // Raggio del cerchio
    public bool rotateElements = true; // Ruota gli elementi verso l’esterno

    void Start()
    {
        ArrangeChildren();
    }

    void OnValidate()
    {
        ArrangeChildren();
    }

    void ArrangeChildren()
    {
        int childCount = transform.childCount;
        float angleStep = 360f / childCount;
        float angleOffset = -angleStep / 2f; // 

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            float angle = (i * angleStep + angleOffset) * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            child.localPosition = pos;

            if (rotateElements)
            {
                child.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90f);
            }
        }

    }
}
