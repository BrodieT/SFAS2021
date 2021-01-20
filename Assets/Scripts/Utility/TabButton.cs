using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//This script is used to identify a tab button for a tabbed menu UI
[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] TabGroup _tabGroup = default; //The tab group linked to this tab button
    [SerializeField] public GameObject _tabPage = default; //The page this tab button will toggle
    [SerializeField] UnityEvent _onSelected = default; //Unity event for what happens when this tab button is selected
    [SerializeField] UnityEvent _onDeselected = default; //Unity event for what happens when a different tab button is selected
    public Image _background { get; set; } //The background image of this tab button - to be changed according to its state (i.e. highlighted, selected, idle)


    private void Start()
    {
        //Get the background image component
        _background = GetComponent<Image>();
        //Subscribe this tab button to its linked tab group
        _tabGroup.Subscribe(this);
    }

    //This function lets the tab group know when the mouse pointer enters this tab button
    public void OnPointerEnter(PointerEventData eventData)
    {
        _tabGroup.OnTabEnter(this);
    }

    //This function lets the tab group know when the mouse pointer selects this tab button
    public void OnPointerClick(PointerEventData eventData)
    {
        _tabGroup.OnTabSelected(this);
    }

    //This function lets the tab group know when the mouse pointer exits this tab button
    public void OnPointerExit(PointerEventData eventData)
    {
        _tabGroup.OnTabExit(this);
    }

    //This function is called when the tab is selected to trigger a unity event
    public void Select()
    {
        _onSelected?.Invoke();
    }

    //This function is called when a different tab is selected to trigger a unity event
    public void Deselect()
    {
        _onDeselected?.Invoke();
    }
}
