using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceMonitoring : MonoBehaviour
{
    public static ExperienceMonitoring instance;
    public float Expirience { private set; get; }

    [Header("Text")]
    [SerializeField] TMPro.TMP_Text textEXP;

    [Header("Details")]
    [SerializeField] Skill[] allSkillsAnvaible;
    [SerializeField] SkillDetails prefabSkill;
    [SerializeField] Transform gridSkills;
    public List<AnvaibleSkill> allMySkills;

    // Prototype Simple Mechanics EXP
    public void AddEXP(int exp) => Expirience += exp;
    public void SubstractEXP(int exp) => Expirience -= exp;

    private void Awake()
    {
        instance = this;
        if (allMySkills == null)
            allMySkills = new List<AnvaibleSkill>();

        for (int i = 0; i < allSkillsAnvaible.Length; i++)
        {
            Instantiate(prefabSkill, gridSkills).Setup(allSkillsAnvaible[i]);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            AddEXP(100);

        textEXP.text = Expirience.ToString();
    }

    public bool TryBuySkill(Skill skill)
    {
        if (Expirience >= skill.costExp)
        {
            // Succes
            SubstractEXP(skill.costExp);
            AddSkill(skill);

            return true;
        }else
        {
            // Failed

            return false;
        }
    }
    public bool TryUpdateSkill(AnvaibleSkill anvaibleSkill = null, Skill skill = null, Skill.Tier tier = Skill.Tier.Untrained)
    {
        if(anvaibleSkill != null)
        {
            if ((int)anvaibleSkill.tier >= 3)
                return false;
            if (Expirience < anvaibleSkill.skill.costExp)
                return false;

            UpdateSkill(anvaibleSkill, tier);
            SubstractEXP(anvaibleSkill.skill.costExp);
        }
        if(skill != null)
        {
            AnvaibleSkill as2 = GetAnvaibleSkill(skill);

            if ((int)as2.tier >= 3)
                return false;
            if (Expirience < as2.skill.costExp)
                return false;

            UpdateSkill(as2, tier);
            SubstractEXP(skill.costExp);
        }
        if (skill == null && anvaibleSkill == null)
            return false;

        return true;
    }
    private void AddSkill(Skill skill)
    {
        allMySkills.Add(new AnvaibleSkill(skill, Skill.Tier.Untrained));     
    }
    private void UpdateSkill(AnvaibleSkill anvaibleSkill, Skill.Tier tier)
    {
        anvaibleSkill.tier = tier;
    }

    public bool HaveSkill(Skill skill, Skill.Tier tier)
    {
        foreach (AnvaibleSkill item in allMySkills)
        {
            if (item.skill == skill && item.tier == tier)
                return true;
        }

        return false;
    }
    public AnvaibleSkill GetAnvaibleSkill(Skill skill)
    {
        foreach (AnvaibleSkill item in allMySkills)
        {
            if (item.skill == skill)
                return item;
        }

        return null;
    }

    private SkillDetails GetSkillDetails(Skill skill)
    {
        foreach (SkillDetails item in gridSkills.GetComponentsInChildren<SkillDetails>())
        {
            if (item.Index == skill)
                return item;
        }

        return null;
    }

    [System.Serializable]
    public class AnvaibleSkill
    {
        public AnvaibleSkill(Skill skill, Skill.Tier tier)
        {
            this.skill = skill;
            this.tier = tier;
        }

        public Skill skill;
        public Skill.Tier tier;     
    }
}
