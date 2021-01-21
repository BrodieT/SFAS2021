using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This interactable type covers both NPC dialogue as well as the terminal systems in the game
public class NarrativeInteract : Interactable
{
    [SerializeField] Transform _cameraPosition = default;
    [SerializeField] Transform _playerPosition = default;


    public override void DoInteract()
    {
        //Disable player movement
        GameUtility._isPlayerObjectBeingControlled = false;

        //Disable the prompt
        Game_Manager.instance._UIManager.GetInteractPrompt().gameObject.SetActive(false);
        
        PlayerAutoPilot _autopilot = null;

        if (Game_Manager.instance._player.TryGetComponent<PlayerAutoPilot>(out _autopilot))
        {
            //Use the autopilot to move to the correct interact transforms
            _autopilot.BeginAutoPilot(_playerPosition.position, _playerPosition.rotation, _cameraPosition.position, _cameraPosition.rotation);
        }
        else
        {
            Debug.LogWarning("No Autopilot found. Manually Adding one now.");
            _autopilot = Game_Manager.instance._player.AddComponent(typeof(PlayerAutoPilot)) as PlayerAutoPilot;
        }

        //Wait for the autopilot to finish
        StartCoroutine(IsTerminalReady(_autopilot));

        }

    //This coroutine waits for the player autopilot to reach the destination then begins the terminal display
    //NB: This applies to NPC dialogue as well
    IEnumerator IsTerminalReady(PlayerAutoPilot _autopilot)
    {
        bool _ready = false;

        while (!_ready)
        {
            if (!_autopilot._isAutoPiloting)
            {
                if (transform == null)
                {
                    Debug.LogError("Terminal Interactable becoming null before player was in position");
                    _ready = true;
                }

                //Prepare the dialogue manager on the terminal to begin the story
                BranchingNarrative narrative;
                GameUtility.ShowCursor();

                if (transform.TryGetComponent(out narrative))
                {
                    narrative.StartDisplay();
                }
                else
                {
                    Debug.LogError("No Branching Narrative found on laptop interactable");
                }

                _ready = true;

            }

            yield return null;
        }
    }

}
