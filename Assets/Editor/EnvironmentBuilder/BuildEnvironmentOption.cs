using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EnvironmentBuilder
{
    [Serializable]
    public class BuildEnvironmentOption
    {
        [Serializable]
        public class PlatformOption
        {
            public string Platform;
            public string ProductName;
            public string ApplicationIdentifier;
        }

        [SerializeField] private List<PlatformOption> PlatformOptions;
        [SerializeField] private List<string> DefineSymbols;
        [SerializeField] private List<string> BuildOptions;

        public string JoinDefineSymbols => string.Join(";", DefineSymbols);

        public PlatformOption FindPlatform(string platform) =>
            PlatformOptions.FirstOrDefault(p => p.Platform == platform);

        public BuildOptions GetBuildOptions()
        {
            BuildOptions ret = UnityEditor.BuildOptions.None;
            foreach (BuildOptions bo in Enum.GetValues(typeof(BuildOptions)))
            {
                var buildOptionsName = Enum.GetName(typeof(BuildOptions), bo);
                if (BuildOptions.Any(b => b == buildOptionsName))
                {
                    ret |= bo;
                }
            }
            return ret;
        }

#region CreateSample
        public static BuildEnvironmentOption CreateSample()
        {
            return new BuildEnvironmentOption()
            {
                PlatformOptions = new List<PlatformOption>()
                    {
                        new PlatformOption()
                        {
                             Platform = BuildTarget.iOS.ToString(),
                             ProductName = "AppName",
                             ApplicationIdentifier = "com.appname",
                        },
                        new PlatformOption()
                        {
                             Platform = BuildTarget.Android.ToString(),
                             ProductName = "AppName",
                             ApplicationIdentifier = "com.appname",
                        }
                    },
                DefineSymbols = new List<string>()
                    {
                        "SERVER_DEVELOP",
                        "DEBUG_LOG",
                        "TUTORIAL_SKIP",
                    },
                BuildOptions = new List<string>()
                    {
                        "Development",      // UnityEditor.BuildOptions.Development
                    }

            };
        }
#endregion

    }

}
