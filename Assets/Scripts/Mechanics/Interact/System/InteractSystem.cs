using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractSystem : MonoBehaviour
{
    [Header("Skills Require")]
    public Skill RequireSkill;
    public Skill.Tier RequireTier = Skill.Tier.None;
    public void TryInteract()
    {
        if(RequireSkill != null)
        {
            if ((ExperienceMonitoring.instance.GetAnvaibleSkill(RequireSkill) == null && RequireTier == Skill.Tier.None))
            {
                OnInteract();
                return;
            }
            if ((ExperienceMonitoring.instance.GetAnvaibleSkill(RequireSkill) == null && RequireTier != Skill.Tier.None))
            {
                Debug.Log("Null Anvaible Skill");
                InteractRaycast.Instance.SetTextInfo("You need at least {" + RequireTier.ToString() + "} " +  RequireSkill.nameSkill + " Skill Tier", 4f);
                return;
            }
            Debug.Log("Anvaible Skill: " + (int)ExperienceMonitoring.instance.GetAnvaibleSkill(RequireSkill).tier
                + " Rquire Skill: " +(int) RequireTier);

            if (ExperienceMonitoring.instance.GetAnvaibleSkill(RequireSkill).tier >= RequireTier
                || RequireTier == Skill.Tier.None)
            {
                OnInteract();
                
                return;
            }

            return;
        }

        OnInteract();
        return;
    }

    public abstract void OnInteract();
}
