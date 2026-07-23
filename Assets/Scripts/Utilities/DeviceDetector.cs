using CrazyGames;
using System.Collections;
using UnityEngine;

public class DeviceDetector : MonoBehaviour
{
    public bool mobileTest = false;
    public static bool isMobiletestActive;

    void Start()
    {
        isMobiletestActive = mobileTest;

        if (CrazySDK.IsAvailable)
        {
            CrazySDK.Init(() =>
            {
                ControllaDispositivo();
            });
        }
    }

    void ControllaDispositivo()
    {
        string dispositivo = CrazySDK.User.SystemInfo.device.type;

        if (mobileTest)
        {
            dispositivo = "mobile"; // Forza il test su mobile
        }

        if (dispositivo == "mobile" || dispositivo == "tablet")
        {
            if (PrepareForMobile.Singleton != null)
            {
                //PrepareForMobile.Singleton.SetCameraForMobile(); // Ruota a 90° e imposta Size a 10
                //PrepareForMobile.Singleton.RotateUI(); // Ruota a 90° e imposta Size a 10
                //PER DEV IN QUESTO CASO PROBABILMENTE NON CI SERVE CAMBIARE NULLA A PARTE LA UI
            }
            Debug.Log("Il giocatore è su TELEFONO o TABLET");
        }
        else
        {
            Debug.Log("Il giocatore è su PC / DESKTOP");
        }
    }


}