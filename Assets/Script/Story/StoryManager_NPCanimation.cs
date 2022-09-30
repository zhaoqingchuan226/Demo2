using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StoryManager : MonoSingleton<StoryManager>
{
    //公共
    [Space(10)]
    public ParticleSystem pm;//背面乱飞的粒子
    public List<Light> lights_AI = new List<Light>();
    public Light light_Player;
    Color originColor_light_Player;
    //不同NPC的灯光不同
    public List<Color> lightColors = new List<Color>();

    //非雨
    [Space(10)]
    public List<SkinnedMeshRenderer> sms = new List<SkinnedMeshRenderer>();

    List<Material> Mats_Origin_Overload = new List<Material>();
    public List<Material> Mats_Night_Overload = new List<Material>();
    public GameObject DD_1;//笔仙装置
    public GameObject Tanzhou;//潭州的模型和尚德的原画
    public Transform hand;//手的节点
    public Transform flower;
    public GameObject vase;

    //月雾
    [Space(10)]
    public GameObject connecter;//接口
    public GameObject knife;//画阶刀
    public GameObject stage;//桌面上的台阶

    void O_C_NightMode(bool b)//姐姐的变身,ture则变为夜间模式
    {
        for (var i = 0; i < sms.Count; i++)
        {
            if (b)//变为夜间模式
            {
                sms[i].material = Mats_Night_Overload[i];
            }
            else
            {
                sms[i].material = Mats_Origin_Overload[i];
            }
        }
    }
    //这些物品都在NPC_Objs下
    void O_C_SpecialObj(bool b)
    {
        switch (plotNow.id)
        {
            case 1:
                DD_1.SetActive(b);
                break;
            case 3:
                Tanzhou.SetActive(b);
                break;
            case 4:
                vase.SetActive(b);
                flower.gameObject.SetActive(b);
                O_C_NightMode(b);
                break;
            case 11:
                connecter.SetActive(b);
                break;
            case 12:
                knife.SetActive(b);
                stage.SetActive(b);
                break;
            default:
                break;
        }
    }
    public void SetFlowerParent()//把设置花的parent为手，让手拿走花 //signal用
    {
        flower.SetParent(hand);
    }

    void ChangeLightAndParticle(bool b)
    {
        if (b)
        {
            light_Player.color = lightColors[NPCs_HasFound.IndexOf(plotNow.owner)];
            ParticleSystem.MainModule mainModule = pm.main;
            mainModule.startColor = light_Player.color;
            foreach (var light in lights_AI)
            {
                light.gameObject.SetActive(false);
            }
        }
        else
        {
            light_Player.color = originColor_light_Player;
            ParticleSystem.MainModule mainModule = pm.main;
            mainModule.startColor = light_Player.color;
            foreach (var light in lights_AI)
            {
                light.gameObject.SetActive(true);
            }
        }
    }

}
