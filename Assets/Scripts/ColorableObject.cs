using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorableObject : MonoBehaviour
{
    public bool wasColorLastFrame = false;
    public bool isColor = false;

    public ParticleSystem particleFX;
    public Color tempGreyscale;
    public Color tempColor;

    public void MakeColorful()
    {
        StopAllCoroutines();
        StartCoroutine(ColorFade(tempGreyscale, tempColor));
    }

    public void MakeGreyscale()
    {
        StopAllCoroutines();
        StartCoroutine(ColorFade(tempColor, tempGreyscale));
    }

    IEnumerator ColorFade(Color colorStart, Color colorEnd)
    {
        Material material = GetComponentInChildren<Renderer>().material;
        material.color = colorStart;

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
            material.color = Color.Lerp(colorStart, colorEnd, count / 0.1f);
            count += Time.deltaTime;
            yield return null;
        }
        material.color = colorEnd;
    }
}
