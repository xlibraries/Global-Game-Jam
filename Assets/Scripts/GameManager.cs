using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    void Awake()
    {
        if (GameManager.instance != null)
            Destroy(this);
        else
            instance = this;
    }

    public ColorableObject[] colorableObjects;

    public void ResetColorableObjects()
    {
        foreach(ColorableObject colorableObject in colorableObjects)
        {
            colorableObject.isColor = false;
        }
    }

    public void UpdateColorableObjects()
    {
        foreach (ColorableObject colorableObject in colorableObjects)
        {
            if(colorableObject.wasColorLastFrame != colorableObject.isColor)
            {
                if (colorableObject.isColor)
                    colorableObject.MakeColorful();
                else
                    colorableObject.MakeGreyscale();
            }
            colorableObject.wasColorLastFrame = colorableObject.isColor;
        }
    }
}
