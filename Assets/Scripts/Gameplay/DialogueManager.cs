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
    [SerializeField] public TextDisplay _outputScreen = default; //The screen that this process will display text on
    [SerializeField] public GameObject ButtonList = default;
    [SerializeField] public GameObject ButtonPrefab = default;

    [SerializeField] private GameObject _TextDisplayPrefab = default;

    private BeatData _currentBeat;
    private WaitForSeconds _wait;

    private bool _beginStory = false;

    private void Awake()
    {
        _currentBeat = null;
        ButtonList.SetActive(false);
        _wait = new WaitForSeconds(0.5f);
        _story._isCompleted = false;
    }

    public void StartDisplay()
    {
       // _outputScreen = Instantiate(_TextDisplayPrefab).GetComponentInChildren<TextDisplay>();

        _currentBeat = null;
        ButtonList.SetActive(false);
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
                if (_currentBeat.Decision[0].NextID < 0 || _currentBeat.Decision[0].NextID >= _story.GetBeatCount())
                    Debug.Log("Invalid Linked ID");

                DisplayBeat(_currentBeat.Decision[0].NextID);
                return;
            }
        }


        if (_currentBeat.Decision.Count == 0)
        {
            ButtonList.SetActive(false);
            _story._isCompleted = true;
            GameUtility._isPlayerObjectBeingControlled = true;
            PlayerAutoPilot.instance.ResetCamera();
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
        for (int i = 0; i < ButtonList.transform.childCount; i++)
        {
            Destroy(ButtonList.transform.GetChild(i).gameObject);
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

        if (_outputScreen == null)
            Debug.Log("Screen is null");

        if (data == null)
            Debug.Log("data is null");

        _outputScreen.Display(data.DisplayText, data.DisplayTime);

        while (_outputScreen.IsBusy)
        {
            yield return null;
        }


        if (data.Decision.Count >= 1)
        {
            for (int count = 0; count < data.Decision.Count; ++count)
            {
                ChoiceData choice = data.Decision[count];
                //_output.Display(string.Format("{0}: {1}", (count + 1), choice.DisplayText));
                //_outputScreen.Display(choice.DisplayText, data.DisplayTime);
                ButtonList.SetActive(true);

                GameObject g = Instantiate(ButtonPrefab, ButtonList.transform);
                if (g.GetComponentInChildren<TMP_Text>())
                {
                    g.GetComponentInChildren<TMP_Text>().text = choice.DisplayText;
                    g.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { DisplayBeat(choice.NextID); choice.OnSelected.Invoke(true); });

                }


                while (_outputScreen.IsBusy)
                {
                    yield return null;
                }
            }
        }

        if (data.Decision.Count > 0)
        {
            _outputScreen.ShowWaitingForInput();
        }

    }



}

