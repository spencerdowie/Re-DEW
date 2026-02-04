using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerUI : MonoBehaviour
{
    //Out, In, Left, Up, Down, Right
    [SerializeField]
    private TMPro.TextMeshProUGUI valueText;
    [SerializeField]
    private MeshRenderer playerUIModel;
    private Material outer, inner;
    private Material[] ammoBars = new Material[4];
    [SerializeField]
    private Color playerColour;


    private void Awake()
    {
        Material[] mats = playerUIModel.materials;
        outer = mats[0];
        inner = mats[1];
        ammoBars[0] = mats[2];
        ammoBars[1] = mats[3];
        ammoBars[2] = mats[5];
        ammoBars[3] = mats[4];
        ammoBars[0].SetVector("_Direction", Vector2.up);
        ammoBars[1].SetVector("_Direction", Vector2.right);
        ammoBars[2].SetVector("_Direction", Vector2.down);
        ammoBars[3].SetVector("_Direction", Vector2.left);
    }

    public void SetAmmo(int ammoCount)
    {
        if (ammoCount >= 0 && ammoCount <= 4)
        {
            for (int i = 0; i < 4; i++)
            {
                ammoBars[i].SetFloat("_Lit", ammoCount - i);
            }
        }
    }

    public void SetValue(int value)
    {
        valueText.text = value.ToString();
    }

    public void SetPlayerColour(Color color)
    {
        playerColour = color;
        foreach (Material mat in ammoBars)
        {
            mat.SetColor("_PlayerColour", color);
        }
        //outer.SetColor("_PlayerColour", color);
        //inner.SetColor("_PlayerColour", color);
    }
}
