using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorableObject : MonoBehaviour
{
    public bool wasColorLastFrame = false;
    public bool isColor = false;

    public ParticleSystem particleFX;
    public Color Greyscale;
    public Color Colorful;

    public void MakeColorful()
    {
        StopAllCoroutines();
        StartCoroutine(ColorFade(Greyscale, Colorful));
    }

    public void MakeGreyscale()
    {
        StopAllCoroutines();
        StartCoroutine(ColorFade(Colorful, Greyscale));
    }

    IEnumerator ColorFade(Color colorStart, Color colorEnd)
    {
        float startBlend = 0;
        float endBlend = 1;
        if (colorStart != Greyscale) {
            startBlend = 1;
            endBlend = 0;
        }
        Material material = GetComponentInChildren<Renderer>().material;
        material.SetFloat("_Blend", startBlend);

        Animator anim = GetComponent<Animator>();
        anim.Play("Normal", -1, 0f);
        anim.SetTrigger("Squash");

        particleFX.Stop();
        ParticleSystem.MainModule main = particleFX.main;
        main.startColor = colorEnd;
        particleFX.Play();

        float count = 0;
        while(count < 0.1f)
        {
            material.SetFloat("_Blend", Mathf.Lerp(startBlend, endBlend, count / 0.1f));
            count += Time.deltaTime;
            yield return null;
        }
        material.SetFloat("_Blend", endBlend);
    }
}
