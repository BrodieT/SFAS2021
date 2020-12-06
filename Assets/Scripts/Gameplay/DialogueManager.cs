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

    [SerializeField] public StoryData Story = default; //The story used at the introduction to the game
    [SerializeField] public TextDisplay _outputScreen = default; //The screen that this process will display text on
    [SerializeField] public GameObject ButtonList = default;
    [SerializeField] public GameObject ButtonPrefab = default;

    private BeatData _currentBeat;
    private WaitForSeconds _wait;

    private bool BeginStory = false;

    private void Awake()
    {
        _currentBeat = null;
        ButtonList.SetActive(false);
        _wait = new WaitForSeconds(0.5f);
        Story.IsCompleted = false;
    }

    public void StartDisplay()
    {
        _currentBeat = null;
        ButtonList.SetActive(false);
        _wait = new WaitForSeconds(0.5f);
        Story.IsCompleted = false;
        BeginStory = true;
    }

    private void Update()
    {
        if (BeginStory)
        {
            if (Story.IsCompleted)
            {
                GameUtility.HideCursor();
            }
            else if (GameUtility.IsCursorHidden)
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
                DisplayBeat(_currentBeat.Decision[0].NextID);
                return;
            }
        }


        if (_currentBeat.Decision.Count == 0)
        {
            ButtonList.SetActive(false);
            Story.IsCompleted = true;
            PlayerController.Instance.CanControl = true;
            PlayerController.Instance.ResetCamera();
        }
    }

   
    private void DisplayBeat(int id)
    {
       
        BeatData data = Story.GetBeatById(id);
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

        _outputScreen.Display(data.DisplayText, data.DisplayTime);

        while (_outputScreen.IsBusy)
        {
            yield return null;
        }


        if (data.Decision.Count > 1)
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

