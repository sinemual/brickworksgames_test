using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeScreen : MonoBehaviour
{
    [SerializeField] private List<SkillView> skillViews;
    [SerializeField] private TextMeshProUGUI skillPointsText;

    [SerializeField] private Button earnSkillPointButton;
    [SerializeField] private Button learnSkillButton;
    [SerializeField] private Button forgetSkillButton;
    [SerializeField] private Button forgetAllSkillsButton;

    public List<SkillView> SkillViews => skillViews;

    private SkillTree _skillTree;

    public void Inject(SkillTree skillTree)
    {
        _skillTree = skillTree;
    }

    public void ChangePlayerSkillPointText(string amount) => skillPointsText.text = $"{amount}";

    private void Start()
    {
        earnSkillPointButton.onClick.AddListener(_skillTree.EarnSkillPoint);
        learnSkillButton.onClick.AddListener(_skillTree.LearnSkill);
        forgetSkillButton.onClick.AddListener(_skillTree.ForgetSkill);
        forgetAllSkillsButton.onClick.AddListener(_skillTree.ForgetAllSkills);
    }

    public void SetEarnSkillButtonState(bool state) => earnSkillPointButton.interactable = state;
    public void SetLearnSkillButtonState(bool state) => learnSkillButton.interactable = state;
    public void SetForgetSkillButtonState(bool state) => forgetSkillButton.interactable = state;
    public void SetForgetAllSkillsButtonState(bool state) => forgetAllSkillsButton.interactable = state;
}