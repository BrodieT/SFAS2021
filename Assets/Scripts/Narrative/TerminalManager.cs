
using UnityEngine;
public class TerminalManager : BranchingNarrative
{
    public override void OutputSetup()
    {
        _outputScreen = GetComponentInChildren<TextDisplay>();

        GameObject _interactableUI = transform.Find("ScreenInteractable").gameObject;
        _interactableUI.GetComponent<Canvas>().worldCamera = Game_Manager.instance._playerCamera;

        _buttonList = _interactableUI.transform.GetChild(0).gameObject;
        _buttonList.SetActive(false);

        base.OutputSetup();
    }

    public override void FinishDisplay()
    {
        _outputScreen.Clear();
        _buttonList.SetActive(false);
        base.FinishDisplay();
    }
}
