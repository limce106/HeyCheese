//using UnityEngine;
//using UnityEditor;
//using UnityEditor.SceneManagement;
//using System.Collections.Generic;
//using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(SceneLoader))]
public class SceneLoaderEditor : Editor
{
    //private string[] sceneNames;
    //private int selectedIndex;

    //public override void OnInspectorGUI()
    //{
    //    SceneLoader loader = (SceneLoader)target;

    //    // Get scenes from build settings
    //    int sceneCount = SceneManager.sceneCountInBuildSettings;
    //    List<string> sceneList = new List<string>();

    //    for (int i = 0; i < sceneCount; i++)
    //    {
    //        string path = SceneUtility.GetScenePathByBuildIndex(i);
    //        string name = Path.GetFileNameWithoutExtension(path);
    //        sceneList.Add(name);
    //    }

    //    sceneNames = sceneList.ToArray();

    //    // Match current sceneToLoad
    //    selectedIndex = Mathf.Max(0, System.Array.IndexOf(sceneNames, loader.sceneToLoad));
    //    selectedIndex = EditorGUILayout.Popup("Scene To Load", selectedIndex, sceneNames);
    //    loader.sceneToLoad = sceneNames[selectedIndex];

    //    if (GUI.changed)
    //    {
    //        EditorUtility.SetDirty(loader);
    //    }
    //}

    /// ½Ãµµ2

    //private string[] sceneNames;
    //private int selectedIndex = -1;

    //void OnEnable()
    //{
    //    List<string> sceneList = new List<string>();
    //    int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

    //    for (int i = 0; i < sceneCount; i++)
    //    {
    //        string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
    //        string name = System.IO.Path.GetFileNameWithoutExtension(path);
    //        sceneList.Add(name);
    //    }

    //    sceneNames = sceneList.ToArray();

    //    SceneLoader loader = (SceneLoader)target;
    //    selectedIndex = System.Array.IndexOf(sceneNames, loader.sceneToLoad);
    //    if (selectedIndex < 0) selectedIndex = 0;
    //}

    //public override void OnInspectorGUI()
    //{
    //    SceneLoader loader = (SceneLoader)target;

    //    EditorGUILayout.LabelField("Select Scene to Load", EditorStyles.boldLabel);

    //    if (sceneNames.Length > 0)
    //    {
    //        selectedIndex = EditorGUILayout.Popup("Scene", selectedIndex, sceneNames);
    //        loader.sceneToLoad = sceneNames[selectedIndex];
    //    }
    //    else
    //    {
    //        EditorGUILayout.HelpBox("No scenes in build settings!", MessageType.Warning);
    //    }

    //    DrawDefaultInspector(); // Show LoadScene button if needed
    //}
}
