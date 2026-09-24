using System.Collections.Generic;
using UnityEngine;

public class PlayVFX : MonoBehaviour
{
    public List<ParticleSystem> VFX;

    public void ActiveAndPlayVFX()
    {
        for (int i = 0; i < VFX.Count; i++)
        {
            VFX[i].gameObject.SetActive(true);
            VFX[i].Play();
        }
    }
}
