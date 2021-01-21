using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

//This script is used to switch between scenes at runtime.
public class SceneLoader : AutoCleanupSingleton<SceneLoader>
{
    [SerializeField] Canvas _loadingScreenPrefab = default; //The loading screen prefab to be instatiated when loading a new scene
    private TMP_Text _loadingTxt = default; //The text displayed on the loading screen
    private string _elipses = ""; //The elipses that will be placed at the end of the loading screen text
    private Canvas _currentLoadingScreen = default; //The instatiated version of the loading screen
    public static bool _isFinishedLoading = false; //bool to track is the current scene load process is finished
    private List<SceneData> _allScenes = new List<SceneData>(); //A list of all possible scenes and the corresponding build index

    [System.Serializable] 
    public enum SceneName { None = 0, MainMenu = 1, Warehouse = 2, City = 3, Sewers = 4, Shop = 5, Palace = 6, End = 7} //Identifiers for each scene to be loaded
    
    [System.Serializable] 
    //Struct to link the scene name component with the build index of the scene
    public struct SceneData
    {
        public SceneName _sceneName;
        public int _sceneIndex;
    
        public SceneData(SceneName name, int index)
        {
            _sceneName = name;
            _sceneIndex = index;
        }
    
    }
  
    
    // Start is called before the first frame update
    void Awake()
    {
        //Ensure this is carried between scenes
        DontDestroyOnLoad(this.gameObject);
    }

    //This function will begin loading the desired scene
    public void LoadLevel(SceneName scene)
    {
        if (scene == SceneName.None)
            return;

        _isFinishedLoading = false;
        //Instantiate the loading screen and get the text component
        _currentLoadingScreen = Instantiate(_loadingScreenPrefab, new Vector3(0, 0, 10), Quaternion.identity);
        _loadingTxt = _currentLoadingScreen.GetComponentInChildren<TMP_Text>();     
        //Ensure the loading screen is not destroyed when changing scenes
        DontDestroyOnLoad(_currentLoadingScreen.gameObject);
        //Start updating the Text on the loading screen
        StartCoroutine(UpdateLoadingUI());
        //begin the load operation
        StartCoroutine(LoadAsync(_allScenes.Find(x => x._sceneName == scene)._sceneIndex));
    }

    //This coroutine will update the loading screen text to animate the elipses 
    IEnumerator UpdateLoadingUI()
    {
        int secondCounter = 0;
        
        //This will add a "." to the end of the "Loading" string each second up to a maximum of 3 
        //at which point it will remove the elipses and start again.
        //This is to animate the loading screen slightly to assure the player it is not frozen
        while (true)
        {
            if (_elipses.Length == 3)
            {
                _elipses = "";
            }
            else
            {
                _elipses += ".";
            }

            if (_loadingTxt != null)
                _loadingTxt.text = "Loading" + _elipses;
           
            yield return new WaitForSeconds(1.0f);
            secondCounter++;
        }
    }

    //This coroutine will load the desired scene
    IEnumerator LoadAsync(int scene)
    {
        //Artificially increase the load time slightly to prevent the loading screen from flashing up too quickly
        yield return new WaitForSeconds(5.0f);
        //begin loading
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        while (!operation.isDone)
        {
            //Any desired functionality for while the loading is taking place goes here
            //E.g. updating a percentage bar
            yield return null;
        }

        //Delay the removal of the loading screen to ensure everything in the new scene is in place
        yield return new WaitForSeconds(2.0f);

        //Remove the load screen and finish the process
        _currentLoadingScreen.gameObject.SetActive(false);
        _isFinishedLoading = true;


        yield return new WaitForSeconds(2.0f);

        //Cleanup
        Destroy(_currentLoadingScreen.gameObject);
        StopAllCoroutines();
    }
}
