using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SkillTree
{
    private readonly List<SkillData> _skillData;
    private readonly SkillTreeScreen _ui;
    private readonly PlayerSkillsData _playerData;

    private SkillData _selectedSkillData;
    private SkillView _selectedSkillView;

    public SkillTree(PlayerSkillsData playerData, List<SkillData> skillData, SkillTreeScreen ui)
    {
        _skillData = skillData;
        _ui = ui;
        _playerData = playerData;
        _ui.Inject(this);

        _playerData.LearningSkillsState = new Dictionary<string, bool>();
        foreach (var idx in _skillData)
            _playerData.LearningSkillsState.Add(idx.Id, false);

        _ui.ChangePlayerSkillPointText(playerData.PlayerSkillPoints.ToString());
        MapViewData();
        InitBaseSkill();
    }

    private void MapViewData()
    {
        var skillViews = _ui.SkillViews;

        foreach (var idx in _skillData)
        {
            var skillView = skillViews.First(x => x.SkillData == idx);
            skillView.Init(idx.Sprite, idx.SkillName, idx.SkillPoints.ToString());
            skillView.ViewSelected += OnViewSelected;
        }
    }

    private void InitBaseSkill()
    {
        _playerData.LearningSkillsState[_playerData.BaseSkillData.Id] = true;
        foreach (var view in _ui.SkillViews)
            if (view.SkillData == _playerData.BaseSkillData)
                view.SetLearnState(true);
    }

    private void OnViewSelected(SkillView skillView, SkillData data)
    {
        _selectedSkillData = _skillData.First(x => x.Id == data.Id);
        _selectedSkillView = skillView;

        foreach (var view in _ui.SkillViews)
            view.SetSelectSelect(false);

        skillView.SetSelectSelect(true);

        UpdateViewStates();
    }

    private bool IsPlayerHasSkillPoints() => _playerData.PlayerSkillPoints >= _selectedSkillData.SkillPoints;

    public void EarnSkillPoint()
    {
        ChangeSkillPoints(1);
        UpdateViewStates();
    }

    private void UpdateViewStates()
    {
        _ui.SetForgetSkillButtonState(IsSkillCanBeForgot());
        bool learnConditions = !_playerData.LearningSkillsState[_selectedSkillData.Id] && IsPlayerHasSkillPoints() && IsSkillCanBeLearned();
        _ui.SetLearnSkillButtonState(learnConditions);
    }

    public void LearnSkill()
    {
        _playerData.LearningSkillsState[_selectedSkillData.Id] = true;
        ChangeSkillPoints(-_selectedSkillData.SkillPoints);
        _selectedSkillView.SetLearnState(true);
        UpdateViewStates();
    }

    public void ForgetSkill()
    {
        _playerData.LearningSkillsState[_selectedSkillData.Id] = false;
        ChangeSkillPoints(_selectedSkillData.SkillPoints);
        _selectedSkillView.SetLearnState(false);
        UpdateViewStates();
    }

    public void ForgetAllSkills()
    {
        Dictionary<string, bool> temp = new Dictionary<string, bool>();
        foreach (var skill in _playerData.LearningSkillsState)
            temp.Add(skill.Key, false);

        foreach (var idx in temp)
        {
            if(idx.Key == _playerData.BaseSkillData.Id)
                continue;
            
            _playerData.LearningSkillsState[idx.Key] = idx.Value;
        }

        foreach (var skillData in _skillData)
            if (_playerData.LearningSkillsState[skillData.Id])
                ChangeSkillPoints(skillData.SkillPoints);

        foreach (var skillView in _ui.SkillViews)
            skillView.SetLearnState(_playerData.LearningSkillsState[skillView.SkillData.Id]);
    }

    private void ChangeSkillPoints(int value)
    {
        _playerData.PlayerSkillPoints += value;
        _ui.ChangePlayerSkillPointText(_playerData.PlayerSkillPoints.ToString());
    }

    private bool IsSkillCanBeLearned()
    {
        foreach (var neighbour in _selectedSkillData.NeighbourSkills)
        {
            if (_playerData.LearningSkillsState[neighbour.Id] && IsThisSkillHasPathToBase(neighbour))
                return true;
        }

        return false;
    }

    private bool IsSkillCanBeForgot()
    {
        if (!_playerData.LearningSkillsState[_selectedSkillData.Id])
            return false;

        foreach (var neighbour in _selectedSkillData.NeighbourSkills)
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
            
            if (current == _playerData.BaseSkillData)
                return true;
            
            foreach (var neighbor in current.NeighbourSkills)
            {
                if (!_playerData.LearningSkillsState[neighbor.Id])
                    continue;

                if (visited.Contains(neighbor))
                    continue;
                
                if (neighbor == _selectedSkillData)
                    continue;

                queue.Enqueue(neighbor);
                if (neighbor == _playerData.BaseSkillData)
                    return true;
            }
        }

        return false;
    }
}