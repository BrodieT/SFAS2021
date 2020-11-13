using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
[CreateAssetMenu(menuName = "My Assets/Story Data")]
public class StoryData : ScriptableObject
{
    [SerializeField] private List<BeatData> _beats = default; //A list of all the story beats
 
    //return the beat data for the given ID
    public BeatData GetBeatById( int id )
    {
        return _beats.Find(b => b.ID == id);
    }

#if UNITY_EDITOR
    public const string PathToAsset = "Assets/Data/Story.asset";

    //This function Loads in the provided story data
    public static StoryData LoadData(string path)
    {
        StoryData data = AssetDatabase.LoadAssetAtPath<StoryData>(path);
        if (data == null)
        {
            Debug.LogWarning("Story Data could not be found at given file path. Creating new story data.");
            //data = CreateInstance<StoryData>();
            //AssetDatabase.CreateAsset(data, path);
            return null;
        }

        return data;
    }

    public static void CreateNewStoryData(string path)
    {
        StoryData data = CreateInstance<StoryData>();
        AssetDatabase.CreateAsset(data, path);
    }

    public static void DeleteStoryData(string path)
    {
        AssetDatabase.DeleteAsset(path);
    }
#endif
}

