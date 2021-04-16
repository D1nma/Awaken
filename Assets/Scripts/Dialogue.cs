using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct sentence
{
    [TextArea(3, 15)]
    public string[] sentences;
    public bool next;
}
[System.Serializable]
public class Dialogue
{
    public string name,name2;
    public sentence[] sentences;
    public bool isQuestion = false;
}
