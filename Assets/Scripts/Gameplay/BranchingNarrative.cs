using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TMPro;

public class BranchingNarrative : MonoBehaviour
{
    [SerializeField] public StoryData _story = default; //The story used at the introduction to the game
    [SerializeField] public float _displayTime = 0.1f;



    [HideInInspector] public TextDisplay _outputScreen = default; //The screen that this process will display text on
    private GameObject _buttonList = default;
    [SerializeField] public GameObject _buttonPrefab = default;

    private BeatData _currentBeat;
    private WaitForSeconds _wait;

    private bool _beginStory = false;

    private void Start()
    {
        _currentBeat = null;
        _wait = new WaitForSeconds(0.5f);
        _story._isCompleted = false;
        

        GameObject _interactableUI = transform.Find("ScreenInteractable").gameObject;
        _interactableUI.GetComponent<Canvas>().worldCamera = PlayerInteract.instance._playerCamera;
        _buttonList = _interactableUI.transform.GetChild(0).gameObject;
        _buttonList.SetActive(false);

    }

    public virtual void GetOutputScreen()
    {
        
    }

    public void StartDisplay()
    {
        _currentBeat = null;
        _buttonList.SetActive(false);
        _wait = new WaitForSeconds(0.5f);

        if (_story._isRepeatable && _story._isCompleted)
            _story._isCompleted = false;

        _beginStory = true;
    }


    public virtual void Update()
    {
        if (_beginStory)
        {
            //Toggle the cursor where appropriate if the story is in progress
            if (_story._isCompleted)
            {
                GameUtility.HideCursor();
            }
            else if (GameUtility._isCursorHidden)
            {
                GameUtility.ShowCursor();
            }

            //If an invalid beat is selected while the screen is idle, revert to the first beat
            //Otherwise update input
            if (_outputScreen.IsIdle)
            {
                if (_currentBeat == null)
                {
                    DisplayBeat(1);
                }
                else
                {
                    UpdateInput();
                }
            }
        }
    }


    public virtual void UpdateInput()
    {
        //Autoprogress if there is only one option for the current beat and it is set to auto
        if (_currentBeat.Decision.Count == 1)
        {
            if (_currentBeat.Decision[0].AutoProgress)
            {
                if (_currentBeat.Decision[0].NextID <= 0 || _currentBeat.Decision[0].NextID >= _story.GetBeatCount())
                    Debug.LogError("Invalid Linked ID");

                DisplayBeat(_currentBeat.Decision[0].NextID);
                return;
            }
        }

        //Complete the story if no further options are available
        if (_currentBeat.Decision.Count == 0)
        {
            _outputScreen.Clear();
            _buttonList.SetActive(false);
            _story._isCompleted = true;
            GameUtility._isPlayerObjectBeingControlled = true;
            PlayerAutoPilot.instance.ResetCamera();
            _outputScreen.FinishDisplay();
            StopAllCoroutines();
            _beginStory = false;
            GameUtility.HideCursor();
        }
    }

    private void DisplayBeat(int id)
    {
        if (id <= 0)
            id = 1;

        BeatData data = _story.GetBeatById(id);
        StartCoroutine(DoDisplay(data));
        _currentBeat = data;
    }

    private void ClearButtons()
    {
        for (int i = 0; i < _buttonList.transform.childCount; i++)
        {
            Destroy(_buttonList.transform.GetChild(i).gameObject);
        }
    }

    IEnumerator DoDisplay(BeatData data)
    {
        ClearButtons();
        _outputScreen.Clear();

        while (_outputScreen.IsBusy)
        {
            yield return null;
        }

        _outputScreen.Display(data.DisplayText);

        //Wait until the text has finished appearing before showing options
        while (_outputScreen.IsBusy)
        {
            yield return null;
        }

        //Show the dialogue Options

        if (data.Decision.Count >= 1)
        {
            _buttonList.SetActive(true);

            for (int count = 0; count < data.Decision.Count; ++count)
            {
                ChoiceData choice = data.Decision[count];

                GameObject g = Instantiate(_buttonPrefab, _buttonList.transform);

                TMP_Text buttonText = default;

                for (int i = 0; i < g.transform.childCount; i++)
                {
                    if (g.transform.GetChild(i).TryGetComponent<TMP_Text>(out buttonText))
                    {
                        //Show the display text on the button
                        buttonText.text = choice.DisplayText;

                        //Add a listener to the button to display the linked story beat when clicked
                        g.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate
                        { DisplayBeat(choice.NextID); choice.OnSelected.Invoke(true); });

                        break;
                    }
                }
            }

            //Show the blinking cursor while awaiting input
            _outputScreen.ShowWaitingForInput();

        }

    }
}
