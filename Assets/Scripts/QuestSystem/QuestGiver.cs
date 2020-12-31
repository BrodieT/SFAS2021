using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] Quest _quest = default;

    // Start is called before the first frame update
    void Start()
    {
        PlayerQuestLog.instance.AddNewActiveQuest(_quest); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
