using UnityEngine;
using UnityEditor;


using UnityEngine.UIElements;
using UnityEditor.UIElements;

//This class creates the story editor window
public class StoryEditor : EditorWindow
{

    #region Singleton

    public static StoryEditor Instance;


    #endregion

    private enum View { List, Beat }

    private Vector2 _scroll = new Vector2();
    private int _currentIndex = -1;
    private View _view;
    
    //The filename of the story data being saved/loaded
    private static string _currentStoryFileName = "New Story Data";
    StoryData CurrentStory = default;
    SerializedObject CurrentData = default;
    SerializedProperty CurrentBeatList = default;

    Toolbar _toolbar = default;
    private Label _filenameLabel = default;
    private Label _repeatableLabel = default;

    [MenuItem("SFAS/Show Story Editor")]
    public static void ShowStoryEditor()
    {
        GetWindow(typeof(StoryEditor));
    }

    private void OnEnable()
    {
        Instance = this;
        GenerateToolbar();
    }
    //This function handles the creation of the editor GUI for story creation
    void OnGUI()
    {
        if (CurrentStory == null || CurrentData == null || CurrentBeatList == null)
            return;

        _filenameLabel.text = "Story Data Name: " + _currentStoryFileName;


        //Begin the GUI 
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Create a scroll view
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        //If a beat is selected show the beat GUI
        if (_view == View.Beat && _currentIndex != -1)
        {
            OnGUI_BeatView(CurrentBeatList, _currentIndex);
        }
        else //Otherwise show the list of all beats
        {
            OnGUI_ListView(CurrentBeatList);
        }

        //End the GUI

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        CurrentData.ApplyModifiedProperties();


    }

    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if (Selection.activeObject as StoryData != null)
        {
            _currentStoryFileName = Selection.activeObject.name;

            ShowStoryEditor();
            LoadStory(Selection.activeObject as StoryData);
            return true; //catch open file
        }

