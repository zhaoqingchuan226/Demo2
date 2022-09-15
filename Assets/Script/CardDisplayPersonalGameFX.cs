using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//此脚本挂载在card prefab上
//用于将Card中的数据赋予给UI
public partial class CardDisplayPersonalGame : MonoBehaviour
{
    Animator animator;
    ParticleSystem ps;
    public void FX()
    {
        Speak();
        if (card.id == 2 || card.id == 14 || card.id == 15 || card.id == 19 || card.id == 61 || card.id == 72)//扳手区
        {
            animator.SetTrigger("1");//扭动扳手
            if (card.id == 14 || card.id == 15 || card.id == 19 || card.id == 61)
            {
                ps.Stop();
                ps.Play();
            }
            return;
        }
        if (card.id == 28 || card.id == 29 || card.id == 30)//摸鱼区
        {
            animator.SetTrigger("21");//鱼跳跃
            return;
        }
        else if (card.id == 3 || card.id == 4)//啥都没有
        {
            return;
        }
        else if (card.id == 16 || card.id == 22 || card.id == 27 || card.id == 31 || card.id == 32 || card.id == 33
        || card.id == 35 || card.id == 36 || card.id == 37 || card.id == 38 || card.id == 39 || card.id == 40 || card.id == 44
        || card.id == 59 || card.id == 62
        || card.id == 10001 || card.id == 10002 || card.id == 10003 || card.id == 10004 || card.id == 10005)//只播放特效
        {
            ps.Stop();
            ps.Play();
            return;
        }
        else if (card.id == 23)//特效和动画都播放
        {
            ps.Stop();
            ps.Play();
        }

        animator.SetTrigger(card.id.ToString());

    }
    void Speak()
    {
        this.GetComponent<Dialog>().Speak(card.id, "chess");
    }
}
