using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    private Vector2 startPosition;
    [SerializeField] private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    public void ResetAtStart()
    {
        rb.angularVelocity = 0;
        rb.linearVelocity = Vector2.zero;

        transform.position = startPosition;
    }
}
