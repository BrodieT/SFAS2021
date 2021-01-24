using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DiscoveryTrigger : MonoBehaviour
{
    [SerializeField] private string _discoveryText = "";
    [SerializeField] private bool _triggerExit = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!_triggerExit)
            Game_Manager.instance._UIManager._discoveryUI.Discover(new DiscoveryUI.Discovery(_discoveryText));
    }

    private void OnTriggerExit(Collider other)
    {
        if (_triggerExit)
            Game_Manager.instance._UIManager._discoveryUI.Discover(new DiscoveryUI.Discovery(_discoveryText));
    }
}
