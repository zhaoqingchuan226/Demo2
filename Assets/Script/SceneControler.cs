using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;
public class SceneControler : MonoSingleton<SceneControler>
{
    //MainMenuUI
    public GameObject mainMenu;
    public List<GameObject> FadeUIs = new List<GameObject>();

    public GameObject blackPlastic;//黑色塑料袋
    float originalpha;//此项和上一项均用于没有开场动画时播放塑料袋渐隐的动画

    public GameObject Black0;
    Vector2 Black0OriPos;
    public GameObject Black1;
    Vector2 Black1OriPos;
    float timer;
    public float aniTime = 2f;

    bool isClickAnyButton = false;//是否点击了主界面的任意按钮
    void Awake()
    {
        mainMenu.SetActive(true);
    }
    void Start()
    {
        Black0OriPos = Black0.GetComponent<RectTransform>().anchoredPosition;
        Black1OriPos = Black1.GetComponent<RectTransform>().anchoredPosition;
        originalpha = blackPlastic.GetComponent<MeshRenderer>().material.color.a;
    }


    public void OnClickButton(int sceneID)
    {
        PlayerData.Instance.SavePlayerData();
        StartCoroutine(LoadSceneDelay(sceneID));

    }

    IEnumerator LoadSceneDelay(int sceneID)
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(sceneID);
    }

    public void OnClickExitButton()
    {
        PlayerData.Instance.SavePlayerData();
        StartCoroutine(LoadExit());
    }

    IEnumerator LoadExit()
    {
        yield return new WaitForSeconds(0.3f);
#if UNITY_EDITOR

        EditorApplication.isPlaying = false;
#else

        Application.Quit();
#endif
    }

    public void OnClickStart()
    {
        if (!isClickAnyButton)
        {
            StartCoroutine(MainMenuFade());
            isClickAnyButton = true;
        }


    }
    IEnumerator MainMenuFade()
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > aniTime)
            {
                timer = 0;
                mainMenu.SetActive(false);
                if (StoryManager.Instance.isBeginAni)
                {
                    StoryManager.Instance.BeginAni();
                }
                else
                {
                    Mechanism.Instance.PlayChess(false);
                }

                break;
            }
            float factor = timer / aniTime;
            RectTransform rt0 = Black0.GetComponent<RectTransform>();
            RectTransform rt1 = Black1.GetComponent<RectTransform>();
            rt0.anchoredPosition = new Vector2(rt0.anchoredPosition.x, Mathf.Lerp(Black0OriPos.y, Black0OriPos.y + rt0.sizeDelta.y, factor));
            rt1.anchoredPosition = new Vector2(rt1.anchoredPosition.x, Mathf.Lerp(Black1OriPos.y, Black1OriPos.y - rt1.sizeDelta.y, factor));

            foreach (var UI in FadeUIs)
            {
                UI.TryGetComponent<Image>(out Image image);
                UI.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI tmp);
                if (image != null)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 1 - factor);
                }
                if (tmp != null)
                {
                    tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1 - factor);
                }
            }

            if (!StoryManager.Instance.isBeginAni)
            {
                Material mat = blackPlastic.GetComponent<MeshRenderer>().material;
                Color color = new Color(mat.color.r, mat.color.g, mat.color.b, originalpha * (1 - factor));
                mat.SetColor("_Color", color);
            }

            yield return null;
        }
    }
}
