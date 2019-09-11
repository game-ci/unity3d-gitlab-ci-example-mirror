using UnityEditor;
using System.Linq;
using System;
using System.IO;

static class BuildCommand
{
    private const string KEYSTORE_NAME  = "KEYSTORE_NAME";
    private const string KEYSTORE_PASS  = "KEYSTORE_PASS";
    private const string KEY_ALIAS_PASS = "KEY_ALIAS_PASS";
    private const string KEY_ALIAS_NAME = "KEY_ALIAS_NAME";
    private const string KEYSTORE       = "keystore.keystore";

    static string GetArgument(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Contains(name))
            {
                return args[i + 1];
            }
        }
        return null;
    }

    static string[] GetEnabledScenes()
    {
        return (
            from scene in EditorBuildSettings.scenes
            where scene.enabled
            where !string.IsNullOrEmpty(scene.path)
            select scene.path
        ).ToArray();
    }

    static BuildTarget GetBuildTarget()
    {
        string buildTargetName = GetArgument("customBuildTarget");
        Console.WriteLine(":: Received customBuildTarget " + buildTargetName);

        if (buildTargetName.ToLower() == "android")
        {
#if !UNITY_5_6_OR_NEWER
			// https://issuetracker.unity3d.com/issues/buildoptions-dot-acceptexternalmodificationstoplayer-causes-unityexception-unknown-project-type-0
			// Fixed in Unity 5.6.0
			// side effect to fix android build system:
			EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
#endif
        }

        return ToEnum<BuildTarget>(buildTargetName, BuildTarget.NoTarget);
    }

    static string GetBuildPath()
    {
        string buildPath = GetArgument("customBuildPath");
        Console.WriteLine(":: Received customBuildPath " + buildPath);
        if (buildPath == "")
        {
            throw new Exception("customBuildPath argument is missing");
        }
        return buildPath;
    }

    static string GetBuildName()
    {
        string buildName = GetArgument("customBuildName");
        Console.WriteLine(":: Received customBuildName " + buildName);
        if (buildName == "")
        {
            throw new Exception("customBuildName argument is missing");
        }
        return buildName;
    }

    static string GetFixedBuildPath(BuildTarget buildTarget, string buildPath, string buildName)
    {
        if (buildTarget.ToString().ToLower().Contains("windows"))
        {
            buildName = buildName + ".exe";
        }
        return buildPath + buildName;
    }

    static BuildOptions GetBuildOptions()
    {
        string buildOptions = GetArgument("customBuildOptions");
        return buildOptions == "AcceptExternalModificationsToPlayer" ? BuildOptions.AcceptExternalModificationsToPlayer : BuildOptions.None;
    }

    // https://stackoverflow.com/questions/1082532/how-to-tryparse-for-enum-value
    static TEnum ToEnum<TEnum>(this string strEnumValue, TEnum defaultValue)
    {
        if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
        {
            return defaultValue;
        }

        return (TEnum)Enum.Parse(typeof(TEnum), strEnumValue);
    }

    static bool TryGetEnv(string key, out string value)
    {
        value = Environment.GetEnvironmentVariable(key);
        return !string.IsNullOrEmpty(value);
    }

    static void PerformBuild()
    {
        Console.WriteLine(":: Performing build");

        //PlayerSettings.keystorePass = getEnv ("KEYSTORE_PASS", true);
        //PlayerSettings.keyaliasPass = getEnv ("KEY_ALIAS_PASS", true);
        //EditorSetup.AndroidSdkRoot = getEnv ("ANDROID_SDK_HOME");
        //EditorSetup.JdkRoot = getEnv ("JAVA_HOME");
        //EditorSetup.AndroidNdkRoot = getEnv ("ANDROID_NDK_HOME");
        var buildTarget = GetBuildTarget();

        if (buildTarget == BuildTarget.Android) {
            HandleAndroidKeystore();
        }

        var buildPath = GetBuildPath();
        var buildName = GetBuildName();
        var fixedBuildPath = GetFixedBuildPath(buildTarget, buildPath, buildName);

        BuildPipeline.BuildPlayer(GetEnabledScenes(), fixedBuildPath, buildTarget, GetBuildOptions());
        Console.WriteLine(":: Done with build");
    }

    private static void HandleAndroidKeystore()
    {
        PlayerSettings.Android.useCustomKeystore = false;

        if (!File.Exists(KEYSTORE)) {
            Console.WriteLine($":: {KEYSTORE} not found, skipping setup, using Unity's default keystore");
            return;    
        }

        string keystorePass;
        string keystoreAliasPass;

        if (TryGetEnv(KEYSTORE_NAME, out string keystoreName)) {
            PlayerSettings.Android.keystoreName = keystoreName;
            Console.WriteLine($":: using ${KEYSTORE_NAME} env var on PlayerSettings");
        } else {
            Console.WriteLine($":: ${KEYSTORE_NAME} env var not set, use Project's PlayerSettings");
        }

        if (TryGetEnv(KEY_ALIAS_NAME, out string keyaliasName)) {
            PlayerSettings.Android.keyaliasName = keyaliasName;
            Console.WriteLine($":: using ${KEY_ALIAS_NAME} env var on PlayerSettings");
        } else {
            Console.WriteLine($":: ${KEY_ALIAS_NAME} env var not set, use Project's PlayerSettings");
        }

        if (!TryGetEnv(KEYSTORE_PASS, out keystorePass)) {
            Console.WriteLine($":: ${KEYSTORE_PASS} env var not set, skipping setup, using Unity's default keystore");
            return;
        }

        if (!TryGetEnv(KEY_ALIAS_PASS, out keystoreAliasPass)) {
            Console.WriteLine($":: ${KEY_ALIAS_PASS} env var not set, skipping setup, using Unity's default keystore");
            return;
        }

        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystorePass = keystorePass;
        PlayerSettings.Android.keyaliasPass = keystoreAliasPass;
    }
}
