
using System;

public class Skill
{
    public readonly SkillData Data;
    public readonly SkillView View;

    private SkillTree _skillTree;

    public Action<Skill> SelectSkill;

    public Skill(SkillData skillData, SkillView view)
    {
        Data = skillData;
        View = view;

        View.Init(Data.Sprite, Data.SkillName, Data.SkillPoints.ToString());
        View.ViewSelected += OnViewSelected;
    }

    private void OnViewSelected() => SelectSkill.Invoke(this);
}