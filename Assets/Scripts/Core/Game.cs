using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private UserInterface _ui;
    [SerializeField] private SharedData _data;

    private void Awake()
    {
        SkillTree skillTree = new SkillTree(_data.playerSkillsData, _ui.skillTreeScreen);
    }
}