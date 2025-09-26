using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIToggle : MonoBehaviour
{
    [SerializeField]
    private GameObject trueShow, falseShow;
    private Toggle toggle;

    private void OnEnable()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleChanged);
        OnToggleChanged(toggle.isOn);
    }

    private void OnDisable()
    {
        GetComponent<Toggle>().onValueChanged.RemoveListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool value)
    {
        trueShow.SetActive(value);
        falseShow.SetActive(!value);
    }

    public void Toggle()
    {
        toggle.isOn = !toggle.isOn;
    }
}
