using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShakeBehaviour : MonoBehaviour
{
    void Start()
    {
        transform.DOShakePosition(9999.0f, 0.02f, 1);
        transform.DOShakeRotation(9999.0f, 0.02f, 1);
    }

    void Update()
    {
        
    }
}
