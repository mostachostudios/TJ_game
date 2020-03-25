using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Trigger")]
public class Trigger : ScriptableObject
{
    public Condition[] conditions = new Condition[0];

    public bool Check()
    {
        foreach(Condition condition in conditions)
        {
            if(!condition.Check())
            {
                return false;
            }
        }
        return true;
    }

    public Trigger Clone()
    {
        Trigger clone = Instantiate(this);
        clone.name = this.name;

        for (int i = 0; i < conditions.Length; i++)
        {
            clone.conditions[i] = conditions[i].Clone();
        }
        return clone;
    }
}
