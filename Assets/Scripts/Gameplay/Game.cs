using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private StoryData _introStory = default; //The story used at the introduction to the game
    [SerializeField] private TextDisplay _outputScreen = default; //The screen that this process will display text on
    [SerializeField] private GameObject _introLaptopScreen = default;

    private StoryData _currentStory = default;
    private BeatData _currentBeat;
    private WaitForSeconds _wait;

    private void Awake()
    {
        _currentBeat = null;
        _wait = new WaitForSeconds(0.5f);
        SetStoryData(_introStory);
    }

    private void Update()
    {
        if(_outputScreen.IsIdle)
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

    private void UpdateInput()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if(_currentBeat != null)
        //    {
        //        if (_currentBeat.ID == 1)
        //        {
        //            Application.Quit();
        //        }
        //        else
        //        {
        //            DisplayBeat(1);
        //        }
        //    }
        //}
        //else
        //{
        //    KeyCode alpha = KeyCode.Alpha1;
        //    KeyCode keypad = KeyCode.Keypad1;

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
            if (_currentStory == _introStory)
            {
                _introStory._isCompleted = true;
                _introLaptopScreen.GetComponentInParent<Animator>().SetTrigger("Play");
                PlayerController.Instance.CanControl = true;
            }
        }

        //    for (int count = 0; count < _currentBeat.Decision.Count; ++count)
        //    {
        //        if (alpha <= KeyCode.Alpha9 && keypad <= KeyCode.Keypad9)
        //        {
        //            if (Input.GetKeyDown(alpha) || Input.GetKeyDown(keypad))
        //            {
        //                ChoiceData choice = _currentBeat.Decision[count];
        //                DisplayBeat(choice.NextID);
        //                break;
        //            }
        //        }

        //        ++alpha;
        //        ++keypad;
        //    }
        //}
    }

    private void SetStoryData(StoryData story)
    {
        _currentStory = story;
    }

    private void DisplayBeat(int id)
    {
        BeatData data = _currentStory.GetBeatById(id);
        StartCoroutine(DoDisplay(data));
        _currentBeat = data;
    }

    private IEnumerator DoDisplay(BeatData data)
    {
        _outputScreen.Clear();

        while (_outputScreen.IsBusy)
        {
            yield return null;
        }

        _outputScreen.Display(data.DisplayText, data.DisplayTime);

        while(_outputScreen.IsBusy)
        {
            yield return null;
        }
        
        for (int count = 0; count < data.Decision.Count; ++count)
        {
            ChoiceData choice = data.Decision[count];
            //_output.Display(string.Format("{0}: {1}", (count + 1), choice.DisplayText));
            _outputScreen.Display(choice.DisplayText, data.DisplayTime);

            while (_outputScreen.IsBusy)
            {
                yield return null;
            }
        }

        if(data.Decision.Count > 0)
        {
            _outputScreen.ShowWaitingForInput();
        }
    }
}
