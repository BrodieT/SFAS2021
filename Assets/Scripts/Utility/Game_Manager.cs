using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : AutoCleanupSingleton<Game_Manager>
{
    [SerializeField] public GameObject _player = default;
    [SerializeField] public Camera _playerCamera = default;
}
