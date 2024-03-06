using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObject/Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    public Message[] messages;
}