#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NutriAR.Editor
{
    public static class ProjectSetup
    {
        private const string ScenePath = "Assets/Scenes/NutriAR.unity";

        [MenuItem("NutriAR/Preparar projeto")]
        public static void Configure()
        {
            Directory.CreateDirectory("Assets/Scenes");

            if (!File.Exists(ScenePath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, ScenePath);
            }

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };

            PlayerSettings.companyName = "NutriAR Educacao";
            PlayerSettings.productName = "NutriAR";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "br.edu.nutriar.app");
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, "br.edu.nutriar.app");
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.iOS.cameraUsageDescription = "O NutriAR usa a câmera para enquadrar alimentos e rótulos nutricionais.";

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("NutriAR: projeto configurado e cena principal pronta.");
        }
    }
}
#endif

