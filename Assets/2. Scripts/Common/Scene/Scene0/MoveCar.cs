using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public Transform[] wayPoints;
    public Transform handle;
    private Vector3 currPoint;
    private int index = 0;
    public float speed;
    private AudioSource audioSource;
    public AudioClip carSfx;
    public AudioClip breakSfx;
    public AudioClip collideSfx;
    private bool isCollide;
    public Scene0Ctrl sc;
        
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = carSfx;
        audioSource.Play();
        StartCoroutine(Move());
    }
    IEnumerator Move()
    { 
        while (index < wayPoints.Length)
        {
            var step = speed * Time.deltaTime;
            currPoint = transform.position;
            transform.position = Vector3.MoveTowards(currPoint, wayPoints[index].position,step);
            var rot = Vector3.RotateTowards(currPoint, wayPoints[index].position, step*50, 0.0f);
            transform.rotation = Quaternion.LookRotation(rot);
            if (Vector3.Distance(wayPoints[index].position, currPoint) <= 1.0f)
            {
                index++;
            }
            yield return null;
        }
    }

    IEnumerator BreakSound()
    {
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(breakSfx);
        handle.localRotation = Quaternion.Lerp(handle.localRotation, Quaternion.Euler(handle.localRotation.x, handle.localRotation.y, 90.0f),3.0f);
    }

    public void BreakOn()
    {
        StartCoroutine(BreakSound());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.CompareTag("Wall") && !isCollide)
        {
            isCollide = true;
            sc.Scene0On();
        }
    }
}
