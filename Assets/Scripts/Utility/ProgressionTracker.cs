using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionTracker : AutoCleanupSingleton<ProgressionTracker>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    #region Achievement Progression

    [System.Serializable]
    public struct AchievementData
    {        
        [SerializeField] public string _achievementName;
        [SerializeField, TextArea()] public string _achievementDescription;
        [SerializeField, Min(0)] public int _achievementID;
        [SerializeField] public Image _icon;
        [SerializeField] public bool _isAchieved;
    }

    public List<AchievementData> _allAchievements = new List<AchievementData>();
    #endregion


    #region Level Progression

    //A list of all the scenes that have been loaded thus far
    //Used to disable any first time only occurences when revisiting an area
    public List<SceneLoader.SceneData> _loadedScenes = new List<SceneLoader.SceneData>();



    public void AddNewLoadedScene(SceneLoader.SceneData data)
    {
        _loadedScenes.Add(data);
    }

    public bool HasSceneBeenLoadedBefore(SceneLoader.SceneData data)
    {
        return _loadedScenes.Contains(data);
    }
    #endregion


    #region Quest Progression

    [System.Serializable]
    public struct QuestInfo
    {
        public Quest _quest;
        public int _currentStage;
        public bool _isComplete;
        public bool _isActive;

        public QuestInfo(Quest q, int stage, bool complete, bool active)
        {
            _quest = q;
            _currentStage = stage;
            _isComplete = complete;
            _isActive = active;
        }
    }
    //A list of all the quests in the game and their current progression status
    //Used when revisiting or switching scenes to update the quest log
    //that is local to that scene with the correct data
    [SerializeField] public List<QuestInfo> _questTracker = new List<QuestInfo>();

    public void UpdateQuestProgression(QuestInfo info)
    {
        int index = _questTracker.FindIndex(x => x._quest == info._quest);
        _questTracker[index] = info;
    }

    #endregion
}
