using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndCondition : Condition
{
    public Condition[] conditions = new Condition[0];

    public override bool Check()
    {
        foreach (Condition condition in conditions)
        {
            if (condition.Check() == false)
            {
                return false;
            }
        }
        return true;
    }
    public override Condition Clone()
    {
        AndCondition clone = new AndCondition();

        for (int i = 0; i < conditions.Length; i++)
        {
            clone.conditions[i] = conditions[i].Clone();
        }

        return clone;
    }
}
