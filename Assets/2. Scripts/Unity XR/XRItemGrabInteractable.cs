using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class XRItemGrabInteractable : XRGrabInteractable
{
   public Transform handPos;
   private Transform hand;
   public bool canTakeByOther;
   protected override void OnSelectEntered(SelectEnterEventArgs args)
   {
      base.OnSelectEntered(args);
      if (handPos)
      {
         if (selectingInteractor.transform.Find("Mesh"))
         {
            hand = selectingInteractor.transform.Find("Mesh");
         }
      }
   }

   protected override void OnSelectExited(SelectExitEventArgs args)
   {
      base.OnSelectExited(args);
      if (handPos && hand)
      {
         hand.transform.localPosition = Vector3.zero;
         hand.transform.localRotation = Quaternion.Euler(0, 0, 0);
      }
      hand = null;
   }

   public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
   {
      base.ProcessInteractable(updatePhase);
      
      if(selectingInteractor)
      {
         transform.position = selectingInteractor.attachTransform.position;
         transform.rotation = selectingInteractor.attachTransform.rotation;
      }
      
      if (handPos && hand)
      {
         hand.position = handPos.position;
         hand.rotation = selectingInteractor.GetComponent<XRController>().controllerNode == UnityEngine.XR.XRNode.LeftHand || handPos.localRotation.z == 0 ? handPos.rotation : handPos.rotation * Quaternion.Euler(0, 0, 180);
      }
   }
   
   public void DropIt()
   {
      Drop();
   }

   public override bool IsSelectableBy(XRBaseInteractor interactor)
   {

      if (!canTakeByOther)
      {
         if (isSelected && interactor != selectingInteractor)
            return false;
         else
            return true;
      }
      else
         return true;
   }
}
