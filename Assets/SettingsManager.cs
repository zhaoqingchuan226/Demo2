using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum GameSpeed
{
    S05,
    S1,
    S2,
    S4,
    // S8,
    // S16

}
public class SettingsManager : MonoSingleton<SettingsManager>
{
    public GameSpeed gameSpeed = GameSpeed.S1;
    public TextMeshProUGUI gameSpeed_tmp;
    public GameObject settingPanel;
    public TextMeshProUGUI beginAni_tmp;
    public TextMeshProUGUI guide_tmp;
    public GameObject OpenAni;
    public GameObject OpenGuide;
    void Start()
    {
        gameSpeed_tmp.text = "1x";
    }
    void HideSpecialButton()
    {
        if (Mechanism.Instance.playState != PlayState.MainMenu)
        {
            OpenAni.SetActive(false);
            OpenGuide.SetActive(false);
        }
        else
        {
            OpenAni.SetActive(true);
            OpenGuide.SetActive(true);
        }
    }
    public void OnClickSettingButton()
    {
        if (settingPanel.activeInHierarchy)
        {
            settingPanel.SetActive(false);
        }
        else
        {
            settingPanel.SetActive(true);
            HideSpecialButton();
        }
    }
    public void OnClickChangeSpeedButton()
    {
        if (gameSpeed == GameSpeed.S4)
        {
            gameSpeed = GameSpeed.S05;
        }
        else
        {
            gameSpeed++;
        }

        switch (gameSpeed)
        {
            case GameSpeed.S05:
                Time.timeScale = 0.5f;
                gameSpeed_tmp.text = "0.5x";
                break;
            case GameSpeed.S1:
                Time.timeScale = 1f;
                gameSpeed_tmp.text = "1x";
                break;
            case GameSpeed.S2:
                Time.timeScale = 2f;
                gameSpeed_tmp.text = "2x";
                break;
            case GameSpeed.S4:
                Time.timeScale = 4f;
                gameSpeed_tmp.text = "4x";
                break;
            // case GameSpeed.S8:
            //     Time.timeScale = 8f;
            //     gameSpeed_tmp.text = "8x";
            //     break;
            // case GameSpeed.S16:
            //     Time.timeScale = 16f;
            //     gameSpeed_tmp.text = "16x";
            //     break;
            // default:
            //     break;
        }

    }

    public void OnClickBeginAniButton()
    {
        if (Mechanism.Instance.playState != PlayState.MainMenu)
        {
            return;
        }

        if (StoryManager.Instance.O_C_BeginAni())
        {
            beginAni_tmp.text = "开启";
        }
        else
        {
            beginAni_tmp.text = "关闭";
        }
    }

    public void OnClickGuideButton()
    {
        // Debug.Log(Mechanism.Instance.playState.ToString());
        if (Mechanism.Instance.playState != PlayState.MainMenu)
        {
            return;
        }

        if (TeachManager.Instance.O_C_Guide())
        {
            guide_tmp.text = "开启";
        }
        else
        {
            guide_tmp.text = "关闭";
        }
    }
}
