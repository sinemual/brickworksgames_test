using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/PlayerSkillsData", fileName = "PlayerSkillsData")]
public class PlayerSkillsData  : ScriptableObject
{
    public List<SkillData> SkillsData;
    public int PlayerSkillPoints;
    public SkillData BaseSkillData;
    public Dictionary<string, bool> LearningSkillsState;
}