        return false; // let unity open the file
    }

    //This function will be used to generate a toolbar at the top of the story editor window
    //that will allow the saving and loading of different story data
    private void GenerateToolbar()
    {
        //Create the toolbar object
        _toolbar = new Toolbar();

        _filenameLabel = new Label();
        _filenameLabel.text = "Story Data Name: " + _currentStoryFileName;
        _toolbar.Add(_filenameLabel);


        _toolbar.Add(child: new Button(clickEvent: () => {
            if (CurrentBeatList != null)
            {
                int newBeatId = FindUniqueId(CurrentBeatList);
                AddBeat(CurrentBeatList, newBeatId);
            }
        })
        { text = "Add New Beat" });


        _toolbar.Add(child: new Button(clickEvent: () => {
            ToggleRepeatable();
        })
        { text = "Toggle Repeatable" });

        _repeatableLabel = new Label();
        _toolbar.Add(_repeatableLabel);

        //Add the toolbar to the editor window
        rootVisualElement.Add(_toolbar);
    }

    private void ToggleRepeatable()
    {
        if(CurrentStory != null)
        {
            CurrentStory._isRepeatable = !CurrentStory._isRepeatable;
            if (CurrentStory._isRepeatable)
                _repeatableLabel.text = "Repeatable";
            else
                _repeatableLabel.text = "Not Repeatable";
        }
    }

    //This function loads the story data into the editor window
    private static void LoadStory(StoryData story)
    {
        if (story == null)
            return;


        if(StoryEditor.Instance == null)
        {
            Debug.Log("Error");
            return;
        }

       StoryEditor.Instance.CurrentStory = story;
        StoryEditor.Instance.CurrentData = new SerializedObject(StoryEditor.Instance.CurrentStory);
        //Get the list of story beats
        StoryEditor.Instance.CurrentBeatList = StoryEditor.Instance.CurrentData.FindProperty("_beats");
        StoryEditor.Instance._view = View.List;

    }
 
    //This function creates the list of story beats in the editor window
    private void OnGUI_ListView(SerializedProperty beatList)
    {
        EditorGUILayout.BeginVertical();

        //Edge case for if there are no beats in the list, automatically add one
        if (CurrentBeatList.arraySize == 0)
        {
           AddBeat(CurrentBeatList, 1, "First Story Beat");
        }

        //Loop through the story beats 
        for (int count = 0; count < beatList.arraySize; ++count)
        {
            SerializedProperty arrayElement = beatList.GetArrayElementAtIndex(count);
            SerializedProperty choiceList = arrayElement.FindPropertyRelative("_choices");
            SerializedProperty text = arrayElement.FindPropertyRelative("_text");
            SerializedProperty id = arrayElement.FindPropertyRelative("_id");

            EditorGUILayout.BeginHorizontal();

            //Display the ID as a label for this beat
            EditorGUILayout.LabelField(id.intValue.ToString());

            //Add an edit button that will switch to beat view
            if (GUILayout.Button("Edit"))
            {
                _view = View.Beat;
                _currentIndex = count;
                break;
            }

            //Add a button for deleting this beat
            if (GUILayout.Button("Delete"))
            {
                beatList.DeleteArrayElementAtIndex(count);
                break;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            //Add a text field for the beat contents to go
            EditorGUILayout.PropertyField(text);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    //This function creates the GUI for editing an individual story beat
    private void OnGUI_BeatView(SerializedProperty beatList, int index)
    {
        SerializedProperty arrayElement = beatList.GetArrayElementAtIndex(index);
        SerializedProperty choiceList = arrayElement.FindPropertyRelative("_choices");
        SerializedProperty text = arrayElement.FindPropertyRelative("_text");
        SerializedProperty id = arrayElement.FindPropertyRelative("_id");
        SerializedProperty time = arrayElement.FindPropertyRelative("DisplayTime");

        EditorGUILayout.BeginVertical();
        //Display the ID as a label
        EditorGUILayout.LabelField("Beat ID: " + id.intValue.ToString());
        //Store the contents of the text area being created
        text.stringValue = EditorGUILayout.TextArea(text.stringValue, GUILayout.Height(200));
        
        EditorGUILayout.PropertyField(time);

        //Function for displaying the choices for this beat
        OnGUI_BeatViewDecision(choiceList, beatList);

        EditorGUILayout.EndVertical();

        //Add a button to return to the list view
        if (GUILayout.Button("Return to Beat List", GUILayout.Height(50)))
        {
            _view = View.List;
            _currentIndex = -1;

            GUI.FocusControl(null);
        }

    }

    //This function displays all the choices for the current beat
    private void OnGUI_BeatViewDecision(SerializedProperty choiceList, SerializedProperty beatList)
    {
        EditorGUILayout.BeginHorizontal();

        //Loop through all the choices and display in the editor
        for (int count = 0; count < choiceList.arraySize; ++count)
        {
            OnGUI_BeatViewChoice(choiceList, count, beatList);
        }

        //Add a button for adding more choices
        if (GUILayout.Button((choiceList.arraySize == 0 ? "Add Choice" : "Add Another Choice"), GUILayout.Height(100)))
        {
           // int newBeatId = FindUniqueId(beatList);
           // AddBeat(beatList, newBeatId);
            AddChoice(choiceList, -1);
        }

        EditorGUILayout.EndHorizontal();
    }

    

    //This function displays the individual choices available
    private void OnGUI_BeatViewChoice(SerializedProperty choiceList, int index, SerializedProperty beatList)
    {
        SerializedProperty arrayElement = choiceList.GetArrayElementAtIndex(index);
        SerializedProperty text = arrayElement.FindPropertyRelative("_text");
        SerializedProperty beatId = arrayElement.FindPropertyRelative("_beatId");
        SerializedProperty prog = arrayElement.FindPropertyRelative("AutoProgress");
        SerializedProperty evnt = arrayElement.FindPropertyRelative("OnSelected");

        EditorGUILayout.BeginVertical();

        //Store the contents of the text area for this choice
        text.stringValue = EditorGUILayout.TextArea(text.stringValue, GUILayout.Height(50));
        //Add label for the linked ID 
        EditorGUILayout.LabelField("Leads to Beat ID: " + beatId.intValue.ToString());

        beatId.intValue = EditorGUILayout.IntField(new GUIContent("Linked ID"), beatId.intValue);


        if(!IsIdInList(CurrentBeatList, beatId.intValue))
        {
            beatId.intValue = 0;
        }

        //Add a button for deleting this choice
        if (GUILayout.Button("Delete"))
        {
            choiceList.DeleteArrayElementAtIndex(index);
        }

        //Add a button to go to the linked beat
        if (GUILayout.Button("Go to Beat"))
        {
            _currentIndex = FindIndexOfBeatId(beatList, beatId.intValue);
            GUI.FocusControl(null);
            Repaint();
        }
        
        prog.boolValue = (EditorGUILayout.Toggle("Automatically Progress", prog.boolValue));


        EditorGUILayout.PropertyField(evnt);
        

        
        CurrentData.ApplyModifiedProperties();

        

        EditorGUILayout.EndVertical();
    }

    //This function will find a unique ID for a new beat by finding the largest existing ID and incrementing
    private int FindUniqueId(SerializedProperty beatList)
    {
        int result = 1;

        while (IsIdInList(beatList, result))
        {
            ++result; 
        }

        return result;
    }

    //This function loops through the beat list and returns true if the given ID already exists in the list
    private bool IsIdInList(SerializedProperty beatList, int beatId)
    {
        bool result = false;
        

        for (int count = 0; count < beatList.arraySize && !result; ++count)
        {
            SerializedProperty arrayElement = beatList.GetArrayElementAtIndex(count);
            SerializedProperty id = arrayElement.FindPropertyRelative("_id");
            result = id.intValue == beatId;
        }

        return result;
    }

    //This function return the index of a provided beat ID 
    private int FindIndexOfBeatId(SerializedProperty beatList, int beatId)
    {
        int result = -1;

        for (int count = 0; count < beatList.arraySize; ++count)
        {
            SerializedProperty arrayElement = beatList.GetArrayElementAtIndex(count);
            SerializedProperty id = arrayElement.FindPropertyRelative("_id");
            if (id.intValue == beatId)
            {
                result = count;
                break;
            }
        }

        return result;
    }

    //This function creates a new beat 
    private void AddBeat(SerializedProperty beatList, int beatId, string initialText = "New Story Beat")
    {
        int index = beatList.arraySize;
        beatList.arraySize += 1;
        SerializedProperty arrayElement = CurrentBeatList.GetArrayElementAtIndex(index);
        SerializedProperty text = arrayElement.FindPropertyRelative("_text");
        SerializedProperty id = arrayElement.FindPropertyRelative("_id");

        text.stringValue = initialText;
        id.intValue = beatId;
    }

    //This function creates a new beat choice
    private void AddChoice(SerializedProperty choiceList, int beatId, string initialText = "New Beat Choice")
    {
        int index = choiceList.arraySize;
        choiceList.arraySize += 1;
        SerializedProperty arrayElement = choiceList.GetArrayElementAtIndex(index);
        SerializedProperty text = arrayElement.FindPropertyRelative("_text");
        SerializedProperty nextId = arrayElement.FindPropertyRelative("_beatId");

        text.stringValue = initialText;
        nextId.intValue = beatId;
    }

}
