using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/SkillData", fileName = "SkillData")]
public class SkillData : ScriptableObject
{
    public string Id;
    public string SkillName;
    public int SkillPoints;
    public Sprite Sprite;
    public List<SkillData> NeighbourSkills;
}