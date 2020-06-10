using UnityEngine;

public class CheckPlayerDetectedCondition : Condition 
{
    public GameObject instance;
    public bool expected;

    private Script_EnemyPerception script_EnemyPerception;

    public override bool Check()
    {
        script_EnemyPerception = instance.GetComponent<Script_EnemyPerception>();

        if (script_EnemyPerception == null)
        {
            throw new UnityException("There is no Script_EnemyPerception component attached to the selected object");
        }

        return script_EnemyPerception.m_PlayerDetected == expected;
    }

    public override Condition Clone()
    {
        CheckPlayerDetectedCondition clone = ScriptableObject.CreateInstance<CheckPlayerDetectedCondition>();
        clone.instance = this.instance;
        clone.expected = this.expected;

        return clone;
    }
}
