using System.Collections.Generic;
using UnityEngine;

public class PrepareForMobile : MonoBehaviour
{
    public static PrepareForMobile Singleton { get; private set; }

    public List<SetMobileSettings> elementsToRotate;


    [SerializeField] private float cameraRotationZ = -90f; // Rotazione della telecamera per mobile
    [SerializeField] private float cameraDistance = 10f; // Rotazione della telecamera per mobile

    // Trascina la tua telecamera principale qui dall'Inspector di Unity
    public Camera mainCamera;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Se non l'hai trascinata, prova a prenderla in automatico
        }
    }

    public void SetCameraForMobile()
    {
        if (mainCamera != null)
        {
            // Modifica la rotazione sull'asse Z (mantenendo X e Y a 0)
            mainCamera.transform.rotation = Quaternion.Euler(0f, 0f, cameraRotationZ);

            // Modifica la dimensione della telecamera Ortografica
            if (mainCamera.orthographic)
            {
                mainCamera.orthographicSize = cameraDistance;
            }
            else
            {
                // Se usi una telecamera in Prospettiva (3D), la grandezza si cambia con il Field of View (FOV)
                // mainCamera.fieldOfView = 60f; // Scommenta questa linea se il tuo gioco è 3D
                Debug.LogWarning("La telecamera non è impostata su Ortografica (2D).");
            }
        }
    }

    public void RotateUI()
    {
        RotateAllElements();
    }

    void RotateAllElements()
    {
        foreach (var item in elementsToRotate)
        {
            item.ChangeRotation();
        }
    }

}
