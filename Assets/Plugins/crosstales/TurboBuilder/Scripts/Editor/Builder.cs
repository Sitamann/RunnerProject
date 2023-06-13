#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using UnityEditor;
using Crosstales.TPB.Util;
using Crosstales.Common.Util;
using Ionic.Zip; // this uses the Unity port of DotNetZip https://github.com/r2d2rigo/dotnetzip-for-unity

namespace Crosstales.TPB
{
   /// <summary>Platform builder.</summary>
   public static class Builder
   {
      #region Variables

      private const bool deleteBuild = true; //CHANGE: set to true you want to delete the previous build

      #endregion


      #region Properties

      /// <summary>The current build target.</summary>
      public static BuildTarget CurrentBuildTarget { get; private set; } = BuildTarget.NoTarget;

      /// <summary>True if the Builder is busy.</summary>
      public static bool isBusy { get; private set; }

      #endregion


      #region Events

      public delegate void BuildingStart();

      public delegate void BuildingComplete(bool success);

      public delegate void BuildStart(BuildTarget target, string path, string name);

      public delegate void BuildComplete(BuildTarget target, string path, bool success);

      public delegate void BuildAllStart();

      public delegate void BuildAllComplete(bool success);

      /// <summary>An event triggered before the build process starts.</summary>
      public static event BuildingStart OnBuildingStart;

      /// <summary>An event triggered after the build process is completed.</summary>
      public static event BuildingComplete OnBuildingComplete;

      /// <summary>An event triggered whenever a build is started.</summary>
      public static event BuildStart OnBuildStart;

      /// <summary>An event triggered whenever a build is completed.</summary>
      public static event BuildComplete OnBuildComplete;

      /// <summary>An event triggered whenever the "BuildAll"-method is started.</summary>
      public static event BuildAllStart OnBuildAllStart;

      /// <summary>An event triggered whenever the "BuildAll"-method is completed.</summary>
      public static event BuildAllComplete OnBuildAllComplete;

      #endregion


      #region Public methods

      /// <summary>Builds the given target.</summary>
      /// <param name="target">Build target</param>
      /// <param name="path">Build path (optional)</param>
      /// <param name="name">Name of the build artifact (optional)</param>
      /// <param name="scenes">Scenes for the build (optional)</param>
      /// <returns>True if the build was successful.</returns>
      public static bool Build(BuildTarget target, string path = null, string name = null, params string[] scenes)
      {
         CTPlayerPrefs.SetBool(Constants.KEY_BATCHMODE, false);
         CTPlayerPrefs.Save();

         return build(false, path, name, new[] { Helper.GetBuildNameFromBuildTarget(target) }, scenes);
      }

      /// <summary>Builds all selected targets.</summary>
      /// <param name="path">Build path (optional)</param>
      /// <param name="name">Name of the build artifact (optional)</param>
      /// <param name="scenes">Scenes for the build (optional)</param>
      /// <returns>True if the builds were successful.</returns>
      public static bool BuildAll(string path = null, string name = null, params string[] scenes)
      {
         OnBuildAllStart?.Invoke();

         if (!string.IsNullOrEmpty(Config.EXECUTE_METHOD_PRE_BUILD_ALL))
            Helper.InvokeMethod(Config.EXECUTE_METHOD_PRE_BUILD_ALL.Substring(0, Config.EXECUTE_METHOD_PRE_BUILD_ALL.CTLastIndexOf(".")), Config.EXECUTE_METHOD_PRE_BUILD_ALL.Substring(Config.EXECUTE_METHOD_PRE_BUILD_ALL.CTLastIndexOf(".") + 1));

         CTPlayerPrefs.SetBool(Constants.KEY_BATCHMODE, false);
         CTPlayerPrefs.Save();

         System.Collections.Generic.List<string> _targets = new System.Collections.Generic.List<string>();

         foreach (BuildTarget t in Helper.Targets)
         {
            _targets.Add(Helper.GetBuildNameFromBuildTarget(t));
         }

         bool success = build(false, path, name, _targets.ToArray(), scenes);

         OnBuildAllComplete?.Invoke(success);

         if (!string.IsNullOrEmpty(Config.EXECUTE_METHOD_POST_BUILD_ALL))
            Helper.InvokeMethod(Config.EXECUTE_METHOD_POST_BUILD_ALL.Substring(0, Config.EXECUTE_METHOD_POST_BUILD_ALL.CTLastIndexOf(".")), Config.EXECUTE_METHOD_POST_BUILD_ALL.Substring(Config.EXECUTE_METHOD_POST_BUILD_ALL.CTLastIndexOf(".") + 1));

         return success;
      }

