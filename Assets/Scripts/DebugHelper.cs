using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DebugHelper : MonoBehaviour
{
    public UnityEvent StartAction;

    private void Start()
    {
        StartAction?.Invoke();
    }
}
