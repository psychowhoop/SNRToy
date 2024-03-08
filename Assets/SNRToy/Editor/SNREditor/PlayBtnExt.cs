using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class PlayDefaultSc
{
    private const string mDefPathKey = "KSNRDefaultScenePath"; // 默认场景路径保存的键值

    private static string mDefPath = "Assets/Boot/BootSc"; // 默认场景路径

    private static string scenePath = mDefPath;
    private static bool showEdit = false; // 是否显示输入框

    static PlayDefaultSc()
    {
        scenePath = EditorPrefs.GetString(mDefPathKey, mDefPath); // 从 EditorPrefs 中获取默认场景路径
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();

        float buttonWidth = 60f;
        float buttonHeight = 30f;
        float margin = 10f;
        Rect buttonRect = new Rect(margin, margin, buttonWidth, buttonHeight);

        // 绘制播放按钮
        if (GUI.Button(buttonRect, "Play"))
        {
            Debug.Log("Custom button clicked!");
            PlayCustomScene();
        }

        // 添加场景路径输入框
        if (showEdit)
        {
            Rect labelRect = new Rect(margin, margin + buttonHeight + margin, 40f, 20f);
            EditorGUI.LabelField(labelRect, "Path:");

            Rect textFieldRect = new Rect(labelRect.xMax, margin + buttonHeight + margin, 160f, 20f);
            scenePath = EditorGUI.TextField(textFieldRect, scenePath);
        }

        Handles.EndGUI();
    }

    private static void PlayCustomScene()
    {
        scenePath = string.IsNullOrEmpty(scenePath) ? mDefPath : scenePath;
        // 保存用户设定的场景路径到 EditorPrefs 中
        EditorPrefs.SetString(mDefPathKey, scenePath);

        // 保存当前场景
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        // 关闭当前场景
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

        // 加载指定场景
        EditorSceneManager.OpenScene(scenePath + ".unity");
        // 开始播放
        EditorApplication.isPlaying = true;
    }

    [MenuItem("SNRHelper/ShowDefScEdit")]
    private static void ToggleShowEdit()
    {
        showEdit = !showEdit;
        SceneView.RepaintAll();
    }
}
