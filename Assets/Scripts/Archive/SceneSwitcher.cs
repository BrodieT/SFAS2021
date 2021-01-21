using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] public SceneLoader.SceneName _targetScene = 0;
    public void GoToTargetScene()
    {
        SceneLoader.instance.LoadLevel(_targetScene);
    }
}
