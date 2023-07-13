using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Skill : ScriptableObject
{
    public string nameSkill;
    public string descSkill;

    public int costExp;

    public enum Tier
    {
        Untrained, 
        Skilled, 
        Advanced, 
        Nlaster,

        None = 999
    }
}
