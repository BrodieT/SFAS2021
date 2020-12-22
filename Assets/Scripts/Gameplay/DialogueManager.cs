using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TMPro;

public class DialogueManager : MonoBehaviour
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
        _outputScreen = GetComponentInChildren<TextDisplay>();
        _outputScreen.InitialiseWaitTimes(_displayTime);

        GameObject _interactableUI = transform.Find("ScreenInteractable").gameObject;
        _interactableUI.GetComponent<Canvas>().worldCamera = PlayerInteract.instance._playerCamera;
        _buttonList = _interactableUI.transform.GetChild(0).gameObject; 
        _buttonList.SetActive(false);

    }

    public void StartDisplay()
    {
        _currentBeat = null;
        _buttonList.SetActive(false);
        _wait = new WaitForSeconds(0.5f);

        if(_story._isRepeatable && _story._isCompleted)
            _story._isCompleted = false;

        _beginStory = true;
    }

    private void Update()
    {
        if (_beginStory)
        {
            if (_story._isCompleted)
            {
                GameUtility.HideCursor();
            }
            else if (GameUtility._isCursorHidden)
            {
                GameUtility.ShowCursor();
            }


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


        if (_currentBeat.Decision.Count == 0)
        {
            Debug.Log("Story Is Over");
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

    private IEnumerator DoDisplay(BeatData data)
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
                    if(g.transform.GetChild(i).TryGetComponent<TMP_Text>(out buttonText))
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

