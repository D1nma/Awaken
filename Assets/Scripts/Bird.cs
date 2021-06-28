using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{

    Animator animator;
    
    public static bool vol=false,picore;
    bool doOnce;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("picore", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (vol)
        {
            if (!doOnce)
            {
                animator.SetTrigger("vol");
                doOnce = false;
            }
            animator.SetTrigger("vol");
            vol = false;
        }
        if (picore)
        {
            animator.SetBool("picore", true);
        }
    }
   
}
