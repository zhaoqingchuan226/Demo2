using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SceneTest : MonoSingleton<SceneTest>
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickButton(int sceneID)
    {
        PlayerData.Instance.SavePlayerData();
        SceneManager.LoadScene(sceneID);
    }

    public void OnClickExitButton()
    {
#if UNITY_EDITOR
        PlayerData.Instance.SavePlayerData();
        EditorApplication.isPlaying = false;
#else
 PlayerData.Instance.SavePlayerData();
        Application.Quit();
#endif
    }
}