      /// <summary>Builds all selected targets via CLI.</summary>
      public static void BuildAllCLI()
      {
         CTPlayerPrefs.SetBool(Constants.KEY_BATCHMODE, true);
         CTPlayerPrefs.Save();

         System.Collections.Generic.List<string> _targets = new System.Collections.Generic.List<string>();

         foreach (BuildTarget t in Helper.Targets)
         {
            _targets.Add(Helper.GetBuildNameFromBuildTarget(t));
         }

         //TODO add parameters like "scenes"

         string _path = Helper.GetArgument("-tpbPath");
         string _name = Helper.GetArgument("-tpbName");

         build(true, _path, _name, _targets.ToArray());
      }

      /// <summary>Builds the targets via CLI.</summary>
      public static void BuildCLI()
      {
         CTPlayerPrefs.SetBool(Constants.KEY_BATCHMODE, true);
         CTPlayerPrefs.Save();

         //TODO add parameters like "scenes"

         string[] _targets = Helper.GetArgument("-tpbTargets").Split(new[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
         string _path = Helper.GetArgument("-tpbPath");
         string _name = Helper.GetArgument("-tpbName");

         build(true, _path, _name, _targets);
      }

#if CT_TPS
      /// <summary>Builds the current target via TPS.</summary>
      public static void BuildTPS()
      {
         if (CTPlayerPrefs.HasKey(Constants.KEY_BATCHMODE))
         {
            bool forceBatchmode = CTPlayerPrefs.GetBool(Constants.KEY_BATCHMODE);

            if (CTPlayerPrefs.HasKey(Constants.KEY_TARGETS))
            {
               string[] targets = CTPlayerPrefs.GetString(Constants.KEY_TARGETS).Split(';');

               if (targets.Length > 0)
               {
                  if (targets.Length == 1)
                  {
                     CTPlayerPrefs.SetString(Constants.KEY_TARGETS, string.Empty);
                  }
                  else
                  {
                     string result = targets[1];

                     for (int ii = 2; ii < targets.Length; ii++)
                     {
                        result += ";" + targets[ii];
                     }

                     CTPlayerPrefs.SetString(Constants.KEY_TARGETS, result);
                  }

                  CTPlayerPrefs.Save();

                  //CTLogger.Log(EditorUserBuildSettings.activeBuildTarget.ToString());

                  //bool forceBatchmode = "true".CTEquals(Helper.GetArgument("-tpsBatchmode")) ? true : false;
                  string filename = Application.productName; //CHANGE: set your desired filename
                  buildPlayer(EditorUserBuildSettings.activeBuildTarget, filename, Config.PATH_BUILD, null); //TODO set filenamem path and scenes?

                  //CTLogger.Log("targets[0]: '" + targets[0] + "' - " + targets.Length + " - " + forceBatchmode);

                  if (!string.IsNullOrEmpty(targets[0]))
                  {
                     Helper.ProcessBuildPipeline(targets[0], targets.Length > 1 || forceBatchmode);

                     //CTLogger.Log("Process");
                  }
                  else
                  {
                     //CTLogger.Log("Finish");

                     OnBuildingComplete?.Invoke(true);

                     if (!string.IsNullOrEmpty(Config.EXECUTE_METHOD_POST_BUILDING))
                        Helper.InvokeMethod(Config.EXECUTE_METHOD_POST_BUILDING.Substring(0, Config.EXECUTE_METHOD_POST_BUILDING.LastIndexOf(".")), Config.EXECUTE_METHOD_POST_BUILDING.Substring(Config.EXECUTE_METHOD_POST_BUILDING.LastIndexOf(".") + 1));

                     if (forceBatchmode)
                        EditorApplication.Exit(0);
                  }
               }
            }
         }
      }
#endif

      /// <summary>Test building with an execute method.</summary>
      public static void SayHello()
      {
         Debug.LogError("Hello everybody, I was called by " + Constants.ASSET_NAME);

         if (Config.DEBUG)
            Debug.Log("CurrentBuildTarget: " + CurrentBuildTarget);
      }

      /// <summary>Test method (before building).</summary>
      public static void MethodBeforeBuilding()
      {
         Debug.LogWarning("'MethodBeforeBuilding' was called!");
      }

      /// <summary>Test method (after building).</summary>
      public static void MethodAfterBuilding()
      {
         Debug.LogWarning("'MethodAfterBuilding' was called!");
      }

      /// <summary>Test method (before a build).</summary>
      public static void MethodBeforeBuild()
      {
         Debug.LogWarning("'MethodBeforeBuild' was called!");
      }

      /// <summary>Test method (after a build).</summary>
      public static void MethodAfterBuild()
      {
         Debug.LogWarning("'MethodAfterBuild' was called: " + CurrentBuildTarget);
      }

      /// <summary>Test method (before build all).</summary>
      public static void MethodBeforeBuildAll()
      {
         Debug.LogWarning("'MethodBeforeBuildAll' was called!");
      }

      /// <summary>Test method (after build all).</summary>
      public static void MethodAfterBuildAll()
      {
         Debug.LogWarning("'MethodAfterBuildAll' was called!");
      }

      #endregion


      #region Private methods

      private static bool build(bool quit, string path, string name, string[] targets, params string[] scenes)
      {
         if (Config.AUTO_SAVE)
         {
            if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty)
               UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
         }
         else
         {
            if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
               Debug.LogWarning("User canceled the build.");
               return false;
            }
         }

         isBusy = true;

         OnBuildingStart?.Invoke();

         if (!string.IsNullOrEmpty(Config.EXECUTE_METHOD_PRE_BUILDING))
            Helper.InvokeMethod(Config.EXECUTE_METHOD_PRE_BUILDING.Substring(0, Config.EXECUTE_METHOD_PRE_BUILDING.CTLastIndexOf(".")), Config.EXECUTE_METHOD_PRE_BUILDING.Substring(Config.EXECUTE_METHOD_PRE_BUILDING.CTLastIndexOf(".") + 1));

         string _path = string.IsNullOrEmpty(path) ? Config.PATH_BUILD : path;

         string filename = Application.productName; //CHANGE: set your desired filename
         string _filename = string.IsNullOrEmpty(name) ? filename : name;

         Helper.SetupVCS();

         bool success = true;

         System.Collections.Generic.List<string> btList = new System.Collections.Generic.List<string>();

         //optimization: always use active platform first
         foreach (string target in targets)
         {
            BuildTarget bt = Helper.GetBuildTargetForBuildName(target);

            if (EditorUserBuildSettings.activeBuildTarget == bt)
            {
               success = buildPlayer(bt, _filename, _path, scenes); //TODO quit??
            }
            else
            {
               btList.Add(target);
            }
         }
#if CT_TPS
         if (Crosstales.TPS.Util.Config.USE_LEGACY)
         {
            if (btList.Count > 1)
            {
               string result = btList[1];

               for (int ii = 2; ii < btList.Count; ii++)
               {
                  result += ";" + btList[ii];
               }

               CTPlayerPrefs.SetString(Constants.KEY_TARGETS, result);
            }
            else
            {
               CTPlayerPrefs.SetString(Constants.KEY_TARGETS, string.Empty);

               if (btList.Count == 0)
               {
                  OnBuildingComplete?.Invoke(success);

                  if (!string.IsNullOrEmpty(Config.EXECUTE_METHOD_POST_BUILDING))
                     Helper.InvokeMethod(Config.EXECUTE_METHOD_POST_BUILDING.Substring(0, Config.EXECUTE_METHOD_POST_BUILDING.LastIndexOf(".")), Config.EXECUTE_METHOD_POST_BUILDING.Substring(Config.EXECUTE_METHOD_POST_BUILDING.LastIndexOf(".") + 1));
               }
            }

            CTPlayerPrefs.Save();

            //CTLogger.Log("btList[0]: '" + btList[0] + "' - " + btList.Count + " - " + quit);

            if (btList.Count > 0)
               Helper.ProcessBuildPipeline(btList[0], btList.Count > 1 || quit);
         }
         else
         {
            foreach (string target in btList)
            {
               Crosstales.TPS.Switcher.Switch(Helper.GetBuildTargetForBuildName(target));
               success = buildPlayer(Helper.GetBuildTargetForBuildName(target), _filename, _path, scenes);
            }

            OnBuildingComplete?.Invoke(success);

            if (!string.IsNullOrEmpty(Config.EXECUTE_METHOD_POST_BUILDING))
               Helper.InvokeMethod(Config.EXECUTE_METHOD_POST_BUILDING.Substring(0, Config.EXECUTE_METHOD_POST_BUILDING.LastIndexOf(".")), Config.EXECUTE_METHOD_POST_BUILDING.Substring(Config.EXECUTE_METHOD_POST_BUILDING.LastIndexOf(".") + 1));

            isBusy = false;

            if (quit)
               EditorApplication.Exit(0);
         }
#else
         foreach (string target in btList.Where(target => !buildPlayer(Helper.GetBuildTargetForBuildName(target), _filename, _path, scenes)))
         {
            success = false;
         }

         OnBuildingComplete?.Invoke(success);

         if (!string.IsNullOrEmpty(Config.EXECUTE_METHOD_POST_BUILDING))
            Helper.InvokeMethod(Config.EXECUTE_METHOD_POST_BUILDING.Substring(0, Config.EXECUTE_METHOD_POST_BUILDING.CTLastIndexOf(".")), Config.EXECUTE_METHOD_POST_BUILDING.Substring(Config.EXECUTE_METHOD_POST_BUILDING.CTLastIndexOf(".") + 1));

         isBusy = false;

         if (quit)
            EditorApplication.Exit(0);
#endif

         return success;
      }

