using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionTracker : AutoCleanupSingleton<ProgressionTracker>
{
    public override void Awake()
    {
        base.Awake();
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
        public bool _isCurrent;
        public SceneLoader.SceneName _scene;

        public void SetCurrent(bool curr)
        {
            _isCurrent = curr;
        }

        public QuestInfo(Quest q, int stage, bool complete, bool active, bool current, SceneLoader.SceneName name)
        {
            _quest = q;
            _currentStage = stage;
            _isComplete = complete;
            _isActive = active;
            _isCurrent = current;
            _scene = name;
        }
    }
    //A list of all the quests in the game and their current progression status
    //Used when revisiting or switching scenes to update the quest log
    //that is local to that scene with the correct data
    [SerializeField] public List<QuestInfo> _questTracker = new List<QuestInfo>();

    public void UpdateQuestProgression(QuestInfo info)
    {
        int index = -1;
        index = _questTracker.FindIndex(x => x._quest._questID == info._quest._questID);

        if (index >= 0)
        {



            _questTracker[index] = info;

            if (_questTracker[index]._isCurrent && _questTracker[index]._isComplete)
                _questTracker[index].SetCurrent(false);

        }
        else
        {
            Debug.Log("Quest Not Found. Adding Now");
            _questTracker.Add(info);
        }
    }

    public List<QuestInfo> GetQuestData()
    {
        return _questTracker;
    }


    public List<QuestInfo> GetAllActiveQuests()
    {
        return _questTracker.FindAll(x => x._isActive == true);
    }

    public QuestInfo GetCurrentQuest()
    {
        QuestInfo current = _questTracker.Find(x => x._isCurrent == true);
        return current;
    }

    public void SetCurrentQuest(Quest quest)
    {
        int previousIndex = -1;
        previousIndex = _questTracker.FindIndex(x => x._isCurrent == true);
        int nextIndex = -1;
        nextIndex = _questTracker.FindIndex(x => x._quest._questID == quest._questID);

        if(previousIndex >= 0 && nextIndex >= 0)
        {
            _questTracker[previousIndex].SetCurrent(false);
            _questTracker[nextIndex].SetCurrent(true);
        }

    }

    #endregion
}
