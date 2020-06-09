using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace VirtualPhenix
{
    /// <summary>
    /// Adds the given define symbols to PlayerSettings define symbols.
    /// Just add your own define symbols to the Symbols property at the below.
    /// </summary>
    [InitializeOnLoad]
    public class VP_AddDefineSymbols : Editor
    {
        /// <summary>
        /// Symbols that will be added to the editor
        /// </summary>
        public static readonly string[] Symbols = new string[] {
         "USE_VIRTUAL_PHENIX_DIALOGUE_SYSTEM"
        };

        public static readonly string[] External = new string[] {
         "DOTWEEN",
         "USE_TEXT_ANIMATOR",
         "USE_CINEMACHINE"
        };

        /// <summary>
        /// Add define symbols as soon as Unity gets done compiling.
        /// </summary>
        static VP_AddDefineSymbols()
        {

        }

        [MenuItem("Virtual Phenix/Define Symbols/Setup Define Symbols")]
        public static void AddDefine()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            allDefines.AddRange(Symbols.Except(allDefines));


            // Do tween pro
            string dotweenPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Demigiant\\DOTweenPro";
            if (System.IO.Directory.Exists(dotweenPath))
            {
                Debug.Log("Do Tween Pro is in the project. Adding DOTWEEN symbol.");
                allDefines.Add(External[0]);
            }
            else
            {
                Debug.Log($"Do Tween Pro could not be found in {dotweenPath}. if you have it in other path, please, change the path here.");
            }

            // Text Animator
            string textanimatorpath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Plugins\\Febucci\\Text Animator";
            if (System.IO.Directory.Exists(textanimatorpath))
            {
                Debug.Log("Text Animator is in the project. Adding USE_TEXT_ANIMATOR symbol.");
                allDefines.Add(External[1]);
            }
            else
            {
                Debug.Log($"Text Animator could not be found in {textanimatorpath}. if you have it in other path, please, change the path here.");
                textanimatorpath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Febucci\\Text Animator";
                if (System.IO.Directory.Exists(textanimatorpath))
                {
                    Debug.Log("Text Animator is in the project. Adding USE_TEXT_ANIMATOR symbol.");
                    allDefines.Add(External[1]);
                }
                else
                {
                    Debug.Log($"Text Animator could not be found in {textanimatorpath}. if you have it in other path, please, change the path here.");
                }
            }


            // Cinemachine
            string manifest = System.IO.Directory.GetCurrentDirectory() + "\\Packages\\manifest.json";
            if (System.IO.File.Exists(manifest))
            {
                string manifestJSONData = System.IO.File.ReadAllText(manifest);
                if (manifestJSONData.Contains("com.unity.cinemachine"))
                {
                    allDefines.Add(External[2]);
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));

        }

        [MenuItem("Virtual Phenix/Define Symbols/Remove Define Symbols")]
        public static void RemoveDefine()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            allDefines.AddRange(Symbols.Except(allDefines));
            foreach (string str in Symbols)
                allDefines.Remove(str);

            foreach (string str in External)
                allDefines.Remove(str);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup,
            string.Join(";", allDefines.ToArray()));
        }
    }
}