      /// <summary>Builds the platform target.</summary>
      /// <param name="target">Target platform for the build</param>
      /// <param name="name">File name of the build.</param>
      /// <param name="path">Path for the build.</param>
      /// <param name="scenes">Scenes for the build.</param>
      // <param name="quit">Quit Unity after the build is completed (default: false, optional)</param>
      private static bool buildPlayer(BuildTarget target, string name, string path, params string[] scenes) //, bool quit = false)
      {
         bool success = false;
         string playerPath = path;

         if (Helper.hasActiveScenes || scenes?.Length > 0)
         {
            CurrentBuildTarget = target;

            if (!string.IsNullOrEmpty(Config.EXECUTE_METHOD_PRE_BUILD))
               Helper.InvokeMethod(Config.EXECUTE_METHOD_PRE_BUILD.Substring(0, Config.EXECUTE_METHOD_PRE_BUILD.CTLastIndexOf(".")), Config.EXECUTE_METHOD_PRE_BUILD.Substring(Config.EXECUTE_METHOD_PRE_BUILD.CTLastIndexOf(".") + 1));

            string fileExtension = string.Empty;
            string modifier;

            // configure path variables based on the platform we're targeting
            switch (target)
            {
               case BuildTarget.StandaloneWindows:
                  modifier = "win32";
                  fileExtension = ".exe";
                  break;
               case BuildTarget.StandaloneWindows64:
                  modifier = "win64";
                  fileExtension = ".exe";
                  break;
               case BuildTarget.StandaloneOSX:
                  modifier = "mac";
                  fileExtension = ".app";
                  break;
               case BuildTarget.StandaloneLinux64:
                  modifier = "linux";
                  fileExtension = ".x86_64";
                  break;
               case BuildTarget.Android:
                  modifier = "android";
                  fileExtension = ".apk";
                  break;
               case BuildTarget.iOS:
                  modifier = "ios";
                  break;
               case BuildTarget.WSAPlayer:
                  modifier = "wsa";
                  break;
               case BuildTarget.WebGL:
                  modifier = "webgl";
                  break;
               case BuildTarget.tvOS:
                  modifier = "tvOS";
                  break;
               case BuildTarget.PS4:
                  modifier = "ps4";
                  break;
               case BuildTarget.XboxOne:
                  modifier = "xboxone";
                  break;
               case BuildTarget.Switch:
                  modifier = "switch";
                  break;
               default:
                  Debug.LogError($"Can't build, target not supported: {target}");
                  return false;
            }

            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
            EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);

            string version = Application.version; //CHANGE: set your desired version

            //TODO add replace spaces for build path?
            string filename = $"{(Config.ADD_NAME_TO_PATH ? $"{name}_" : string.Empty)}{(Config.ADD_VERSION_TO_PATH ? $"{version}_" : string.Empty)}{modifier}{(Config.ADD_DATE_TO_PATH ? $"_{System.DateTime.Now.ToString(Config.DATE_FORMAT)}" : string.Empty)}";
            string buildPath = FileHelper.ValidatePath($"{path}{filename}");
            playerPath = buildPath + name + fileExtension;

            if (Config.DEBUG)
               Debug.Log($"+++ BuildPlayer: '{target}' at '{buildPath}' +++");

            if (deleteBuild)
            {
               try
               {
                  if (System.IO.Directory.Exists(buildPath))
                     System.IO.Directory.Delete(buildPath, true);
               }
               catch (System.Exception ex)
               {
                  Debug.LogError($"Could not delete build path '{buildPath}: {ex}");
               }
            }

            OnBuildStart?.Invoke(target, path, name);

            UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(scenes?.Length > 0 ? scenes : Helper.ScenePaths, playerPath, target, (Config.BO_SHOW_BUILT_PLAYER ? BuildOptions.ShowBuiltPlayer : BuildOptions.None) | (Config.BO_DEVELOPMENT ? BuildOptions.Development : BuildOptions.None) | (Config.BO_PROFILER ? BuildOptions.ConnectWithProfiler : BuildOptions.None) | (Config.BO_SCRIPTDEBUG ? BuildOptions.AllowDebugging : BuildOptions.None));

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
               if (Config.BO_COMPRESS)
                  CompressDirectory(buildPath, $"{path}/{filename}.zip");

               success = true;
            }

