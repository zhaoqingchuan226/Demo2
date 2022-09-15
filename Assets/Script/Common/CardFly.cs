using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFly : MonoBehaviour
{

    float timer;

    public IEnumerator Fly(Vector3 startworldPos, Vector3 deckWorldPos1, Vector3 deckWorldPos2, float delayTime, float flyTime, Vector2 dir)//传入的参数是卡组UI的世界坐标
    {
        while (true)
        {

            if (timer > flyTime + delayTime)
            {
                timer = 0;
                Destroy(this.gameObject);
                yield break;
            }
            timer += Time.deltaTime;
            float factor = 0;
            // this.transform.position = Camera.main.WorldToScreenPoint(deckWorldPos1);
            this.transform.position = deckWorldPos1;

            if (timer <= delayTime)
            {

            }
            else if (timer > delayTime && timer <= (flyTime *0.7f + delayTime))//往上运动
            {
                factor = (timer-delayTime) / (flyTime *0.7f);

                this.transform.position = Vector3.Lerp(startworldPos,deckWorldPos1, factor);
            }
            else if (timer > (flyTime *0.7f + delayTime))//往牌库运动
            {
                factor = (timer-(flyTime *0.7f + delayTime)) / (flyTime*0.3f);

                this.transform.position = Vector3.Lerp(deckWorldPos1, deckWorldPos2, factor);
            }

            yield return null;
        }
    }



    /*  if (timer_Des > scaleTime)
      {
          timer_Des = 0;
          isBeDesed = false;
          yield break;
      }
      isBeDesed = true;
      timer_Des += Time.deltaTime;
      float lerpFactor = timer_Des / scaleTime;
      tmp.color = Color.Lerp(originTmpColor, Color.red, lerpFactor);

      yield return null;
      */
}
