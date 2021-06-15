using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MachineGunCtrl : MonoBehaviour
{
    [Header("Location")]
    [SerializeField] private Transform[] fireLocation;
    [SerializeField] private Transform[] casingExitLocation;

    [Header("Constituent  Prefab")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject casingPrefab;
    [SerializeField] private ParticleSystem[] muzzleFlashPrefab;
    [SerializeField] private GameObject impactPrefab;

    [Header("Machine Gun Detail Info")]
    [SerializeField] private float power = 10.0f;
    [SerializeField] private float range = 40.0f;
    [SerializeField] private float accuracy = 0.015f;
    [SerializeField] private float muzzleVelocity = 0.07f;
    [SerializeField] private float ejectPower = 300.0f;
    [SerializeField] private float destroyTimer = 2.0f;
    [SerializeField] private float impactDestroyTimer = 0.5f;
    [SerializeField] private float knockBackForce = 0.00003f;
    private float delay;
    private Coroutine firing;
    private AudioSource audioSource;

    [Header("Sound SFX")]
    public AudioClip fireSfx;

    private XRBaseInteractable interactable;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        interactable = GetComponent<XRBaseInteractable>();
        interactable.activated.AddListener(StartFire);
        interactable.deactivated.AddListener(EndFire);
        foreach (var i in muzzleFlashPrefab)
        {
            i.Stop();
        }
    }

    private void StartFire(ActivateEventArgs args)
    {
        firing = StartCoroutine(Trigger());
    }

    private void EndFire(DeactivateEventArgs args)
    {
        if (firing != null)
        {
            StopCoroutine(firing);
        }
    }

    private IEnumerator Trigger()
    {
        while (true)
        {
            if (delay <= Time.time)
            {
                delay = Time.time + muzzleVelocity;
                for(int i=0; i<fireLocation.Length;i++)
                    Fire(fireLocation[i],casingExitLocation[i],muzzleFlashPrefab[i]);
            }
            yield return null;
        }
    }

    private void Fire(Transform tr1, Transform tr2, ParticleSystem particleSystem)
    {
        var layerMask = 1 << 2;
        layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(tr1.position, tr1.forward + Random.insideUnitSphere * accuracy, out hit, range, layerMask))
        {
            if (hit.transform.CompareTag("Body"))
                hit.transform.GetComponent<DamageSystemComponent>()?.TakeGunDamage(power, knockBackForce,hit.point, Quaternion.LookRotation(hit.normal));
            else
            {
                if (hit.rigidbody != null)
                    hit.rigidbody.AddForce(-hit.normal * knockBackForce);
                GameObject impactObj = Instantiate(impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactObj, impactDestroyTimer);
            }
        }
        interactable.selectingInteractor.GetComponent<XRBaseController>().SendHapticImpulse(0.7f, 0.05f);
        particleSystem.Play();
        audioSource.PlayOneShot(fireSfx);
        CasingRelease(tr2);
    }
    

    void CasingRelease(Transform tr)
    {
        if (!casingPrefab)
        { return; }

        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, tr.position, tr.rotation) as GameObject;
        var temp = tempCasing.GetComponent<Rigidbody>();
        temp.AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (tr.position - tr.right * 0.3f - tr.up * 0.6f), 1f);
        temp.AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        Destroy(tempCasing, destroyTimer);
    }
}
