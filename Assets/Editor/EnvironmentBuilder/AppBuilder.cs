using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class AppBuilder
{
    const string BUILD_LOG_FILENAME = "unitybuild.log";

    static public void Perform()
    {
        var builder = new AppBuilder();
        builder.Process();
    }

    [MenuItem("EnvironmentBuild/Create Sample Json", priority = 200)]
    static public void CreateSampleJson()
    {
        var sample = EnvironmentBuilder.BuildEnvironmentOption.CreateSample();
        var jc = new EnvironmentBuilder.JsonController();
        jc.SaveJson("develop", sample);
    }

    [MenuItem("EnvironmentBuild/Sample Build", priority = 201)]
    static public void SampleBuild()
    {
        var builder = new AppBuilder();
        builder.IsExist = false;
        builder.Process();
    }

    private string SavePath { get; set; }
    private string EnvironmentName { get; set; }
    private BuildTarget BuildTarget { get; set; }
    private BuildOptions BuildOptions { get; set; }
    private int ExitCode { get; set; }
    public bool IsExist { get; set; } = true;

    private void Process()
    {
        var args = Environment.GetCommandLineArgs();

        if (File.Exists(BUILD_LOG_FILENAME))
            File.Delete(BUILD_LOG_FILENAME);

        using (var logFile = File.CreateText(BUILD_LOG_FILENAME))
        {
            WriteLog(logFile, DateTime.Now);
            WriteLog(logFile, "----- Start Build -----");

            // current platform.
            BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            EnvironmentName = "develop";
            SavePath = "app";

            for (int i = 0; i < args.Length; i++)
            {
                var cmd = args[i];
                switch (cmd)
                {
                    case "-path":
                        SavePath = args[++i];
                        break;
                    case "-env":
                        EnvironmentName = args[++i];
                        break;
                }
            }

            if (BuildTarget == BuildTarget.Android)
                SavePath += ".apk";

            WriteLog(logFile, $"savePath: {SavePath}");
            WriteLog(logFile, $"EnvironmentName: {EnvironmentName}");

            Prepare(logFile);

            Build(logFile);

            Later(logFile);

            WriteLog(logFile, "----- Finsih Build -----");
            WriteLog(logFile, DateTime.Now);
        }

        if (IsExist)
            EditorApplication.Exit(ExitCode);
    }

    private void Prepare(StreamWriter logFile)
    {
        WriteLog(logFile, "----- Prepare -----");

        var jc = new EnvironmentBuilder.JsonController();

        var envOption = jc.LoadJson(EnvironmentName);
        var platformOption = envOption.FindPlatform(BuildTarget.ToString());

        // ProductName.
        PlayerSettings.productName = platformOption.ProductName;
        WriteLog(logFile, $"PlayerSettings.productName: {PlayerSettings.productName}");

        // ApplicationIdentifier.
        PlayerSettings.applicationIdentifier = platformOption.ApplicationIdentifier;
        WriteLog(logFile, $"PlayerSettings.applicationIdentifier: {PlayerSettings.applicationIdentifier}");

        BuildOptions = envOption.GetBuildOptions();
        WriteLog(logFile, $"BuildOptions: {BuildOptions}");
    }

    private void Build(StreamWriter logFile)
    {
        WriteLog(logFile, "----- Build -----");

        var levels = GetBuildLevels();
        var levelStr = string.Join(",", levels.Select(l => l.path).ToList());
        WriteLog(logFile, $"levelStr: {levelStr}");

#if UNITY_2018_1_OR_NEWER
        var report = BuildPipeline.BuildPlayer(levels, SavePath, BuildTarget, BuildOptions);
        // build result.
        WriteLog(logFile, "build result.");
        WriteLog(logFile, report.summary.result);

        foreach (var message in report.steps.SelectMany(s => s.messages))
            WriteLog(logFile, message.content);

        switch (report.summary.result)
        {
            case UnityEditor.Build.Reporting.BuildResult.Succeeded:
                ExitCode = 0;
                break;
            default:
                ExitCode = 1;
                break;
        }
#else
        var log = BuildPipeline.BuildPlayer(levels, SavePath, BuildTarget, BuildOptions);
        WriteLog(logFile, "build result.");
        WriteLog(logFile, log);
#endif
    }

    private void Later(StreamWriter logFile)
    {
        WriteLog(logFile, "----- Later -----");
    }

    private EditorBuildSettingsScene[] GetBuildLevels()
    {
        // reference build settings.
        return EditorBuildSettings.scenes.Where(e => e.enabled)
                                         .ToArray();
    }

    private void WriteLog(StreamWriter sw, object obj)
    {
        Debug.Log(obj);
        sw.WriteLine(sw);
    }

    private BuildTargetGroup GetBuildTargetGroupFromBuildTarget(BuildTarget buildTarget)
    {
        foreach (BuildTargetGroup buildTargetGroup in Enum.GetValues(typeof(BuildTargetGroup)))
        {
            var name = Enum.GetName(typeof(BuildTargetGroup), buildTargetGroup);
            if (name == buildTarget.ToString())
                return buildTargetGroup;
        }
        return BuildTargetGroup.Unknown;
    }
}
