#if false //|| CT_DEVELOP
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Crosstales.TPB.Example
{
   /// <summary>Rebuilds all Addressables.</summary>
   [InitializeOnLoad]
   public abstract class AddressableRebuilder
   {
      #region Constructor

      static AddressableRebuilder()
      {
         Builder.OnBuildStart += onBuildStart;
      }

      private static void onBuildStart(BuildTarget target, string path, string name)
      {
         AddressableAssetSettings.CleanPlayerContent();
         AddressableAssetSettings.BuildPlayerContent();
      }

      #endregion
   }
}
#endif
#endif
// © 2021-2023 crosstales LLC (https://www.crosstales.com)