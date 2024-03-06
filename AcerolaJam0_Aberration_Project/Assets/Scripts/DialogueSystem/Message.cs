using UnityEngine;

[System.Serializable]
public struct Message
{
    public Character _character;
    [TextArea(3, 10)]
    public string _message;
    public bool _instantText;
}
