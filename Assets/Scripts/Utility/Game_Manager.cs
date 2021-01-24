using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The game manager script is a singleton used for safely and efficiently accessing the player game object and camera
//The values are assigned manually in the editor to ensure the correct objects are being used
[DisallowMultipleComponent]
public class Game_Manager : AutoCleanupSingleton<Game_Manager>
{
    [SerializeField] public GameObject _player = default;
    [SerializeField] public Camera _playerCamera = default;
    [SerializeField] public UIManager _UIManager = default;

   

}
