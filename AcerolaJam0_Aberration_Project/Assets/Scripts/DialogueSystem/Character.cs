using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "Character Info", menuName = "ScriptableObject/Dialogue System/Character")]
public class Character : ScriptableObject
{
    public string _name;
    public TMP_ColorGradient _textGradient;
    public DialogueType _dialogueType;
}
