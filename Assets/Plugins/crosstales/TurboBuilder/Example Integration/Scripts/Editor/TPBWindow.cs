#if false || CT_DEVELOP
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Crosstales.TPB.Util;

namespace Crosstales.TPB.EditorIntegration
{
   /// <summary>Example editor window integration of Turbo Builder for your own scripts.</summary>
   //[InitializeOnLoad]
   public class TPBWindow : ConfigWindow //TODO complete the integration
   {
      #region EditorWindow methods

      [MenuItem("Window/Custom TPB", false, 1100)]
      public static void ShowCustomWindow()
      {
         GetWindow(typeof(TPBWindow));
      }

      private void OnEnable()
      {
         titleContent = new GUIContent("Custom TPB", Helper.Logo_Asset_Small);

         init();
      }

      private void OnGUI()
      {
         showBuild();
      }

      #endregion
   }
}
#endif
#endif
// © 2021-2023 crosstales LLC (https://www.crosstales.com)