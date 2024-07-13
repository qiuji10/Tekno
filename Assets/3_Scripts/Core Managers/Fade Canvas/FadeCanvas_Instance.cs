using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCanvas_Instance : MonoBehaviour
{
    public void OpenFadeCanvas()
    {
        FadeCanvas.Instance.FadeOut();
    }

    public void CloseFadeCanvas()
    {
        FadeCanvas.Instance.FadeIn();
    }
}
