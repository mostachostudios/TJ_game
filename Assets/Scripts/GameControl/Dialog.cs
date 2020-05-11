using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : ScriptableObject
{
    public enum DialogPosition
    {
        UP_LEFT,
        DOWN_LEFT,
        DOWN_RIGHT
    }

    public string m_Text;
    public Sprite m_Avatar;
    public DialogPosition m_DialogPosition;
}
