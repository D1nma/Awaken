using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wwise_Alyx : MonoBehaviour
{
    public AK.Wwise.Event footstep,jump,lever,dead;
    public void PlayFootStep()
    {
        footstep.Post(gameObject);
    }
    public void PlayJump()
    {
        jump.Post(gameObject);
    }
    public void PlaySeLever()
    {
        lever.Post(gameObject);
    }
    public void PlayGameOver()
    {
        dead.Post(gameObject);
    }

}
