using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonObject : MonoBehaviour
{
    public void LoadGame()
    {
         SceneLoader.Instance.SceneChange();
    }

    public void ExitGame()
    {
        SceneLoader.Instance.ExitGame();
    }
}
