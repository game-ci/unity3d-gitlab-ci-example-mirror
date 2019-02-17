using System;
using System.Linq;
using UnityEditor;

static class BuildCommand
{
	static bool IsMac
	{
		get
		{
			return GetArgument ("customBuildTarget").ToString ().ToLower ().Contains ("standaloneosx");
		}
	}
	static bool IsWindows
	{
		get
		{
			return GetArgument ("customBuildTarget").ToString ().ToLower ().Contains ("standalonewindows");
		}
	}
	static bool IsWebGL
	{
		get
		{
			return GetArgument ("customBuildTarget").ToString ().ToLower ().Contains ("webgl");
		}
	}
	static bool IsAndroid
	{
		get
		{
			return GetArgument ("customBuildTarget").ToString ().ToLower ().Contains ("android");
		}
	}

	static string GetArgument (string name)
	{
		string[] args = Environment.GetCommandLineArgs ();
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i].Contains (name))
			{
				return args[i + 1];
			}
		}
		return null;
	}

	static string[] GetEnabledScenes ()
	{
		return (
			from scene in EditorBuildSettings.scenes where scene.enabled select scene.path
		).ToArray ();
	}

	static BuildTarget GetBuildTarget ()
	{
		string buildTargetName = GetArgument ("customBuildTarget");
		Console.WriteLine (":: Received customBuildTarget " + buildTargetName);

		if (IsAndroid)
		{
#if !UNITY_5_6_OR_NEWER
			// https://issuetracker.unity3d.com/issues/buildoptions-dot-acceptexternalmodificationstoplayer-causes-unityexception-unknown-project-type-0
			// Fixed in Unity 5.6.0
			// side effect to fix android build system:
			EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
#endif
		}

		return ToEnum<BuildTarget> (buildTargetName, BuildTarget.NoTarget);
	}

	static string GetBuildPath ()
	{
		string buildPath = GetArgument ("customBuildPath");
		Console.WriteLine (":: Received customBuildPath " + buildPath);
		if (buildPath == "")
		{
			throw new Exception ("customBuildPath argument is missing");
		}
		return buildPath;
	}

	static string GetBuildName ()
	{
		string buildName = GetArgument ("customBuildName");
		Console.WriteLine (":: Received customBuildName " + buildName);
		if (buildName == "")
		{
			throw new Exception ("customBuildName argument is missing");
		}
		return buildName;
	}

	static string GetFixedBuildPath (BuildTarget buildTarget, string buildPath, string buildName)
	{
		if (IsWindows)
		{
			buildName = buildName + ".exe";
		}
		else if (IsWebGL)
		{
			// webgl produces a folder with index.html inside, there is no executable name for this buildTarget
			buildName = "";
		}
		else if (IsAndroid)
		{
			PlayerSettings.productName = buildName;
			buildName = "";
		}
		return buildPath + buildName;
	}

	static BuildOptions GetBuildOptions ()
	{
		string buildOptions = GetArgument ("customBuildOptions");
		return buildOptions == "AcceptExternalModificationsToPlayer" ? BuildOptions.AcceptExternalModificationsToPlayer : BuildOptions.None;
	}

	// https://stackoverflow.com/questions/1082532/how-to-tryparse-for-enum-value
	static TEnum ToEnum<TEnum> (this string strEnumValue, TEnum defaultValue)
	{
		if (!Enum.IsDefined (typeof (TEnum), strEnumValue))
		{
			return defaultValue;
		}

		return (TEnum) Enum.Parse (typeof (TEnum), strEnumValue);
	}

	static string getEnv (string key, bool secret = false, bool verbose = true)
	{
		var env_var = Environment.GetEnvironmentVariable (key);
		if (verbose)
		{
			if (env_var != null)
			{
				if (secret)
				{
					Console.WriteLine (":: env['" + key + "'] set");
				}
				else
				{
					Console.WriteLine (":: env['" + key + "'] set to '" + env_var + "'");
				}
			}
			else
			{
				Console.WriteLine (":: env['" + key + "'] is null");
			}
		}
		return env_var;
	}

	static void PerformBuild ()
	{
		Console.WriteLine (":: Performing build");
		//PlayerSettings.keystorePass = getEnv ("KEYSTORE_PASS", true);
		//PlayerSettings.keyaliasPass = getEnv ("KEY_ALIAS_PASS", true);
		var buildTarget = GetBuildTarget ();
		var buildPath = GetBuildPath ();
		var buildName = GetBuildName ();
		var fixedBuildPath = GetFixedBuildPath (buildTarget, buildPath, buildName);
		if (IsAndroid)
		{
			EditorPrefs.SetString ("AndroidSdkRoot", getEnv ("ANDROID_HOME"));
			EditorPrefs.SetString ("JdkPath", getEnv ("JAVA_HOME"));
			EditorPrefs.SetString ("AndroidNdkRootR16b", getEnv ("ANDROID_NDK_HOME"));
			PlayerSettings.applicationIdentifier = Environment.GetEnvironmentVariable ("BUNDLE_IDENTIFIER");
		}

		BuildPipeline.BuildPlayer (GetEnabledScenes (), fixedBuildPath, buildTarget, GetBuildOptions ());
		Console.WriteLine (":: Done with build");
	}
}