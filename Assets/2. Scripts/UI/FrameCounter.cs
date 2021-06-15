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


    IEnumerator worstReset() //�ڷ�ƾ���� 15�� �������� ���� ������ ��������.
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

    void OnGUI()//�ҽ��� GUI ǥ��.
    {

        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;  //�ʴ� ������ - 1�ʿ�

        if (fps < worstFps)  //���ο� ���� fps�� ���Դٸ� worstFps �ٲ���.
            worstFps = fps;
        frameText.text = msec.ToString("F1") + "ms (" + fps.ToString("F1") + ") //worst : " + worstFps.ToString("F1");
    }
}
