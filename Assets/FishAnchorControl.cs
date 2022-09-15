using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAnchorControl : MonoSingleton<FishAnchorControl>
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }


    // void Update()
    // {
    //     // if (Input.GetKeyDown(KeyCode.M))
    //     // {
    //     //     TurnRight();
    //     // }
    //     // else if (Input.GetKeyDown(KeyCode.N))
    //     // {
    //     //     TurnLeft();
    //     // }
    // }


    public void TurnLeft()
    {
        animator.SetTrigger("left");
        LibraryManager.Instance.TurnPage(true);
    }

    public void TurnRight()
    {
        animator.SetTrigger("right");
        LibraryManager.Instance.TurnPage(false);
    }
}
