#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

namespace Crosstales.TPB.Task
{
   /// <summary>Show the configuration window on the first launch.</summary>
   public class Launch : AssetPostprocessor
   {
      public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
      {
         if (importedAssets.Any(str => str.Contains(Crosstales.TPB.Util.Constants.ASSET_UID.ToString())))
         {
            Crosstales.Common.EditorTask.SetupResources.Setup();
            SetupResources.Setup();

            EditorIntegration.ConfigWindow.ShowWindow(3);
         }
      }
   }
}
#endif
// © 2018-2023 crosstales LLC (https://www.crosstales.com)