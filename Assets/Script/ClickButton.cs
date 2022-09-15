using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;


public class ClickButton : MonoBehaviour, IPointerDownHandler
{
    Vector3 defaultlocalPos;
    public float downDeep = 0.01f;
    public bool isPressed = false;
    public TextMeshPro font;
    public Color PressedFontColor;
    Color originFontColor;
    // public Button
    public List<GameObject> EffectObjs = new List<GameObject>();

    public List<string> CommandIDs = new List<string>();

    float timer = 0;//ExecuteButton自动复位专用

    Animator animator;//闸门专用

    void Awake()
    {
        // if(font.text=="开始")
        // {
        //     Debug.Log("找的就是你");
        // }
        defaultlocalPos = transform.localPosition;
        originFontColor = font.color;
        ButtonStatus();
        if (font.text == "开始" || font.text == "继续" || font.text == "牌库" ||
        font.text == "选择卡牌" || font.text == "选择遗物" || font.text == "岗位升级" || font.text == "跳过" || font.text == "新的一周")
        {
            animator = this.gameObject.GetComponentInParent<Animator>();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        ExecuteCommand();
    }

    void ExecuteCommand()
    {
        foreach (var CommandID in CommandIDs)
        {
            string[] elements = CommandID.Split("|", System.StringSplitOptions.RemoveEmptyEntries);
            string funcName = elements[0];
            string objs = elements[1];


            switch (funcName)
            {
                case "JustPress":
                    JustPress();
                    break;
                case "OnClickStart":
                    if (TeachManager.Instance.currentEvent == "牌库介绍")
                    {
                        return;
                    }
                    if (LibraryManager.Instance.isOpen)//开Library的时候锁开始键
                    {
                        return;
                    }
                    OnClickStart();
                    break;
                case "O_C_Library":
                    O_C_Library();
                    break;
                case "OnClickContinue":
                    OnClickContinue();
                    break;
                case "OnClickExecute":
                    OnClickExecute(EffectObjs);
                    break;
                case "OnClickSkip":
                    OnClickSkip(EffectObjs);
                    break;
                default:
                    break;

            }
        }
    }

    //CommandID 0 单纯按下去
    void JustPress()
    {
        if (!isPressed)
        {
            if (font.text == "开始" || font.text == "继续" || font.text == "牌库" ||
            font.text == "选择卡牌" || font.text == "选择遗物" || font.text == "岗位升级" || font.text == "跳过" || font.text == "新的一周")
            {
                isPressed = true;
                GateStatus();
            }
            else
            {
                isPressed = true;
                ButtonStatus();
            }
            AudioManager.Instance.PlayClip("button1");
        }
        TeachManager.Instance.SetGuide(this.gameObject, false);
        // if (font.text == "牌库")
        // {
        //     Debug.Log(this.gameObject.name);
        // }
    }


    public void ButtonRecover()
    {
        if (isPressed)
        {
            if (font.text == "开始" || font.text == "继续" || font.text == "牌库" ||
             font.text == "选择卡牌" || font.text == "选择遗物" || font.text == "岗位升级" || font.text == "跳过" || font.text == "新的一周")
            {
                isPressed = false;
                GateStatus();
            }
            else
            {
                isPressed = false;
                ButtonStatus();
            }
        }

    }
    // void ShellStatus()//根据isPressed来决定button的状态
    // {
    //     if (isPressed)
    //     {
    //         animator.SetTrigger("On");
    //         font.color = PressedFontColor;
    //     }
    //     else
    //     {
    //         animator.SetTrigger("Off");
    //         font.color = originFontColor;
    //     }
    // }
    void ButtonStatus()//根据isPressed来决定button的状态
    {
        if (isPressed)
        {
            transform.localPosition = defaultlocalPos - new Vector3(0, downDeep, 0);
            font.color = PressedFontColor;
        }
        else
        {
            transform.localPosition = defaultlocalPos;
            font.color = originFontColor;
        }
    }

    void GateStatus()//控制闸门的开关
    {
        if (isPressed)
        {
            animator.SetTrigger("On");
            font.color = PressedFontColor;
        }
        else
        {
            animator.SetTrigger("Off");
            font.color = originFontColor;
        }
    }


    void OnClickStart()
    {
        if (!isPressed && font.text == "开始" && !LibraryManager.Instance.isDeleteMode && !Mechanism.Instance.isAISendShit)
        {
            Mechanism.Instance.OnClickStart();
            JustPress();
        }
    }

    public void Start_2_Continue()//按钮从开始变成继续，或者从继续变成开始
    {
        if (font.text == "开始")
        {
            font.text = "继续";

        }
        else if (font.text == "继续")
        {
            font.text = "开始";
        }
        ButtonRecover();
    }

    public void OnClickContinue()//点击继续
    {
        if (!isPressed && font.text == "继续" && !LibraryManager.Instance.isDeleteMode && Mechanism.Instance.chooseTimes == 0)
        {
            Mechanism.Instance.OnClickContinue();
            JustPress();
        }
    }

    public void OnClickExecute(List<GameObject> objs)
    {

        if (font.text == "选择卡牌")
        {
            Mechanism.Instance.OnClickChooseCardButton();
            JustPress();
            StartCoroutine(Execute_Skip_Recover(0.1f, font.text));
            // font.text = "选择遗物";
        }
        else if (font.text == "选择遗物")
        {
            JustPress();
            StartCoroutine(Execute_Skip_Recover(0.1f, font.text));
        }
        else if (font.text == "岗位升级")
        {
            WeekendMeetingAll.Instance.weekendPhase = weekendPhase.Else;
            Mechanism.Instance.OnClickPostUpgradeButton();
            JustPress();
            foreach (var obj in objs)
            {
                ClickButton cb = obj.GetComponent<ClickButton>();
                cb.font.text = "新的一周";
                TeachManager.Instance.SetGuide(obj, true);

            }
            StartCoroutine(Execute_Skip_Recover(0.1f, font.text));
        }
    }

    public void OnClickSkip(List<GameObject> objs)
    {
        if (TeachManager.Instance.isFirstUpgrade || TeachManager.Instance.isFirstUpgrade)
        {
            return;
        }
        if (!isPressed && font.text == "新的一周")
        {
            Mechanism.Instance.OnClickNextWeekButton();
            JustPress();
            StartCoroutine(Execute_Skip_Recover(Mechanism.Instance.PlayerDataChangeTime + 0.8f, font.text));
        }
        else if (font.text == "跳过")
        {
            JustPress();
            StartCoroutine(Execute_Skip_Recover(0.1f, font.text));
        }
        foreach (var obj in objs)
        {
            ClickButton cb = obj.GetComponent<ClickButton>();
        }
    }



    IEnumerator Execute_Skip_Recover(float interval, string fontTextNow)
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > interval)
            {
                timer = 0;

                if (font.text == "选择卡牌")
                {
                    ButtonRecover();
                    //之后要插入选择遗物就使用下面两行代码
                    // font.text = "选择遗物";
                    // TeachManager.Instance.SetGuide(this.gameObject, true);

                    font.text = "岗位升级";


                    if (PlayerData.Instance.postLevel < 5)
                    {
                        if (PlayerData.Instance.workAbility >= Mechanism.Instance.postUpgradeNeeds[PlayerData.Instance.postLevel - 1])
                        {
                            TeachManager.Instance.SetGuide(this.gameObject, true);
                        }
                        else
                        {
                            foreach (var obj in EffectObjs)
                            {
                                TeachManager.Instance.SetGuide(obj, true);
                            }
                        }
                    }
                    else
                    {
                        foreach (var obj in EffectObjs)
                        {
                            TeachManager.Instance.SetGuide(obj, true);
                        }
                    }
                }
                else if (font.text == "选择遗物")
                {
                    ButtonRecover();
                    //之后要插入选择遗物就使用下面两行代码z
                    // font.text = "岗位升级";
                    // if (PlayerData.Instance.postLevel < 5)
                    // {
                    //     if (PlayerData.Instance.workAbility >= Mechanism.Instance.postUpgradeNeeds[PlayerData.Instance.postLevel - 1])
                    //     {
                    //         TeachManager.Instance.SetGuide(this.gameObject, true);
                    //     }
                    //     else
                    //     {
                    //         foreach (var obj in EffectObjs)
                    //         {
                    //             TeachManager.Instance.SetGuide(obj, true);
                    //         }
                    //     }
                    // }
                    // else
                    // {
                    //     foreach (var obj in EffectObjs)
                    //     {
                    //         TeachManager.Instance.SetGuide(obj, true);
                    //     }
                    // }
                }
                else if (font.text == "岗位升级")
                {

                    ButtonRecover();
                }
                else if (font.text == "跳过")
                {
                    foreach (var obj in EffectObjs)
                    {
                        ClickButton cb = obj.GetComponent<ClickButton>();
                        // if (cb.font.text == "选择卡牌")
                        // {
                        //     cb.font.text = "选择遗物";
                        //     ButtonRecover();
                        // }
                        /*else*/
                        if (cb.font.text == "选择卡牌")
                        {
                            WeekendMeetingAll.Instance.weekendPhase = weekendPhase.PostUpgrade;
                            cb.font.text = "岗位升级";
                            if (PlayerData.Instance.postLevel < 5)
                            {
                                if (PlayerData.Instance.workAbility >= Mechanism.Instance.postUpgradeNeeds[PlayerData.Instance.postLevel - 1])
                                {
                                    TeachManager.Instance.SetGuide(obj, true);
                                }
                                else
                                {
                                    TeachManager.Instance.SetGuide(obj, false);
                                    TeachManager.Instance.SetGuide(gameObject, true);
                                }
                            }
                            else
                            {
                                TeachManager.Instance.SetGuide(gameObject, true);
                            }
                            ButtonRecover();
                        }
                        else if (cb.font.text == "岗位升级")
                        {
                            WeekendMeetingAll.Instance.weekendPhase = weekendPhase.Else;
                            font.text = "新的一周";
                            if (TeachManager.Instance.isFirstSalary && Mechanism.Instance.week % 4 == 0)
                            {
                                TeachManager.Instance.TeachEventTrigger("工资结算介绍");
                                TeachManager.Instance.isFirstSalary = false;
                            }

                            ButtonRecover();
                            TeachManager.Instance.SetGuide(obj, false);
                            TeachManager.Instance.SetGuide(this.gameObject, true);
                        }
                    }
                }
                else if (font.text == "新的一周")
                {
                    ButtonRecover();
                }




                yield break;
            }
            yield return null;
        }
    }

    // public void OpenLibrary()
    // {
    //     StartCoroutine(Mechanism.Instance.ChessBoardMove());
    //     LibraryManager.Instance.isOpen = true;
    //     LibraryManager.Instance.libraryPanel.SetActive(true);
    //     StartCoroutine(LibraryManager.Instance.MoveLibraryPanel());
    //     LibraryManager.Instance.UpdateLibrary();
    //     FishControler.Instance.Open_Close_All();
    //     FishControler.Instance.Mouth();
    //     CameraManager.Instance.SetVirtualCam("LibraryCam");
    // }
    // public void CloseLibrary()
    // {
    //     LibraryManager.Instance.isOpen = false;

    //     StartCoroutine(LibraryManager.Instance.MoveLibraryPanel(true));
    //     StartCoroutine(Mechanism.Instance.ChessBoardMove(true));
    //     // LibraryManager.Instance.UpdateLibrary();
    //     foreach (var card in PlayerData.Instance.playerCards)
    //     {
    //         card.isNew = false;
    //     }

    //     FishControler.Instance.Open_Close_All();
    //     FishControler.Instance.Mouth();
    //     CameraManager.Instance.SetVirtualCam("BlackCam");
    //     TeachManager.Instance.SetGuide(this.gameObject, false);
    // }
    IEnumerator DelayIntro()
    {
        yield return new WaitForSeconds(1.5f);
        TeachManager.Instance.TeachEventTrigger("卡牌介绍");
        yield break;
    }

    void O_C_Library()
    {
        if (!LibraryManager.Instance.isDeleteMode)
        {
            if (Mechanism.Instance.playState == PlayState.Chess &&
            (Mechanism.Instance.phase == Phase.Start || Mechanism.Instance.phase == Phase.HolidayStore
            || Mechanism.Instance.phase == Phase.TripletReward) || Mechanism.Instance.phase == Phase.WeekendMeeting)
            {
                if (!isPressed)
                {
                    if (TeachManager.Instance.isFirstLibrary)
                    {
                        StartCoroutine(DelayIntro());
                    }

                    // LibraryManager.Instance.gameObject.transform.Find("ScrollView").gameObject.SetActive(true);
                    JustPress();
                    LibraryManager.Instance.OpenLibrary();
                }
                else
                {
                    // LibraryManager.Instance.gameObject.transform.Find("ScrollView").gameObject.SetActive(false);
                    ButtonRecover();
                    LibraryManager.Instance.CloseLibrary();

                    if (TeachManager.Instance.isFirstLibrary)
                    {
                        TeachManager.Instance.isFirstLibrary = false;
                        TeachManager.Instance.currentEvent = null;//是为了让之前不能用的开始键现在可以用
                        TeachManager.Instance.TeachEventTrigger_Delay("开始介绍",1.1f);
                        //退出时触发开始按钮介绍
                    }
                }
            }
        }
    }

}
