using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Animator))]
public class DiscoveryUI : MonoBehaviour
{
    [SerializeField] private Animator _anim = default;
    [SerializeField] private TMP_Text _mainText = default;
    [SerializeField] private TMP_Text _subText = default;

    public struct Discovery
    {
        public string _text;
        public bool _isAchivement;

        public Discovery(string text, bool achievement = false)
        {
            _text = text;
            _isAchivement = achievement;
        }
    }

    Queue<Discovery> _discoveries = new Queue<Discovery>();
    private bool _isReady = true;
    public void Discover(Discovery discover)
    {
        _discoveries.Enqueue(discover);        
    }

    private void ShowDiscovery(Discovery discover)
    {
        _isReady = false;
        _mainText.text = discover._text;

        if(discover._isAchivement)
            _subText.text = "Unlocked";
        else
            _subText.text = "Discovered";

        _anim.SetTrigger("Discovery");
        Invoke("ResetTrigger", 5.0f);
    }


    private void Update()
    {
        if (_isReady && _discoveries.Count > 0)
            ShowDiscovery(_discoveries.Dequeue());
    }

    private void ResetTrigger()
    {
        _anim.ResetTrigger("Discovery");
        _isReady = true;
    }
}
