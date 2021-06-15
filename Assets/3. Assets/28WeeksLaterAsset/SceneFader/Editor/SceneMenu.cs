using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneMenu
{
    [MenuItem("Scenes/Menu")]
    public static void OpenMenu()
    {
        OpenScene("Menu");
    }

    [MenuItem("Scenes/Scene 0")]
    public static void OpenScene0()
    {
        OpenScene("Scene 0");
    }

    [MenuItem("Scenes/Scene 1")]
    public static void OpenScene1()
    {
        OpenScene("Scene 1");
    }

    [MenuItem("Scenes/Scene 2")]
    public static void OpenScene2()
    {
        OpenScene("Scene 2");
    }

    [MenuItem("Scenes/Scene 3")]
    public static void OpenScene3()
    {
        OpenScene("Scene 3");
    }
    
    [MenuItem("Scenes/Ending")]
    public static void OpenScene4()
    {
        OpenScene("Ending");
    }

    private static void OpenScene(string sceneName)
    {
        EditorSceneManager.OpenScene("Assets/Scenes/SceneLoader.unity", OpenSceneMode.Single);
        EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + ".unity", OpenSceneMode.Additive);
    }
}