#if false || CT_DEVELOP
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Crosstales.TPB.Example
{
   /// <summary>Simple test script for all callbacks.</summary>
   [InitializeOnLoad]
   public abstract class EventTester
   {
      #region Constructor

      static EventTester()
      {
         Builder.OnBuildingStart += onBuildingStart;
         Builder.OnBuildingComplete += onBuildingComplete;
         Builder.OnBuildStart += onBuildStart;
         Builder.OnBuildComplete += onBuildComplete;
         Builder.OnBuildAllStart += onBuildAllStart;
         Builder.OnBuildAllComplete += onBuildAllComplete;
      }

      #endregion


      #region Callbacks

      private static void onBuildingStart()
      {
         Debug.Log("Building started");
      }

      private static void onBuildingComplete(bool success)
      {
         Debug.Log($"Building completed: {success}");
      }

      private static void onBuildStart(BuildTarget target, string path, string name)
      {
         //EditorUtility.DisplayDialog($"onBuildStart {target}", path, "OK");

         //UnityEditor.PlayerSettings.bundleVersion = "9000.0.0";
         Debug.Log($"Build start: {target} - {path} - {name}");
      }

      private static void onBuildComplete(BuildTarget target, string path, bool success)
      {
         //EditorUtility.DisplayDialog($"onBuildComplete {target}", path, "OK");

         Debug.Log($"Build complete: {target} - {path} - {success}");
      }

      private static void onBuildAllStart()
      {
         Debug.Log("BuildAll start");
      }

      private static void onBuildAllComplete(bool success)
      {
         Debug.Log($"BuildAll complete: {success}");
      }

      #endregion
   }
}
#endif
#endif
// © 2020-2023 crosstales LLC (https://www.crosstales.com)