using System.Collections;
using UnityEngine;
using TMPro;

public class TextDisplay : MonoBehaviour
{
    public enum State { Initialising, Idle, Busy }

    private TMP_Text _displayText;
    private string _displayString;
    private WaitForSeconds _shortWait;
    private WaitForSeconds _longWait;
    private State _state = State.Initialising;
    [SerializeField] private float ShortTime = 0.1f;
    [SerializeField] private float LongTime = 0.8f;
    private string _CurrentText = default;

    public bool IsIdle { get { return _state == State.Idle; } }
    public bool IsBusy { get { return _state != State.Idle; } }




    #region Singleton

    public static TextDisplay Instance;
   
    #endregion


    private void Awake()
    {
        Instance = this;

        _displayText = GetComponent<TMP_Text>();
        _shortWait = new WaitForSeconds(ShortTime);
        _longWait = new WaitForSeconds(LongTime);

        _displayText.text = string.Empty;
        _state = State.Idle;
    }

    private IEnumerator DoShowText(string text)
    {
        _CurrentText = text;
        int currentLetter = 0;
        char[] charArray = text.ToCharArray();

        while (currentLetter < charArray.Length)
        {
            _displayText.text += charArray[currentLetter++];
            yield return _shortWait;
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
            _displayText.text = string.Format( "{0}> {1}", _displayString, ( on ? "|" : " " ));
            on = !on;
            yield return _longWait;
        }
    }

    private IEnumerator DoClearText()
    {
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
        StopAllCoroutines();
        QuickClear();
        _displayText.text = _CurrentText;
        _displayText.text += "\n";
        _displayString = _displayText.text;
        _state = State.Idle;
    }
    public void QuickClear()
    {
        _displayString = string.Empty;
        _displayText.text = _displayString;
    }

    public void Display(string text, float displayTime)
    {
        if (_state == State.Idle)
        {
            _shortWait = new WaitForSeconds(displayTime);
            StopAllCoroutines();
            _state = State.Busy;
            StartCoroutine(DoShowText(text));
        }
    }

    public void ShowWaitingForInput()
    {
        if (_state == State.Idle)
        {
            StopAllCoroutines();
            StartCoroutine(DoAwaitingInput());
        }
    }

    public void Clear()
    {
        if (_state == State.Idle)
        {
            StopAllCoroutines();
            _state = State.Busy;
            StartCoroutine(DoClearText());
        }
    }
}
