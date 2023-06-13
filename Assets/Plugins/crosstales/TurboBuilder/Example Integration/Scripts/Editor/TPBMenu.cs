#if false || CT_DEVELOP
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Crosstales.TPB.Example
{
   /// <summary>Example editor menu integration of Turbo Builder for your own scripts.</summary>
   public static class TPBMenu
   {
      [MenuItem("Tools/Build Windows #&w", false, 20)]
      public static void BuildWindows()
      {
         Debug.Log("Build StandaloneWindows64");

         if (!Builder.Build(BuildTarget.StandaloneWindows64, null, "spacechef"))
            Debug.LogError("Could not build for Windows!");
      }

      [MenuItem("Tools/Build Android #&m", false, 30)]
      public static void BuildAndroid()
      {
         Debug.Log("Build Android");

         if (!Builder.Build(BuildTarget.Android))
            Debug.LogError("Could not build for Android!");
      }

      [MenuItem("Tools/Build All", false, 60)]
      public static void BuildAll()
      {
         Debug.Log("Build All");

         BuildWindows();
         BuildAndroid();

         //if (!Builder.BuildAll())
         //   Debug.LogError("Could not build all platforms!");
      }
   }
}
#endif
#endif
// © 2019-2023 crosstales LLC (https://www.crosstales.com)