using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrCondition : Condition
{
    public Condition[] conditions = new Condition[0];

    public override bool Check()
    {
        foreach(Condition condition in conditions)
        {
            if(condition.Check())
            {
                return true;
            }
        }
        return false;
    }
    public override Condition Clone()
    {
        OrCondition clone = new OrCondition();

        for(int i = 0; i < conditions.Length; i++)
        {
            clone.conditions[i] = conditions[i].Clone();
        }

        return clone;
    }
}
