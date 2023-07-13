using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SkillTree
{
    private readonly SkillTreeScreen _ui;
    private readonly PlayerSkillsData _playerSkillData;

    private Dictionary<string, Skill> _skills;
    private Skill _selected;

    public SkillTree(PlayerSkillsData playerSkillData, SkillTreeScreen ui)
    {
        _ui = ui;
        _playerSkillData = playerSkillData;

        _ui.Inject(this);

        MapDataWithView(_playerSkillData.SkillsData, _ui.SkillViews);
        InitLearningSkillStates();
        LearnBaseSkillOnStart();
       
        _ui.ChangePlayerSkillPointText(playerSkillData.PlayerSkillPoints.ToString());
    }

    private void LearnBaseSkillOnStart()
    {
        _playerSkillData.LearningSkillsState[_playerSkillData.BaseSkillData.Id] = true;
        _skills[_playerSkillData.BaseSkillData.Id].View.SetLearnState(true);
        UpdateLearnStates();
    }

    private void InitLearningSkillStates()
    {
        _playerSkillData.LearningSkillsState = new Dictionary<string, bool>();
        foreach (var idx in _skills.Values)
            _playerSkillData.LearningSkillsState.Add(idx.Data.Id, false);
    }

    private void MapDataWithView(List<SkillData> skillData, List<SkillView> skillViews)
    {
        _skills = new Dictionary<string, Skill>();
        foreach (var data in skillData)
        {
            var skill = new Skill(data, skillViews.First(view => view.SkillId == data.Id));
            skill.SelectSkill += (selected) =>
            {
                _selected = selected;
                UpdateViewStates();
            };
            _skills.Add(data.Id, skill);
        }
    }

    private bool IsPlayerHasSkillPoints(SkillData data) => _playerSkillData.PlayerSkillPoints >= data.SkillPoints;

    public void EarnSkillPoint()
    {
        ChangeSkillPoints(1);
        UpdateViewStates();
    }

    private void UpdateViewStates()
    {
        _ui.SetForgetSkillButtonState(IsSkillCanBeForgot(_selected.Data));

        var skillIsLearned = _playerSkillData.LearningSkillsState[_selected.Data.Id];

        bool learnConditions = !skillIsLearned &&
                               IsPlayerHasSkillPoints(_selected.Data) &&
                               IsSkillCanBeLearned(_selected.Data);

        _ui.SetLearnSkillButtonState(learnConditions);
        _selected.View.SetLearnState(skillIsLearned);

        foreach (var skill in _skills.Values)
            skill.View.SetSelectSelect(skill.View == _selected.View);

        UpdateLearnStates();
    }

    private void UpdateLearnStates()
    {
        foreach (var skill in _playerSkillData.LearningSkillsState)
            _skills[skill.Key].View.SetLearnState(skill.Value); 
    }

    public void LearnSkill()
    {
        _playerSkillData.LearningSkillsState[_selected.Data.Id] = true;
        ChangeSkillPoints(-_selected.Data.SkillPoints);
        UpdateViewStates();
    }

    public void ForgetSkill()
    {
        _playerSkillData.LearningSkillsState[_selected.Data.Id] = false;
        ChangeSkillPoints(_selected.Data.SkillPoints);
        UpdateViewStates();
    }

    public void ForgetAllSkills()
    {
        foreach (var key in _playerSkillData.LearningSkillsState.Keys.ToList())
            _playerSkillData.LearningSkillsState[key] = false;

        _playerSkillData.LearningSkillsState[_playerSkillData.BaseSkillData.Id] = true;

        foreach (var skill in _skills)
            if (_playerSkillData.LearningSkillsState[skill.Value.Data.Id])
                ChangeSkillPoints(skill.Value.Data.SkillPoints);

        UpdateViewStates();
    }

    private void ChangeSkillPoints(int value)
    {
        _playerSkillData.PlayerSkillPoints += value;
        _ui.ChangePlayerSkillPointText(_playerSkillData.PlayerSkillPoints.ToString());
    }

    private bool IsSkillCanBeLearned(SkillData data)
    {
        foreach (var neighbour in data.NeighbourSkills)
        {
            if (_playerSkillData.LearningSkillsState[neighbour.Id] && IsThisSkillHasPathToBase(neighbour))
                return true;
        }

        return false;
    }

    private bool IsSkillCanBeForgot(SkillData data)
    {
        if (!_playerSkillData.LearningSkillsState[data.Id])
            return false;

        foreach (var neighbour in data.NeighbourSkills)
        {
            if (!IsThisSkillHasPathToBase(neighbour))
                return false;
        }

        return true;
    }

    private bool IsThisSkillHasPathToBase(SkillData start)
    {
        List<SkillData> visited = new List<SkillData>();
        Queue<SkillData> queue = new Queue<SkillData>();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            SkillData current = queue.Dequeue();
            visited.Add(current);

            if (current == _playerSkillData.BaseSkillData)
                return true;

            foreach (var neighbor in current.NeighbourSkills)
            {
                if (!_playerSkillData.LearningSkillsState[neighbor.Id])
                    continue;

                if (visited.Contains(neighbor))
                    continue;

                if (neighbor == _selected.Data)
                    continue;

                queue.Enqueue(neighbor);
                if (neighbor == _playerSkillData.BaseSkillData)
                    return true;
            }
        }

        return false;
    }
}