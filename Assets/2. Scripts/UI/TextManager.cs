using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
   public TextMeshProUGUI text;
   public CanvasGroup _canvasGroup;
   private static TextManager instance = null;
   public static TextManager Instance
   {
      get
      {
         if (instance == null)
            return null;

         else return instance;
      }
   }

   private void Start()
   {
      if (instance == null)
      {
         instance = this;
         DontDestroyOnLoad(this.gameObject);
      }
      else
         Destroy(this.gameObject);
   }

   public void TextSetting(float time, string content)
   {
      StartCoroutine(ShowText(time,content));
   }

   IEnumerator ShowText(float time, string content)
   {
      text.text = content;
      while(_canvasGroup.alpha<1.0f)
      {
         _canvasGroup.alpha += 0.01f;
         yield return new WaitForSeconds(0.01f);
      }
      yield return new WaitForSeconds(time);
      while (_canvasGroup.alpha > 0.0f)
      {
         _canvasGroup.alpha -= 0.01f;
         yield return new WaitForSeconds(0.01f);
      }
   }
}
