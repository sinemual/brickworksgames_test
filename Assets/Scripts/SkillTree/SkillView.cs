using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillView : MonoBehaviour
{
    [SerializeField] private SkillData skillData;
    [SerializeField] private Image skillImage;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillPointsText;

    [Header("StateView")] [SerializeField] private GameObject learnedFrame;
    [SerializeField] private GameObject selectedFrame;
    [SerializeField] private Button selectButton;

    public SkillData SkillData => skillData;

    public Action<SkillView, SkillData> ViewSelected;

    public void Init(Sprite skillSprite, string skillName, string skillPoints)
    {
        learnedFrame.SetActive(false);
        selectedFrame.SetActive(false);
        skillImage.sprite = skillSprite;
        skillNameText.text = $"{skillName}";
        skillPointsText.text = skillPoints;
    }

    private void Start() => selectButton.onClick.AddListener(() => ViewSelected(this, SkillData));

    public void SetSelectSelect(bool isSelected) => selectedFrame.SetActive(isSelected);

    public void SetLearnState(bool isLearned)
    {
        learnedFrame.SetActive(isLearned);
        skillPointsText.gameObject.SetActive(!isLearned);
    }
}