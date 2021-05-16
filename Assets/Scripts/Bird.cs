using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{

    Animator animator;
    
    public static bool vol=false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(vol);
        if (vol)
        {
            animator.SetTrigger("vol");
            vol = false;
        }
    }
   
}
