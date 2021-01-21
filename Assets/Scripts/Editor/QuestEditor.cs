using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;

//This script creates a custom editor window for the creation of quests
public class QuestEditor : EditorWindow
{
    private static string _currentQuestFilename = "New Quest"; //The filename of the opened quest
    private static QuestEditor _instance = default; //Editor window instance
    Quest _currentQuest = default; //The current quest object
    SerializedObject _currentQuestData = default; //the serialized data from the current quest
    private Vector2 _scroll = new Vector2(); //The scroll vector in the editor
    SerializedProperty _currentQuestStages = default;

    private enum View { List = 0, Stage = 1}
    private View _view = 0;
    private int _currentIndex = -1;

    private void OnEnable()
    {
        _instance = this;
        _view = View.List;
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

        _scroll = EditorGUILayout.BeginScrollView(_scroll);


        if (_view == View.Stage && _currentIndex != -1)
        {
            OnGUI_StageView(_currentQuestStages, _currentIndex);
        }
        else
        {
            OnGUI_ListView(_currentQuestStages);
        }




        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        _currentQuestData.ApplyModifiedProperties();

    }
   
    private void OnGUI_StageView(SerializedProperty stageList, int index)
    {

        EditorGUILayout.Space(30);

        SerializedProperty currentStage = stageList.GetArrayElementAtIndex(index);

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

        //Show the stage objective text
        SerializedProperty stageObjective = currentStage.FindPropertyRelative("_stageObjective");
        stageObjective.stringValue = EditorGUILayout.TextField("Stage Objective", stageObjective.stringValue);

        EditorGUILayout.Space();


        //SerializedProperty subStageList = currentStage.FindPropertyRelative("_subStages");

        //EditorGUILayout.BeginVertical();
        //EditorUtility.DrawUILine(Color.red);
        //EditorGUILayout.Space(20);


        //for (int subStageCount = 0; subStageCount < subStageList.arraySize; ++subStageCount)
        //{
        //    SerializedProperty currentSubStage = subStageList.GetArrayElementAtIndex(subStageCount);

        //    EditorGUILayout.BeginVertical();


        //    EditorGUILayout.BeginHorizontal();

        //    //Show the quest stage ID
        //    SerializedProperty subStageID = currentSubStage.FindPropertyRelative("_subStageID");
        //    subStageID.intValue = EditorGUILayout.IntField("Sub-Stage ID", subStageID.intValue);

        //    EditorGUILayout.Space();

        //    //Show the linked quest stage ID
        //    SerializedProperty linkedSubStageID = currentSubStage.FindPropertyRelative("_linkedSubStageID");
        //    linkedSubStageID.intValue = EditorGUILayout.IntField("Linked Sub-Stage ID", linkedSubStageID.intValue);

        //    EditorGUILayout.EndHorizontal();

        //    EditorGUILayout.Space();

        //    //Show the quest name
        //    SerializedProperty subStageObjective = currentSubStage.FindPropertyRelative("_subStageObjective");
        //    subStageObjective.stringValue = EditorGUILayout.TextField("Sub-Stage Objective", subStageObjective.stringValue);

        //    EditorGUILayout.Space();

        //    //Section for adding custom event calls to this quest stage
        //    //SerializedProperty subStageCompleteEvent = currentSubStage.FindPropertyRelative("_onSubStageCompleted");
        //    //EditorGUILayout.PropertyField(subStageCompleteEvent);

        //    //Add a button for deleting this beat
        //    if (GUILayout.Button("Delete Sub-Stage"))
        //    {
        //        subStageList.DeleteArrayElementAtIndex(subStageCount);
        //        break;
        //    }

        //    EditorGUILayout.EndVertical();

        //    EditorGUILayout.Space(15);
        //    EditorUtility.DrawUILine(Color.white);
        //    EditorGUILayout.Space(15);

        //}

        //EditorGUILayout.Space(15);

        ////Add a button for adding more choices
        //if (GUILayout.Button((subStageList.arraySize == 0 ? "Add Sub-Stage" : "Add Another Sub-Stage"), GUILayout.Height(100)))
        //{
        //    AddSubStage(subStageList, -1);
        //}


        //EditorGUILayout.EndVertical();

        EditorGUILayout.Space(30);
        EditorUtility.DrawUILine(Color.red);

        //Add a button for deleting this beat
        if (GUILayout.Button("Back to Quest Stages"))
        {
            _currentIndex = -1;
            _view = View.List;
            GUI.FocusControl(null);
        }

        EditorGUILayout.Space();

        //Add a button for deleting this beat
        if (GUILayout.Button("Delete Quest Stage"))
        {

            _currentIndex = -1;
            _view = View.List;
            GUI.FocusControl(null);
            _currentQuestStages.DeleteArrayElementAtIndex(index);
        }
    }

    private void OnGUI_ListView(SerializedProperty questStagesList)
    {
        EditorGUILayout.Space(25);
        EditorGUILayout.BeginVertical();



        //Show the quest ID
        //SerializedProperty questID = _currentQuestData.FindProperty("_questID");
        //questID.intValue = EditorGUILayout.IntField("Quest ID", questID.intValue);

        EditorGUILayout.Space();

        //Show the quest name
        SerializedProperty questName = _currentQuestData.FindProperty("_questName");
        questName.stringValue = EditorGUILayout.TextField("Quest Name", questName.stringValue);

        EditorGUILayout.Space();


        EditorGUILayout.LabelField("Quest Description");

        //Show the quest description
        SerializedProperty questDesc = _currentQuestData.FindProperty("_questDescription");
        questDesc.stringValue = EditorGUILayout.TextArea(questDesc.stringValue);
        EditorGUILayout.Space(25);

        EditorUtility.DrawUILine(Color.white);
        EditorGUILayout.Space(10);


        ////Edge case for if there are no stages in the list, automatically add one
        //if (questStagesList.arraySize == 0)
        //{
        //    AddQuestStage(questStagesList, 1, "First Quest Stage");
        //}

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


            //Add a button for deleting this beat
            if (GUILayout.Button("View Sub-Stages"))
            {
                _currentIndex = count;
                _view = View.Stage;
                GUI.FocusControl(null);
                break;
            }

            EditorGUILayout.Space();

            //Add a button for deleting this beat
            if (GUILayout.Button("Delete Quest Stage"))
            {
                _currentQuestStages.DeleteArrayElementAtIndex(count);
                break;
            }


            EditorGUILayout.Space(10);
            EditorUtility.DrawUILine(Color.black);
            EditorGUILayout.Space(10);

        }

        //Add a button for adding more choices
        if (GUILayout.Button((questStagesList.arraySize == 0 ? "Add Quest Stage" : "Add Another Quest Stage"), GUILayout.Height(100)))
        {
            AddQuestStage(questStagesList, 0);
        }

        EditorGUILayout.EndVertical();
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