            if (!string.IsNullOrEmpty(Config.EXECUTE_METHOD_POST_BUILD))
               Helper.InvokeMethod(Config.EXECUTE_METHOD_POST_BUILD.Substring(0, Config.EXECUTE_METHOD_POST_BUILD.CTLastIndexOf(".")), Config.EXECUTE_METHOD_POST_BUILD.Substring(Config.EXECUTE_METHOD_POST_BUILD.CTLastIndexOf(".") + 1));

            CurrentBuildTarget = BuildTarget.NoTarget;

            //if (quit)
            //    EditorApplication.Exit(0);

            OnBuildComplete?.Invoke(target, playerPath, success);
         }
         else
         {
            Debug.LogWarning("No active scenes found - build not possible!");
         }

         return success;
      }

      // compress the folder into a ZIP file, uses https://github.com/r2d2rigo/dotnetzip-for-unity
      public static void CompressDirectory(string directory, string zipFileOutputPath) //TODO move to Common?
      {
         //Debug.Log($"Attempting to compress {directory} into {zipFileOutputPath}");

         EditorUtility.DisplayProgressBar("Compressing build... please wait", zipFileOutputPath, Random.Range(0.35f, 0.7f)); // display fake percentage

         using (ZipFile zip = new ZipFile())
         {
            zip.ParallelDeflateThreshold = -1; // DotNetZip bugfix that corrupts DLLs / binaries http://stackoverflow.com/questions/15337186/dotnetzip-badreadexception-on-extract
            zip.AddDirectory(directory);
            zip.Save(zipFileOutputPath);
         }

         EditorUtility.ClearProgressBar();
      }

      #endregion
   }
}
#endif
// © 2018-2023 crosstales LLC (https://www.crosstales.com)