using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] internal static float velocity = 50;

    private void Update()
    {
        transform.Rotate(0, 0, velocity * Time.deltaTime);
    }
}
