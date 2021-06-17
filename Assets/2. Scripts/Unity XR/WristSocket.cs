using Pixelplacement;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WristSocket : XRSocketInteractor
{
    private PlayerCtrl player;
    [SerializeField] private string targetTag;
    // Sizing
    public float targetSize = 0.25f;
    public float sizingDuration = 0.25f;
    // Runtime
    private Vector3 originalScale = Vector3.one;
    private Vector3 objectSize = Vector3.zero;
    // Prevents random objects from being selected
    private bool canSelect = false;
    [Header("Move & Rotate By Target")]
    public bool move = false;
    public Transform target;
    public Vector3 offset;

    private TwoHandGrabInteractable two = null;
    private XRItemGrabInteractable item = null;
    private XROffsetGrabInteractable offsetGrab = null;
    private XRSimpleInteractable simple = null;
    private XRSocketInteractorWithName socket = null;
    private DamageSystem ds = null;
    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<PlayerCtrl>();
    }

    private void Update()
    {
        if(move)
        {
            transform.rotation = new Quaternion(0, 0, 0, target.rotation.w - transform.rotation.w);
            transform.position = target.position + Vector3.up * offset.y + Vector3.ProjectOnPlane(target.right, Vector3.up).normalized * offset.x
                + Vector3.ProjectOnPlane(target.forward, Vector3.up).normalized * offset.z;
        }
    }

    public override bool CanSelect(XRBaseInteractable interactable)
    {
        return base.CanSelect(interactable) && interactable.CompareTag(targetTag) && canSelect;
    }

    protected override void OnHoverEntering(HoverEnterEventArgs args)
    {
        base.OnHoverEntering(args);

        // If the object is already selected, wrist can grab it
        if (args.interactable.isSelected)
            canSelect = true;
    }

    protected override void OnHoverExiting(HoverExitEventArgs args)
    {
        base.OnHoverExiting(args);

        // If the wrist didn't grab the object, we can no longer select
       if (!selectTarget)
            canSelect = false;
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    { 
        
        // Store object when select begins
        base.OnSelectEntering(args);
        StoreObjectSizeScale(args.interactable);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Once select has occured, scale object to size
        var temp = args.interactable;
        
        if (temp is TwoHandGrabInteractable)
        {
            two = temp.GetComponent<TwoHandGrabInteractable>();
            two.canTakeByOther = true;

            if (temp.GetComponentInChildren<XRSimpleInteractable>())
            {
                simple = temp.GetComponentInChildren<XRSimpleInteractable>();
                simple.enabled = false;
            }
        }
        else if (temp is XRItemGrabInteractable)
        {
            item = temp.GetComponent<XRItemGrabInteractable>();
            if (item.GetComponent<DamageSystem>())
            {
                ds = item.GetComponent<DamageSystem>();
                ds.enabled = false;
            }
            item.canTakeByOther = true;
        }

        if (temp.GetComponentInChildren<XROffsetGrabInteractable>())
        {
            offsetGrab = temp.GetComponentInChildren<XROffsetGrabInteractable>();
            offsetGrab.enabled = false;
        }
        if (temp.GetComponentInChildren<XRSocketInteractorWithName>())
        {
            socket = temp.GetComponentInChildren<XRSocketInteractorWithName>();
            if (socket.selectTarget)
            {
                var mag = socket.selectTarget.GetComponent<Magazine>();
                player.ammoCount += mag.ammoCount;
                player.SetText(mag.ammoCount,1);
                Destroy(mag.gameObject);
            }
            socket.showInteractableHoverMeshes = false;
            socket.enabled = false;
        }
        base.OnSelectEntered(args);
        TweenSizeToSocket(temp);
       
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // Once the user has grabbed the object, scale to original size
        if (two)
        {
            two.canTakeByOther = false;
            two = null;
            if (simple)
            {
                simple.enabled = true;
                simple = null;
            }
        }
        else if (item)
        {
            item.canTakeByOther = false;
            if (ds)
            {
                ds.enabled = true;
                ds = null;
            }
            item = null;
        }

        if (offsetGrab)
        {
            offsetGrab.enabled = true;
            offsetGrab = null;
        }

        if (socket)
        {
            socket.showInteractableHoverMeshes = true;
            socket.enabled = true;
        }
        base.OnSelectExited(args);
        SetOriginalScale(args.interactable);
    }

    private void StoreObjectSizeScale(XRBaseInteractable interactable)
    {
        // Find the object's size
        objectSize = FindObjectSize(interactable.gameObject);
        originalScale = interactable.transform.localScale;
    }

    private Vector3 FindObjectSize(GameObject objectToMeasure)
    {
        Vector3 size = Vector3.one;

        // Assumes the interactable has one mesh on the root
        if (objectToMeasure.TryGetComponent(out MeshFilter meshFilter))
            size = ColliderMeasurer.Instance.Measure(meshFilter.mesh);

        return size;
    }

    private void TweenSizeToSocket(XRBaseInteractable interactable)
    {
        // Find the new scale based on the size of the collider, and scale
        Vector3 targetScale = FindTargetScale();

        // Tween to our new scale
        Tween.LocalScale(interactable.transform, targetScale, sizingDuration, 0);
    }

    private Vector3 FindTargetScale()
    {
        // Figure out new scale, we assume the object is originally uniformly scaled
        float largestSize = FindLargestSize(objectSize);
        float scaleDifference = targetSize / largestSize;
        return Vector3.one * scaleDifference;
    }

    private void SetOriginalScale(XRBaseInteractable interactable)
    {
        // This isn't necessary, but prevents an error when exiting play

        if(interactable)
        {
            interactable.transform.localScale = originalScale;

            originalScale = Vector3.one;
            objectSize = Vector3.zero;

        }
        // Restore object to original scale

        // Reset just incase
    }

    private float FindLargestSize(Vector3 value)
    {
        float largestSize = Mathf.Max(value.x, value.y);
        largestSize = Mathf.Max(largestSize, value.z);
        return largestSize;
    }

    public override XRBaseInteractable.MovementType? selectedInteractableMovementTypeOverride
    {
        // Move while ignoring physics, and no smoothing
        get { return XRBaseInteractable.MovementType.Instantaneous; }
    }

    // Is the socket active, and object is being held by different interactor
    //public override bool isSelectActive => base.isSelectActive && canSelect;

    private void OnDrawGizmos()
    {
        // Draw the approximate size of the socketed object
        Gizmos.DrawWireSphere(transform.position, targetSize * 0.5f);
    }
}



