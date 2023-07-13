using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillDetails : MonoBehaviour
{
    [Header("Details")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text costText;
    [SerializeField] TMP_Text tierText;

    public Skill Index { get; private set; }
    public Skill.Tier Tier { get; private set; }

    private void Awake()
    {
        Tier = Skill.Tier.None;
    }
    private void Update()
    {
        tierText.text = Tier.ToString();
    }
    public void Setup(Skill index)
    {
        if (this.Index != index)
            this.Index = index;

        nameText.text = index.nameSkill;
        costText.text = index.costExp.ToString();
    }
    public bool TryBuySkill() => ExperienceMonitoring.instance.TryBuySkill(Index);
    public bool TryUpdateSkill() => ExperienceMonitoring.instance.TryUpdateSkill(null, Index, Tier + 1);
    public void InteractWithSkill()
    {
        if((int)Tier == 999)
        {
            if(TryBuySkill())
            {
                Tier = Skill.Tier.Untrained;
            }
        }else if((int)Tier < 3)
        {
            if(TryUpdateSkill())
            {
                Tier = Tier + 1;
            }
        }
    }
}
