using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyVFX : MonoBehaviour
{
    public void DestroyTheVFX()
    {
        Destroy(gameObject);
    }

    internal void StartDestroyVFX()
    {
        StartCoroutine(DestroyVfxFromCode());
    }


    public IEnumerator DestroyVfxFromCode()
    {
        var vfx = GetComponent<ParticleSystem>();

        if (vfx != null)
        {
            yield return new WaitForSeconds(vfx.main.duration);
            Destroy(gameObject);
        }
    }
}
