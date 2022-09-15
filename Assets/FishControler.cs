using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishControler : MonoSingleton<FishControler>
{
    public GameObject door_up;
    public GameObject door_down;
    public GameObject fish;
    [HideInInspector] public Animator ac_door_up;
    [HideInInspector] public Animator ac_door_down;
    [HideInInspector] public Animator ac_fish;
    [HideInInspector] public bool isOpen = false;//门开了吗
    [HideInInspector] public bool isMouthOpen = false;//嘴巴开了吗
    // Start is called before the first frame update
    void Awake()
    {
        ac_door_up = door_up.GetComponent<Animator>();
        ac_door_down = door_down.GetComponent<Animator>();
        ac_fish = fish.GetComponent<Animator>();
    }
    // void Start()
    // {

    // }

    // private void Update()
    // {
    //     // if (Input.GetKeyDown(KeyCode.J))
    //     // {

    //     // }



    //     // if (Input.GetKeyDown(KeyCode.F))
    //     // {
    //     //     Debug.Log("1");

    //     // }
    //     // if (Input.GetKeyDown(KeyCode.G))
    //     // {
    //     //     Mouth();
    //     // }
    // }

    public void Open_Close_All()
    {
        if (!isOpen)
        {
            O_C_Door("up", true);
            O_C_Door("down", true);
            FishMove(true);
            isOpen = true;
        }
        else
        {
            O_C_Door("up", false);
            O_C_Door("down", false);
            FishMove(false);
            isOpen = false;
        }
    }

    void FishMove(bool b)
    {
        if (b)
        {
            StartCoroutine(DelayMoveUp());
        }
        else
        {
            ac_fish.SetTrigger("MoveDown");
        }
    }
    IEnumerator DelayMoveUp()
    {
        yield return new WaitForSeconds(0.1f);
        ac_fish.SetTrigger("MoveUp");
    }
    IEnumerator DelayDoorClose(string s)
    {
        yield return new WaitForSeconds(0.1f);
        if (s == "up")
        {
            ac_door_up.SetTrigger("close");
        }
        else if (s == "down")
        {
            ac_door_down.SetTrigger("close");
        }
    }

    void O_C_Door(string s, bool b)
    {
        if (s == "up")
        {
            if (b)
            {
                ac_door_up.SetTrigger("open");
            }
            else
            {
                StartCoroutine(DelayDoorClose("up"));
            }

        }
        else if (s == "down")
        {
            if (b)
            {
                ac_door_down.SetTrigger("open");
            }
            else
            {
                StartCoroutine(DelayDoorClose("down"));
            }

        }
    }
    public void Mouth()
    {
        if (!isMouthOpen)
        {
            isMouthOpen = true;
            ac_fish.SetTrigger("OpenMouth");
        }
        else
        {
            isMouthOpen = false;
            ac_fish.SetTrigger("CloseMouth");
        }

    }
    // public void EnforceMouth(bool b)
    // {
    //     if (b)
    //     {
    //         isMouthOpen = true;
    //         ac_fish.SetTrigger("OpenMouth");
    //     }
    //     else
    //     {
    //         isMouthOpen = false;
    //         ac_fish.SetTrigger("CloseMouth");
    //     }
    // }

}
