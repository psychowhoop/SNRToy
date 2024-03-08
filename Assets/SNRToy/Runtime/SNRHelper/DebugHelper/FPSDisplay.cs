using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI mFPSText;
    float mDeltaTime = 0.0f;

    void Update()
    {
        mDeltaTime += (Time.unscaledDeltaTime - mDeltaTime) * 0.1f;
        float msec = mDeltaTime * 1000.0f;
        float fps = 1.0f / mDeltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        mFPSText.text = text;
    }
}
