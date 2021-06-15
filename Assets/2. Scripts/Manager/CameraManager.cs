using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.Rendering.VolumeProfile volumeProfile;
    private UnityEngine.Rendering.Universal.Vignette vignette;
    private UnityEngine.Rendering.Universal.LiftGammaGain lift;
    private float intensity = 1.0f;
    private float liftValue = -1.0f;

    private static CameraManager instance = null;
    public static CameraManager Instance
    { get
        {
            if (instance == null)
                return null;

            else return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
            
        if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));
        if (!volumeProfile.TryGet(out lift)) throw new System.NullReferenceException(nameof(lift));
        vignette.intensity.Override(1.0f);
        lift.lift.Override(new Vector4(0, 0, 0, 0));
        
        StartCoroutine(VignetteOut());
        StartCoroutine(LiftOut());
    }

    public IEnumerator VignetteIn()
    {
        while (vignette.intensity.value < 1.0f)
        {
            intensity += 0.01f;
            vignette.intensity.Override(intensity);

            yield return new WaitForSeconds(0.01f);
        }
        vignette.intensity.Override(1.0f);
    }

    public IEnumerator LiftIn()
    {
        while (liftValue > -1.0f)
        {
            liftValue -= 0.01f;
            lift.lift.Override(new Vector4(0, 0, 0, liftValue));
            yield return new WaitForSeconds(0.01f);
        }
        lift.lift.Override(new Vector4(0, 0, 0, -1));
    }

    public IEnumerator VignetteOut()
    {
        while (vignette.intensity.value > 0.0f)
        {
            intensity -= 0.01f;
            vignette.intensity.Override(intensity);
            yield return new WaitForSeconds(0.01f);
        }

        vignette.intensity.Override(0.0f);
    }

    public IEnumerator LiftOut()
    {
        while (liftValue < 0.0f)
        {
            liftValue += 0.01f;
            lift.lift.Override(new Vector4(0, 0, 0, liftValue));
            yield return new WaitForSeconds(0.01f);
        }
        lift.lift.Override(new Vector4(0, 0, 0, 0));
    }
   
    public void Hit()
    {
        StartCoroutine(HitCouroutine());
    }

    private IEnumerator HitCouroutine()
    {
        vignette.intensity.Override(0.8f);
        vignette.color.Override(Color.red);
        yield return new WaitForSeconds(0.2f);
        ResetVolume();
    }
    

    public void ResetVolume()
    {
        vignette.color.Override(Color.black);
        StartCoroutine(VignetteOut());
        StartCoroutine(LiftOut());
    }

    public void Die()
    {
        vignette.color.Override(Color.red);
        StartCoroutine(VignetteIn());
        StartCoroutine(LiftIn());
        SubCamManager.Instance.LogoOff();
        SubCamManager.Instance.SubCamOn();
        SubCamManager.Instance.CanvasOn();
        SubCamManager.Instance.CanvasAlphaOn();
    }

    public void Restart()
    {
        vignette.color.Override(Color.black);
        StartCoroutine(VignetteOut());
        StartCoroutine(LiftOut());
        SubCamManager.Instance.LogoOn();
        SubCamManager.Instance.SubCamOff();
        SubCamManager.Instance.CanvasOff();
        SubCamManager.Instance.CanvasAlphaOff();
    }
    
    
}
