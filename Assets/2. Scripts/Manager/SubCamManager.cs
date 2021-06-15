using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubCamManager : MonoBehaviour
{
    public GameObject subCam;
    [HideInInspector]public bool camOn;
    public GameObject logo;
    public Canvas canvas;
    private CanvasGroup cg;
    public Image image;

    public float delayTime;
    private WaitForSeconds ws;

    private static SubCamManager instance = null;
    public static SubCamManager Instance
    {
        get
        {
            if (instance == null)
                return null;

            else return instance;
        }
    }

    void Start()
    {
        ws = new WaitForSeconds(delayTime);
        cg = canvas.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public void SubCamOff()
    {
        camOn = false;
        subCam.SetActive(false);
    }
    public void SubCamOn()
    {
        camOn = true;
        subCam.SetActive(true);
    }

    public void LogoOn()
    {
        logo.SetActive(true);
    }

    public void LogoOff()
    {
        logo.SetActive(false);
    }

    public void CanvasOn()
    {
        canvas.enabled = true;
    }

    public void CanvasOff()
    {
        canvas.enabled = false;
    }

    public void CanvasAlphaOn()
    {
        StartCoroutine(AlphaOn());   
    }

    public void CanvasAlphaOff()
    {
        StartCoroutine(AlphaOff());
    }
    
    private IEnumerator AlphaOn()
    {
        while (cg.alpha < 1.0f)
        {
            cg.alpha += 0.01f;
            yield return ws;
        }
    }
    private IEnumerator AlphaOff()
    {
        while (cg.alpha > 0f)
        {
            cg.alpha -= 0.01f;
            yield return ws;
        }
    }

}
