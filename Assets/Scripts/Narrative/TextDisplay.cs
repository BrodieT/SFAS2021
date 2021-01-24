using System.Collections;
using UnityEngine;
using TMPro;

//This script displays text to a given output
public class TextDisplay : MonoBehaviour
{
    public enum State { Initialising, Idle, Busy }

    [SerializeField] private TMP_Text _displayText; //Where the text will be displayed
    private string _displayString; //the string to be displayed
    private WaitForSeconds _displayWaitTime; //the frequency of displaying letters 
    private WaitForSeconds _cursorBlinkTime; //the frequency of the cursor blink
    private State _state = State.Initialising; //Whether an operation is underway
    private string _CurrentText = default; //the current display text

    public bool IsIdle { get { return _state == State.Idle; } }
    public bool IsBusy { get { return _state != State.Idle; } }

    private void Start()
    {
        if(_displayText == null)
            _displayText = GetComponent<TMP_Text>();
        _displayText.text = string.Empty;
        _state = State.Idle;
    }

    public void FinishDisplay()
    {
        _displayText.text = string.Empty;
        _state = State.Idle;
    }

    public void InitialiseWaitTimes(float displayTime)
    {
        _displayWaitTime = new WaitForSeconds(displayTime);
        _cursorBlinkTime = new WaitForSeconds(0.8f);
    }

    private IEnumerator DoShowText(string text)
    {
        //If the text display is currently busy, wait for it to finish current task
        while(_state != State.Idle)
        {
            Debug.Log("Display is busy. Waiting to display");
            yield return null;
        }

        _state = State.Busy;

        _CurrentText = text;
        int currentLetter = 0;
        char[] charArray = text.ToCharArray();

        while (currentLetter < charArray.Length)
        {
            _displayText.text += charArray[currentLetter++];
            yield return _displayWaitTime;
        }

        _displayText.text += "\n";
        _displayString = _displayText.text;
        _state = State.Idle;
    }

    private IEnumerator DoAwaitingInput()
    {
        bool on = true;

        while (enabled)
        {
            if (IsBusy)
            {
                enabled = false;
            }

            _displayText.text = string.Format( "{0}> {1}", _displayString, ( on ? "|" : " " ));
            on = !on;
            yield return _cursorBlinkTime;
        }
    }

    private IEnumerator DoClearText()
    {
        _state = State.Busy;

        int currentLetter = 0;
        char[] charArray = _displayText.text.ToCharArray();

        while (currentLetter < charArray.Length)
        {
            if (currentLetter > 0 && charArray[currentLetter - 1] != '\n')
            {
                charArray[currentLetter - 1] = ' ';
            }

            if (charArray[currentLetter] != '\n')
            {
                charArray[currentLetter] = '_';
            }

            _displayText.text = charArray.ArrayToString();
            ++currentLetter;
            yield return null;
        }

        _displayString = string.Empty;
        _displayText.text = _displayString;
        _state = State.Idle;
    }

    public void QuickDisplay()
    {
        //If the display is not busy then there is nothing still to display
        if (_state != State.Busy)
            return;

        //Stop current display processes
        StopAllCoroutines();
        //Quickly clear any leftovers
        QuickClear();

        //Immediately display the current text
        _displayText.text = _CurrentText;
        _displayText.text += "\n";
        _displayString = _displayText.text;

        //Return to the idle state
        _state = State.Idle;
    }
    public void QuickClear()
    {
        _displayString = string.Empty;
        _displayText.text = _displayString;
    }

    public void Display(string text)
    {
        StartCoroutine(DoShowText(text));        
    }

    public void ShowWaitingForInput()
    {
        StartCoroutine(DoAwaitingInput());        
    }

    public void Clear()
    {
        StartCoroutine(DoClearText());
    }
}
