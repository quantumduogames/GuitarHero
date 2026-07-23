using UnityEngine;

public class SetMobileSettings : MonoBehaviour
{
    [SerializeField] float rotationAngle = 130; // Angolo di rotazione desiderato
    public void ChangeRotation()
    {
        transform.localRotation = Quaternion.Euler(0, 0, rotationAngle);
    }
}
