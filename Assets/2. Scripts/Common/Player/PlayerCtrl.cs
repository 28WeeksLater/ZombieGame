using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    [Header("Player Detail")] [SerializeField]
    private GameObject cam;

    public float fullHP = 100;
    public float HP = 0;
    public int ammoCount = 0;

    [Header("Player Info")] [SerializeField]
    private TextMeshProUGUI hpInfo;

    [SerializeField] private TextMeshProUGUI Info;
    [SerializeField] private Image watchImg;
    [SerializeField] private Sprite[] sprite;
    [SerializeField] private UnityEngine.Rendering.VolumeProfile volumeProfile;
    [Header("XR Interactor")] public XRBaseInteractor left;
    public XRBaseInteractor right;
    [Header("Equipment")] [SerializeField] private GameObject[] ammoCase;
    [Header("Ammo Text")] [SerializeField] private TextMeshProUGUI[] ammoText;
    [Header("SFX")] public AudioClip hitSFX;
    public AudioClip healSFX;
    public AudioClip ammoSFX;
    public AudioClip dieSFX;

    private float initInfoTime;
    private float updateWatch = 1.0f;
    
    private DeviceBasedContinuousMoveProvider cm;
    [HideInInspector] public AudioSource audioSource;
    public bool isDie;
    
    private void Awake()
    {
        HP = fullHP;
        cm = GetComponent<DeviceBasedContinuousMoveProvider>();
        audioSource = GetComponent<AudioSource>();
        HPInfo();
    }

    private void Update()
    {
        if (HP <= 0 && !isDie)
            Die();

        if (isDie)
            cm.enabled = false;

        if (initInfoTime >= updateWatch)
        {
            HPInfo();
        }
        else
        {
            initInfoTime += Time.deltaTime;
        }
    }

    public void GetAmmoCount()
    {
        if (ammoText != null)
        {
            foreach (TextMeshProUGUI text in ammoText)
            {
                text.text = ammoCount.ToString();
            }
        }
    }

    public void SetText(int i, int j)
    {
        watchImg.sprite = sprite[j];
        hpInfo.text = "+ " + i.ToString();
        initInfoTime = 0.0f;
    }

    private void HPInfo()
    {
        watchImg.sprite = sprite[0];
        hpInfo.text = HP.ToString();

        if (HP > 50)
            hpInfo.color = Color.white;
        else if (HP > 30)
            hpInfo.color = Color.yellow;
        else
            hpInfo.color = Color.red;
    }


    public void Hit(float damage)
    {
        HP = HP - damage >= 0 ? HP - damage : 0;
        CameraManager.Instance.Hit();
        audioSource.PlayOneShot(hitSFX);
    }

    public void Die()
    {
        isDie = true;
        CameraManager.Instance.Die();
        audioSource.PlayOneShot(dieSFX);
    }

    public void ResetState()
    {
        HP = fullHP;
        isDie = false;
    }
}
