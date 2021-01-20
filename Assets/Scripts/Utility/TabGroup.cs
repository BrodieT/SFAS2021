using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script represents a group of tab buttons and is used to switch between the pages
//of a tabbed menu UI when selecting the associated tab
public class TabGroup : MonoBehaviour
{
    [SerializeField] Sprite _tabIdle = default; //The sprite displayed when the tab button is idle
    [SerializeField] Sprite _tabSelected = default; //The sprite displayed when selecting a tab button
    [SerializeField] Sprite _tabHover = default; //The sprite displayed when highlighting a tab button

    private TabButton _selectedTab = default; //The currently selected tab in this group  
    private List<TabButton> _tabButtons = new List<TabButton>(); //The list of tab buttons

    private void Start()
    {
        if (_tabButtons.Count > 0)
        {
            //Select the first tab by default 
            OnTabSelected(_tabButtons[0]);
        }
    }

    //This function is used to add a tab button to this tab group
    public void Subscribe(TabButton button)
    {
        if(_tabButtons == null)
        {
            _tabButtons = new List<TabButton>();
        }

        _tabButtons.Add(button);
    }
   
    //This function is called when the mouse pointer enters a tab
    public void OnTabEnter(TabButton button)
    {
        //Reset the tabs to idle
        ResetTabs();
        //If the tab the mouse entered is not the selected one, display the highlighted sprite
        if (_selectedTab != null && button != _selectedTab)
            button._background.sprite = _tabHover;
    }

    //This function is called when the mouse pointer exits a tab button to restore the idle states
    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    //This function is called when the mouse pointer clicks on a tab button
    public void OnTabSelected(TabButton button)
    {
        //Deselect the previously selected tab if there is one
        if (_selectedTab != null)
            _selectedTab.Deselect();

        //Assign the newly selected tab
        _selectedTab = button;

        //Reset the tabs and display the appropriate pages
        ResetTabs();
        //Update the background of the newly selected tab
        button._background.sprite = _tabSelected;
        button.Select();
    }

    //This function resets all non-selected tabs to the idle state and displays the currently selected tab's page
    public void ResetTabs()
    {
        //Loop through all the tabs in this group
        foreach (TabButton tab in _tabButtons)
        {
            //If not selected disable the page and set the sprite to idle
            if (_selectedTab != null && tab != _selectedTab)
            {
                tab._background.sprite = _tabIdle;
                tab._tabPage.SetActive(false);
            }
            else
            {
                //If selected and the page is inactive, enable it
                if (!tab._tabPage.activeSelf)
                    tab._tabPage.SetActive(true);
            }
        }
    }

}
