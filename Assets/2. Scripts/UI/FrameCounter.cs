using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FrameCounter : MonoBehaviour
{
    float deltaTime = 0.0f;

    float msec;
    float fps;
    float worstFps = 100f;
    public TextMeshProUGUI frameText;

    void Awake()
    {
        StartCoroutine("worstReset");
    }


    IEnumerator worstReset() //코루틴으로 15초 간격으로 최저 프레임 리셋해줌.
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            worstFps = 100f;
        }
    }


    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()//소스로 GUI 표시.
    {

        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;  //초당 프레임 - 1초에

        if (fps < worstFps)  //새로운 최저 fps가 나왔다면 worstFps 바꿔줌.
            worstFps = fps;
        frameText.text = msec.ToString("F1") + "ms (" + fps.ToString("F1") + ") //worst : " + worstFps.ToString("F1");
    }
}
