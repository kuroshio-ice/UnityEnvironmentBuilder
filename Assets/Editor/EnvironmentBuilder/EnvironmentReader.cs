using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace EnvironmentBuilder
{
    public class JsonController
    {
        const string JSON_FILE_PATH = "Assets/Editor/EnvironmentBuilder/Envs/";

        public BuildEnvironmentOption LoadJson(string environment)
        {
            if (!Directory.Exists(JSON_FILE_PATH))
                Directory.CreateDirectory(JSON_FILE_PATH);

            string fileName = $"{environment}.json";
            string path = Path.Combine(JSON_FILE_PATH, fileName);
            return Deserialize(path);
        }

        public void SaveJson(string environment, BuildEnvironmentOption buildEnvironment)
        {
            if (!Directory.Exists(JSON_FILE_PATH))
                Directory.CreateDirectory(JSON_FILE_PATH);

            string fileName = $"{environment}.json";
            string path = Path.Combine(JSON_FILE_PATH, fileName);
            Serialize(path, buildEnvironment);
        }

        private BuildEnvironmentOption Deserialize(string jsonPath)
        {
            using (var sr = new StreamReader(jsonPath))
            {
                string json = sr.ReadToEnd();
                return JsonUtility.FromJson<BuildEnvironmentOption>(json);
            }
        }

        private void Serialize(string jsonPath, BuildEnvironmentOption buildEnvironment)
        {
            var json = JsonUtility.ToJson(buildEnvironment, true);
            using (var sw = new StreamWriter(jsonPath))
            {
                sw.WriteLine(json);
            }
        }

    }
}
