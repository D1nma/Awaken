using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocherOiseau : MonoBehaviour
{
    public GameObject DialogueRocher;
    public static bool rocherDialogue;
    void Start()
    {
        DialogueRocher.SetActive(false);
        rocherDialogue = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (rocherDialogue)
        {
            DialogueRocher.SetActive(true);
        }
    }
}
