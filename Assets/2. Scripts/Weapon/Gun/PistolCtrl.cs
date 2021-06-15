using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PistolCtrl : MonoBehaviour
{
    [SerializeField]
    private RifleData data;
    [Header("Location")]
    [SerializeField] private Transform fireLocation;
    [SerializeField] private Transform casingExitLocation;
    public ParticleSystem muzzle;
    
    private Vector3 recoil;
    private Vector3 originRecoil;

    private AudioSource audioSource;

    private Magazine magazine;
    private bool hasSlide;
    private Animator anim;
    private readonly int hashFire = Animator.StringToHash("Fire");
    private float destroyTimer = 0.05f;


    [Header("Bolt Interactable")]
    public XROffsetGrabInteractable offsetGrabInteractable;
    [Header("Magazine Socket")]
    public XRBaseInteractor socketInteractor;
    private XRBaseInteractable grabInteractable;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        grabInteractable = GetComponent<XRBaseInteractable>();
        socketInteractor.selectEntered.AddListener(AddMagazine);
        socketInteractor.selectExited.AddListener(RemoveMagazine);

        recoil = data.Recoil;
        originRecoil = data.Recoil;
    }

    private void FixedUpdate()
    {
        if (grabInteractable.selectingInteractor)
        {
            transform.position = grabInteractable.selectingInteractor.attachTransform.position;
            transform.rotation = grabInteractable.selectingInteractor.attachTransform.rotation;
            grabInteractable.selectingInteractor.attachTransform.localPosition = Vector3.Slerp(grabInteractable.selectingInteractor.attachTransform.localPosition, Vector3.zero, data.RecoilAmount / 0.5f);
            grabInteractable.selectingInteractor.attachTransform.rotation = Quaternion.Slerp(grabInteractable.selectingInteractor.attachTransform.rotation, Quaternion.LookRotation(grabInteractable.selectingInteractor.transform.forward, grabInteractable.selectingInteractor.transform.up), data.RecoilAmount / 1.0f);
        }
    }

    public virtual void Fire()
    {
        if (magazine && magazine.ammoCount > 0)
        {
            var layerMask = 1 << 2;
            layerMask = ~layerMask;
            RaycastHit hit;
            if (Physics.Raycast(fireLocation.position, fireLocation.forward + Random.insideUnitSphere * data.Accuracy, out hit, data.Range, layerMask))
            {
                if (hit.transform.CompareTag("Body"))
                    hit.transform.GetComponent<DamageSystemComponent>()?.TakeGunDamage(data.Power, data.KnockBackForce,hit.point, Quaternion.LookRotation(hit.normal));
                else
                {
                    if (hit.rigidbody != null)
                        hit.rigidbody.AddForce(-hit.normal * data.KnockBackForce);
                    GameObject impactObj = Instantiate(data.ImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactObj, destroyTimer);
                }
            }
            grabInteractable.selectingInteractor.GetComponent<XRBaseController>().SendHapticImpulse(0.7f, 0.05f);
            muzzle.Play();
            audioSource.PlayOneShot(data.fireSfx);
            magazine.ammoCount--;
            anim?.SetTrigger(hashFire);
            CasingRelease();
            ApplyRecoil();
        }
        else
        {
            audioSource.PlayOneShot(data.noAmmoSfx);
        }
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
        Quaternion recoilSlerp = Quaternion.Slerp(grabInteractable.selectingInteractor.attachTransform.rotation, grabInteractable.selectingInteractor.attachTransform.rotation * Quaternion.Euler(-recoil), data.RecoilAmount / 2f);
        grabInteractable.selectingInteractor.attachTransform.rotation = recoilSlerp;
        grabInteractable.selectingInteractor.attachTransform.position -= grabInteractable.selectingInteractor.attachTransform.forward * data.RecoilPosition;
        if (recoil.x + data.IncreaseRecoil.x < 180 && recoil.y + data.IncreaseRecoil.y < 180 && recoil.z + data.IncreaseRecoil.z < 180)
            recoil += data.IncreaseRecoil;
    }
}
