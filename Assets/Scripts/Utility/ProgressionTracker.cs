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

    public struct DeadEnemyLog
    {
        public SceneLoader.SceneName _scene;
        public int _index;

        public DeadEnemyLog(int id, SceneLoader.SceneName name)
        {
            _scene = name;
            _index = id;
        }
    }

    public List<DeadEnemyLog> _deadEnemies = new List<DeadEnemyLog>();

    public void ClearDefeatedEnemies()
    {
        if (EnemyManager.instance)
        {
            List<DeadEnemyLog> toClear = _deadEnemies.FindAll(x => x._scene == SceneLoader.instance.GetCurrentScene()._sceneName);
            for (int i = 0; i < toClear.Count; i++)
            {
                Destroy(EnemyManager.instance._allEnemies[toClear[i]._index]);
            }
        }
    }
    public void AddDefeatedEnemy(int index)
    {
        _deadEnemies.Add(new DeadEnemyLog(index, SceneLoader.instance.GetCurrentScene()._sceneName));
    }



    #region SaveLoad
    public void StartNewGame()
    {
        _questTracker.Clear();
        _loadedScenes.Clear();

        for (int i = 0; i < _allAchievements.Count; i++)
        {
            AchievementData data = _allAchievements[i];
            data._isAchieved = false;
            _allAchievements[i] = data;
        }
    }

    public Vector3 _playersLastPosition = new Vector3();
    public void ContinueGame()
    {
        Game_Manager.instance._player.transform.position = _playersLastPosition;
        Game_Manager.instance.GetComponent<PlayerQuestLog>().UpdateQuestmarker();
    }
    #endregion

    #region Resource Management

    public int _loadedAmmo = 6;
    public int _spareAmmo = 30;

    public void SetAmmoCounters(int loaded, int spare)
    {
        _loadedAmmo = loaded;
        _spareAmmo = spare;
    }

    #endregion

    #region Achievement Progression


    #region Collectible-based Achievements

    public int _collectiblesFound = 0;
    public int _totalCollectibleCount = 3;

    public void FoundCollectible()
    {
        _collectiblesFound++;

        //25% of collectibles
        if (_collectiblesFound == (int)(_totalCollectibleCount / 4))
        {
            UnlockAchievement(4);
        }

        //50% of collectibles
        if (_collectiblesFound == (int)(_totalCollectibleCount / 2))
        {
            UnlockAchievement(5);
        }

        //75% of collectibles
        if (_collectiblesFound == (int)((_totalCollectibleCount / 4) * 3))
        {
            UnlockAchievement(6);
        }

        //100% of collectibles
        if (_collectiblesFound == _totalCollectibleCount)
        {
            UnlockAchievement(7);
        }
    }
    #endregion
    #region Turret Smasher Achievement
    private int _turretsKilled = 0;
    public void IncrementTurretCounter()
    {
        _turretsKilled++;
        IncrementEnemyKillCount();
        if (_turretsKilled > 10)
            UnlockAchievement(9);
    }
    #endregion
    #region Immovable Object Achievement
    private int _chargersKilled = 0;
    public void IncrementChargerCounter()
    {
        _chargersKilled++;
        IncrementEnemyKillCount();
        if (_chargersKilled > 5)
            UnlockAchievement(10);
    }
    #endregion
    #region Oncoming Storm Achievement
    [SerializeField] private int _totalNumberOfEnemies = 20;
    private int _enemiesKilled = 0;
    public void IncrementEnemyKillCount()
    {
        _enemiesKilled++;

        if (_enemiesKilled >= _totalNumberOfEnemies)
            UnlockAchievement(11);
    }
    #endregion
    #region Bloodthirsty Achievement
    public void CheckBloodthirstyAchievementStatus()
    {
        QuestInfo easyWay = _questTracker.Find(x => x._quest._questName == "The Easy Way");
        QuestInfo hardWay = _questTracker.Find(x => x._quest._questName == "The Hard Way");

        if (easyWay._isComplete && hardWay._currentStage > 0)
            UnlockAchievement(3);

        //Find both The Easy Way and The Hard Way quest entries
        //if The Easy Way is completed && The Hard Way is past the first quest stage
        //Unlock Achievement

        //NB Call function on completion of both The Easy Way's final stage and The Hard Way's first stage
    }
    #endregion
    #region Sinking Ship Achievement
    public void CheckSinkingShipAchievementStatus()
    {
        QuestInfo sinkingShip = _questTracker.Find(x => x._quest._questName == "Escape the Flooded District");
        if (sinkingShip._isComplete)
            UnlockAchievement(2);
    }
    #endregion
    #region Recharged Achievement
    public void CheckRechargedAchievementStatus()
    {
        QuestInfo tutorial = _questTracker.Find(x => x._quest._questName == "Tutorial Simulation");
        if (tutorial._isComplete)
            UnlockAchievement(1);
    }
    #endregion

    public void UnlockAchievement(int id)
    {
        if (id >= 0 && id < _allAchievements.Count)
        {
            AchievementData data = _allAchievements[id];
            data._isAchieved = true;
            _allAchievements[id] = data;
            Game_Manager.instance._UIManager._discoveryUI.Discover(new DiscoveryUI.Discovery("Achievement", "Unlocked"));

            //Prevent infinite loop, only proceed if not completing the completionist trophy
            if (id > 0)
            {
                //Check to see if completionist trophy is achieved
                for (int i = 1; i < _allAchievements.Count; i++)
                {
                    //If any remain locked then return
                    if (!_allAchievements[i]._isAchieved)
                        return;
                }

                //if not returned then all others must be unlocked
                UnlockAchievement(0);
            }
        }
    }

    [System.Serializable]
    public struct AchievementData
    {        
        [SerializeField] public string _achievementName;
        [SerializeField] public bool _isAchieved;

        public void Complete()
        {
            _isAchieved = true;
        }
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

            if (info._currentStage < 0)
            {
                info._isComplete = true;
            }

            if (info._isComplete)
                info._isCurrent = false;

            _questTracker[index] = info;

            CheckBloodthirstyAchievementStatus();
            CheckRechargedAchievementStatus();
            CheckSinkingShipAchievementStatus();

        }
        else
        {
            _questTracker.Add(info);
        }

        PlayerQuestLog.instance.UpdateQuestmarker();

       

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
