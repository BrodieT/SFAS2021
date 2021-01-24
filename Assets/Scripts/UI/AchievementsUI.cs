using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsUI : MonoBehaviour
{
    [System.Serializable]
    public struct AchievementUIData
    {
        public Image _icon;
        public int _achievementID;
        public Sprite _completedIcon;
    }

    [SerializeField] private Sprite _lockedIcon = default;


    [SerializeField] List<AchievementUIData> _achievements = new List<AchievementUIData>();

    private void UpdateAchievementUI()
    {
        foreach (AchievementUIData achievement in _achievements)
        {
            if (ProgressionTracker.instance._allAchievements[achievement._achievementID]._isAchieved)
                achievement._icon.sprite = achievement._completedIcon;
            else
                achievement._icon.sprite = _lockedIcon;
        }
    }

    public void AchievementsLogOpened()
    {
        UpdateAchievementUI();
    }
}
    