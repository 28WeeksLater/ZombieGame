using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrigger: MonoBehaviour
{
    public GameObject target;
    public Transform wayPoint;
    public float speed;
    public bool isFinal;
    private bool isClear;
    public float delayTime;
    private WaitForSeconds ws;

    private void Start()
    {
        ws = new WaitForSeconds(delayTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isClear)
        {
            isClear = true;
            if (other.GetComponent<MoveCar>())
                other.GetComponent<MoveCar>().BreakOn();
            if (isFinal == true)
            {
                target.GetComponent<AudioTrigger>().ClipOn(0);
                StartCoroutine(DelayTime());
            }
            MoveIt();
        }
    }

    void MoveIt()
    {
        StartCoroutine(Move());
    }

    IEnumerator DelayTime()
    {
        yield return ws;
        StartCoroutine(CameraManager.Instance.VignetteIn());
        yield return StartCoroutine(CameraManager.Instance.LiftIn());
        SubCamManager.Instance.SubCamOn();
        SceneLoader.Instance.SceneChange();
    }

    IEnumerator Move()
    {
        while(Vector3.Distance(target.transform.position,wayPoint.position) > 0.0f)
        {
            var step = speed * Time.deltaTime;
            target.transform.position = Vector3.MoveTowards(target.transform.position, wayPoint.position, step);
            yield return null;
        }
    }
}
