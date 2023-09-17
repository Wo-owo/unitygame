using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]

public class SceneNameDrawer : PropertyDrawer
{
    int sceneIndex = -1;

    GUIContent[] sceneNames;

    readonly string[] scenePathSplit = { "/", ".unity" };
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (EditorBuildSettings.scenes.Length == 0) return;
        if(sceneIndex == -1)
            GetSceneNameArray(property);
        int oldIndex = sceneIndex;
        sceneIndex = EditorGUI.Popup(position, label, sceneIndex, sceneNames);
        if (oldIndex != sceneIndex)
            property.stringValue = sceneNames[sceneIndex].text;
    }

    private void GetSceneNameArray(SerializedProperty property)
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        //��ʼ������
        sceneNames = new GUIContent[scenes.Length];

        for(int i = 0; i < scenes.Length; i++)
        {
            string path = scenes[i].path;
            string[] splitPath = path.Split(scenePathSplit, System.StringSplitOptions.RemoveEmptyEntries);

            string sceneName = "";
            if(splitPath.Length > 0)
            {
                sceneName = splitPath[splitPath.Length - 1];
            }
            else
            {
                sceneName = "(Deleted Scene)";
            }
            sceneNames[i]=new GUIContent(sceneName);
        }
        if(scenePathSplit.Length == 0)
        {
            sceneNames = new[] { new GUIContent("Check Your Build Setting") };
        }

        if(!string.IsNullOrEmpty(property.stringValue))
        {
            bool namefound = false;
            for(int i = 0; i < sceneNames.Length; i++)
            {
                if (sceneNames[i].text == property.stringValue)
                {
                    sceneIndex = i;
                    namefound = true;
                    break;
                }
                if (namefound == false)
                {
                    sceneIndex = 0;
                }
            }
        }
        else
        {
            sceneIndex = 0;
        }
        property.stringValue = sceneNames[sceneIndex].text;
    }
}
