using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene0Ctrl : MonoBehaviour
{
    public GameObject rig;
    public GameObject cam;
    public GameObject wife;
    public GameObject bus;
    public Transform wifePos;
    public CanvasGroup cg;

    public void Scene0On()
    {
        cg.alpha = 0;
        StartCoroutine(Scene0Move());
    }

    IEnumerator Scene0Move()
    {
        StartCoroutine(CameraManager.Instance.VignetteIn());
        yield return StartCoroutine(CameraManager.Instance.LiftIn());
        cam.SetActive(false);
        rig.SetActive(true);
        wife.transform.SetParent(bus.transform);
        wife.transform.position = wifePos.position;
        wife.transform.rotation = wifePos.rotation;
        wife.GetComponent<Animator>()?.SetBool("Screaming",true);
        wife.GetComponent<AudioSource>()?.Play();
        yield return StartCoroutine(CameraManager.Instance.LiftOut()); 
        StartCoroutine(CameraManager.Instance.VignetteOut());
        yield return StartCoroutine(CanvasControll());
        StopAllCoroutines();
    }

    IEnumerator CanvasControll()
    {
        while(cg.alpha<1.0f)
        {
            cg.alpha += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(3.0f);
        while (cg.alpha > 0.0f)
        {
            cg.alpha -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
