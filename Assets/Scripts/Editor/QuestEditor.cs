using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class QuestEditor : EditorWindow
{
    private static string _currentQuestFilename = "New Quest"; //The filename of the opened quest
    private static QuestEditor _instance = default; //Editor window instance
    Quest _currentQuest = default; //The current quest object
    SerializedObject _currentQuestData = default; //the serialized data from the current quest
    private Vector2 _scroll = new Vector2(); //The scroll vector in the editor
    SerializedProperty _currentQuestStages = default;
    Toolbar _toolbar = default;

    

    private void OnEnable()
    {
        _instance = this;


        GenerateToolbar();
    }

    //Opens the editor window 
    [MenuItem("SFAS/Show Quest Editor")]
    public static void ShowQuestEditor()
    {
        GetWindow(typeof(QuestEditor));
    }

    //Occurs when the asset is opened in the editor
    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if (Selection.activeObject as Quest != null)
        {
            _currentQuestFilename = Selection.activeObject.name;

            ShowQuestEditor();
            LoadQuest(Selection.activeObject as Quest);
            return true; //catch open file
        }

        return false; // let unity open the file
    }

    //This function loads the story data into the editor window
    private static void LoadQuest(Quest quest)
    {
        if (quest == null)
            return;


        if (QuestEditor._instance == null)
        {
            Debug.Log("Error");
            return;
        }

        QuestEditor._instance._currentQuest = quest;
        QuestEditor._instance._currentQuestData = new SerializedObject(QuestEditor._instance._currentQuest);

        //Get the list of quest stages from the quest asset
        QuestEditor._instance._currentQuestStages = QuestEditor._instance._currentQuestData.FindProperty("_questStages");
    }

    private void OnGUI()
    {
        //Breakout if there is nothing to show
        if (_currentQuest == null || _currentQuestData == null)
            return;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        OnGUI_QuestStages(_currentQuestStages);





        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        _currentQuestData.ApplyModifiedProperties();

    }
    //This function will be used to generate a toolbar at the top of the story editor window
    //that will allow the saving and loading of different story data
    private void GenerateToolbar()
    {
        //Create the toolbar object
        _toolbar = new Toolbar();

        //_filenameLabel = new Label();
        //_filenameLabel.text = "Story Data Name: " + _currentStoryFileName;
        //_toolbar.Add(_filenameLabel);


        _toolbar.Add(child: new Button(clickEvent: () => {
            if (_currentQuestStages != null)
            {
                //int newBeatId = FindUniqueId(CurrentBeatList);
                AddQuestStage(_currentQuestStages, 0);
            }
        })
        { text = "Add New Quest Stage" });


       
        //Add the toolbar to the editor window
        rootVisualElement.Add(_toolbar);
    }


    private void OnGUI_QuestStages(SerializedProperty questStagesList)
    {
        EditorGUILayout.BeginVertical();

        //Show the quest ID
        SerializedProperty questID = _currentQuestData.FindProperty("_questID");
        questID.intValue = EditorGUILayout.IntField("Quest ID", questID.intValue);

        EditorGUILayout.Space();
        
        //Show the quest name
        SerializedProperty questName = _currentQuestData.FindProperty("_questName");
        questName.stringValue = EditorGUILayout.TextField("Quest Name", questName.stringValue);
        
        EditorGUILayout.Space();


        EditorGUILayout.LabelField("Quest Description");

        //Show the quest description
        SerializedProperty questDesc = _currentQuestData.FindProperty("_questDescription");
        questDesc.stringValue = EditorGUILayout.TextArea(questDesc.stringValue);

        EditorUtility.DrawUILine(Color.white);

        //Edge case for if there are no stages in the list, automatically add one
        if (questStagesList.arraySize == 0)
        {
            AddQuestStage(questStagesList, 1, "First Quest Stage");
        }

        //Loop through the quest stages 
        for (int count = 0; count < _currentQuestStages.arraySize; ++count)
        {
            SerializedProperty currentStage = questStagesList.GetArrayElementAtIndex(count);

            EditorGUILayout.Space();


            EditorGUILayout.BeginHorizontal();
           
            //Show the quest stage ID
            SerializedProperty stageID = currentStage.FindPropertyRelative("_stageID");
            stageID.intValue = EditorGUILayout.IntField("Stage ID", stageID.intValue);

            EditorGUILayout.Space();

            //Show the linked quest stage ID
            SerializedProperty linkedStageID = currentStage.FindPropertyRelative("_linkedStageID");
            linkedStageID.intValue = EditorGUILayout.IntField("Linked Stage ID", linkedStageID.intValue);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            //Show the quest name
            SerializedProperty stageObjective = currentStage.FindPropertyRelative("_stageObjective");
            stageObjective.stringValue = EditorGUILayout.TextField("Stage Objective", stageObjective.stringValue);

            EditorGUILayout.Space();

            //Section for adding custom event calls to this quest stage
            SerializedProperty stageCompleteEvent = currentStage.FindPropertyRelative("_onQuestStageComplete");
            EditorGUILayout.PropertyField(stageCompleteEvent);


            SerializedProperty subStageList = currentStage.FindPropertyRelative("_subStages");
            
            EditorGUILayout.BeginHorizontal();

            for (int subStageCount = 0; subStageCount < subStageList.arraySize; ++subStageCount)
            {
                SerializedProperty currentSubStage = subStageList.GetArrayElementAtIndex(subStageCount);

                EditorGUILayout.BeginVertical();


                EditorGUILayout.BeginHorizontal();

                //Show the quest stage ID
                SerializedProperty subStageID = currentSubStage.FindPropertyRelative("_subStageID");
                subStageID.intValue = EditorGUILayout.IntField("Sub-Stage ID", subStageID.intValue);

                EditorGUILayout.Space();

                //Show the linked quest stage ID
                SerializedProperty linkedSubStageID = currentSubStage.FindPropertyRelative("_linkedSubStageID");
                linkedSubStageID.intValue = EditorGUILayout.IntField("Linked Sub-Stage ID", linkedSubStageID.intValue);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                //Show the quest name
                SerializedProperty subStageObjective = currentSubStage.FindPropertyRelative("_subStageObjective");
                subStageObjective.stringValue = EditorGUILayout.TextField("Sub-Stage Objective", subStageObjective.stringValue);

                EditorGUILayout.Space();

                //Section for adding custom event calls to this quest stage
                SerializedProperty subStageCompleteEvent = currentSubStage.FindPropertyRelative("_onSubStageCompleted");
                EditorGUILayout.PropertyField(subStageCompleteEvent);

                //Add a button for deleting this beat
                if (GUILayout.Button("Delete Sub-Stage"))
                {
                    subStageList.DeleteArrayElementAtIndex(subStageCount);
                    break;
                }
                EditorGUILayout.EndVertical();

            }

            //Add a button for adding more choices
            if (GUILayout.Button((subStageList.arraySize == 0 ? "Add Sub-Stage" : "Add Another Sub-Stage"), GUILayout.Height(100)))
            {
                AddSubStage(subStageList, -1);
            }


            EditorGUILayout.EndHorizontal();
            
            //Add a button for deleting this beat
            if (GUILayout.Button("Delete Quest Stage"))
            {
                _currentQuestStages.DeleteArrayElementAtIndex(count);
                break;
            }



            EditorUtility.DrawUILine(Color.black);
        }

        EditorGUILayout.EndVertical();
    }


    private void AddSubStage(SerializedProperty subStageList, int subStageID = -1, string initialText = "New Sub-Stage")
    {
        int index = subStageList.arraySize;
        subStageList.arraySize += 1;
        SerializedProperty arrayElement = subStageList.GetArrayElementAtIndex(index);
        SerializedProperty text = arrayElement.FindPropertyRelative("_subStageObjective");
        SerializedProperty id = arrayElement.FindPropertyRelative("_subStageID");

        text.stringValue = initialText;
        id.intValue = subStageID;
    }

    private void AddQuestStage(SerializedProperty questStageList, int questStageID, string initialText = "New Quest Stage")
    {
        int index = questStageList.arraySize;
        questStageList.arraySize += 1;
        SerializedProperty arrayElement = questStageList.GetArrayElementAtIndex(index);
        SerializedProperty text = arrayElement.FindPropertyRelative("_stageObjective");
        SerializedProperty id = arrayElement.FindPropertyRelative("_stageID");

        text.stringValue = initialText;
        id.intValue = questStageID;
    }
}
