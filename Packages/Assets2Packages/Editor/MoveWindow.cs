using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Packages.Assets2Packages.Editor
{
    public class MoveWindow  : EditorWindow
    {
        const string SamplePackage = "com.unity.sample";

        string category = "Unity/AssetStore";
        string description = "";
        string displayName = "";
        Object folder;
        string keywords = "";
        string packageName = SamplePackage;
        string unity = "2018.2";
        string version = "1.0.0";

        [MenuItem("Window/Assets 2 Packages")]
        static void Init()
        {
            var window = (MoveWindow) GetWindow(typeof(MoveWindow));
            window.Show();
            window.titleContent = new GUIContent("Assets 2 Packages");
        }

        bool CheckSettings()
        {
            return folder != null && string.IsNullOrEmpty(packageName) == false && packageName != SamplePackage;
        }

        void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            var newFolder = EditorGUILayout.ObjectField("Folder", folder, typeof(Object), false);
            var path = AssetDatabase.GetAssetPath(newFolder);
            if (path.StartsWith("Packages/")) path = string.Empty;
            folder = AssetDatabase.IsValidFolder(path) ? newFolder : null;
            packageName = EditorGUILayout.TextField("Package Name", packageName);
            displayName = EditorGUILayout.TextField("Display Name", displayName);
            category = EditorGUILayout.TextField("Category", category);
            description = EditorGUILayout.TextField("Description", description);
            unity = EditorGUILayout.TextField("Unity", unity);
            version = EditorGUILayout.TextField("Version", version);
            keywords = EditorGUILayout.TextField("Keywords", keywords);

            EditorGUI.BeginDisabledGroup(CheckSettings() == false);
            if (GUILayout.Button("Move Start")) Move();
            EditorGUI.EndDisabledGroup();
        }

        void Move()
        {
            var unityPackageInfo = new UnityPackageInfo
            {
                category = category,
                description = description,
                keywords = keywords.Split(','),
                name = packageName.ToLower(),
                displayName = displayName,
                unity = unity,
                version = version
            };
            var json = JsonUtility.ToJson(unityPackageInfo);

            var packageDirectory = Path.Combine(Application.dataPath, $"../Packages/{packageName}");

            var path = Path.Combine(Application.dataPath, AssetDatabase.GetAssetPath(folder).Substring("Assets/".Length));

            Directory.Move(path, packageDirectory);

            using (var writer = new StreamWriter(packageDirectory + "/package.json", false))
            {
                writer.WriteLine(json);
            }

            AssetDatabase.Refresh();
            Close();
        }

    
    }
}
