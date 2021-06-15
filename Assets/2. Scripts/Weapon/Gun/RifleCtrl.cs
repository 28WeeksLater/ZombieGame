using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;

public class RifleCtrl : MonoBehaviour
{
    [Header("Rifle Data")]
    [SerializeField] private RifleData data;

    [Header("Location")]
    [SerializeField] private Transform fireLocation;
    [SerializeField] private Transform casingExitLocation;
    public ParticleSystem muzzle;
    private Vector3 recoil;
    private Vector3 originRecoil;

    private float delay;
    private Coroutine firing;
    private AudioSource audioSource;

    private Magazine magazine;
    private bool hasSlide;
    private Animator anim;
    private readonly int hashFire = Animator.StringToHash("Fire");
    private float destroyTimer = 1.0f;


    [Header("Bolt Interactable")]
    public XROffsetGrabInteractable offsetGrabInteractable;
    [Header("Magazine Socket")]
    public XRSocketInteractorWithName socketInteractor;
    private TwoHandGrabInteractable twoHandGrabInteractable;

    private float dropDelay = 1.0f;
    private float initTime = 0.0f;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        twoHandGrabInteractable = GetComponent<TwoHandGrabInteractable>();
        socketInteractor.selectEntered.AddListener(AddMagazine);
        socketInteractor.selectExited.AddListener(RemoveMagazine);
        twoHandGrabInteractable.activated.AddListener(StartFire);
        twoHandGrabInteractable.deactivated.AddListener(EndFire);
        
        delay = Time.time;
        recoil = data.Recoil;
        originRecoil = data.Recoil;
    }

    private void FixedUpdate()
    {
        if (twoHandGrabInteractable.selectingInteractor)
        { 
            twoHandGrabInteractable.selectingInteractor.attachTransform.localPosition = Vector3.Slerp(twoHandGrabInteractable.selectingInteractor.attachTransform.localPosition, Vector3.zero, data.RecoilAmount / 0.5f);
            if (!twoHandGrabInteractable.onSecond)
                twoHandGrabInteractable.selectingInteractor.attachTransform.rotation = Quaternion.Slerp(twoHandGrabInteractable.selectingInteractor.attachTransform.rotation, Quaternion.LookRotation(twoHandGrabInteractable.selectingInteractor.transform.forward, twoHandGrabInteractable.selectingInteractor.transform.up), data.RecoilAmount / 1.0f);
        }
    }

    private void Update()
    {
        if (initTime >= dropDelay)
        {
            initTime = 0;
            socketInteractor.canSelect = true;
        }
        else if(!socketInteractor.canSelect)
        {
            initTime += Time.deltaTime;
        }
        
        if (magazine && twoHandGrabInteractable.selectingInteractor)
        {
            if (twoHandGrabInteractable.selectingInteractor.GetComponent<XRController>().inputDevice.TryGetFeatureValue(CommonUsages.primaryButton,out bool press) && press)
            {
                socketInteractor.canSelect = false;
                socketInteractor.selectTarget.GetComponent<XRItemGrabInteractable>().DropIt();
            }
        }
    }

    private void StartFire(ActivateEventArgs args)
    {
        firing = StartCoroutine(Trigger());
    }

    private void EndFire(DeactivateEventArgs args)
    {
        if(firing != null)
        {
            StopCoroutine(firing);
        }
        recoil = originRecoil;
    }

    private IEnumerator Trigger()
    {
        while(true)
        {
            if (magazine && magazine.ammoCount > 0 && hasSlide && !offsetGrabInteractable.onOffset)
            {
                if (delay <= Time.time)
                {
                    delay = Time.time + data.MuzzleVelocity;
                    Fire();
                }
            }
            else
            {
                audioSource.PlayOneShot(data.noAmmoSfx);
                break;
            }
            yield return null;
        }
    }

    private void Fire()
    {
        var layerMask = 1 << 2;
        layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(fireLocation.position, fireLocation.forward+Random.insideUnitSphere*data.Accuracy, out hit, data.Range, layerMask))
        {
            if (hit.transform.CompareTag("Body"))
                hit.transform.GetComponent<DamageSystemComponent>()?.TakeGunDamage(data.Power, data.KnockBackForce,hit.point, Quaternion.LookRotation(hit.normal));
            else
            {
                if (hit.rigidbody)
                    hit.rigidbody.AddForce(-hit.normal * data.KnockBackForce);
                GameObject impactObj = Instantiate(data.ImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactObj, destroyTimer);
            }
        }
        twoHandGrabInteractable.selectingInteractor.GetComponent<XRBaseController>().SendHapticImpulse(0.7f,0.05f);
        if(twoHandGrabInteractable.secondInteractor)
            twoHandGrabInteractable.secondInteractor.GetComponent<XRBaseController>().SendHapticImpulse(0.7f, 0.05f);
        //MuzzleFlash();
        muzzle.Play();
        audioSource.PlayOneShot(data.fireSfx);
        magazine.ammoCount--;
        if(anim)
            anim.SetTrigger(hashFire);
        CasingRelease();
        ApplyRecoil();
    }

    public void AddMagazine(SelectEnterEventArgs args)
    {
        magazine = args.interactable.GetComponent<Magazine>();
        hasSlide = false;
    }

    public void RemoveMagazine(SelectExitEventArgs args)
    {
        magazine = null;
    }

    public void Slide()
    {
        if (hasSlide && magazine && magazine.ammoCount > 0)
        {
            if (!casingExitLocation || !data.BulletPrefab)
            { return; }

            GameObject tempCasing;
            tempCasing = Instantiate(data.BulletPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
            var temp = tempCasing.GetComponent<Rigidbody>();
            temp.AddExplosionForce(Random.Range(data.EjectPower * 0.7f, data.EjectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
            temp.AddTorque(new Vector3(0, Random.Range(100f, 200f), Random.Range(100f, 300f)), ForceMode.Impulse);
           
            Destroy(tempCasing, destroyTimer);
            magazine.ammoCount--;
        }
        hasSlide = true;
        audioSource.PlayOneShot(data.reloadSfx);
    }

    void CasingRelease()
    {
        if (!casingExitLocation || !data.CastingPrefab)
        { return; }

        GameObject tempCasing;
        tempCasing = Instantiate(data.CastingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        var temp = tempCasing.GetComponent<Rigidbody>();
        temp.AddExplosionForce(Random.Range(data.EjectPower * 0.7f, data.EjectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        temp.AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        Destroy(tempCasing, destroyTimer);
    }

    void ApplyRecoil()
    {
        Quaternion recoilSlerp = Quaternion.Slerp(twoHandGrabInteractable.selectingInteractor.attachTransform.rotation, twoHandGrabInteractable.selectingInteractor.attachTransform.rotation * Quaternion.Euler(-recoil), data.RecoilAmount / 2f);
        twoHandGrabInteractable.selectingInteractor.attachTransform.rotation = recoilSlerp;
        twoHandGrabInteractable.selectingInteractor.attachTransform.position -= twoHandGrabInteractable.selectingInteractor.attachTransform.forward * data.RecoilPosition;
        if (recoil.x + data.IncreaseRecoil.x < 180 && recoil.y + data.IncreaseRecoil.y < 180 && recoil.z + data.IncreaseRecoil.z < 180)
            recoil += data.IncreaseRecoil;
    }

}
