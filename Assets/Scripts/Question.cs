using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Choice
{
    [TextArea(2, 5)]
    public string text;
    public Dialogue dialogue;
}

[CreateAssetMenu(fileName = "Nouvelle Question", menuName = "Question")]
public class Question : ScriptableObject
{
    [TextArea(2, 5)]
    public string text;
    public Choice[] choices;
}
