using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LightningSystemBase : MonoBehaviour
{
    protected Color color;
    public void SetColour(Color color)
    {
        this.color = color;
    }
}
