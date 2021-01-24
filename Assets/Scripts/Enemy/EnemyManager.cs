using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is used by the progress tracker to remove already dead enemies when loading into a scene
public class EnemyManager : AutoCleanupSingleton<EnemyManager>
{
    public List<GameObject> _allEnemies = new List<GameObject>(); //A list of all enemies in the scene

    //Add a newly defeated enemy to the progress tracker using the index in this list
    public void Killed(GameObject obj)
    {
        ProgressionTracker.instance.AddDefeatedEnemy(_allEnemies.FindIndex(x => x == obj));
    }

}
