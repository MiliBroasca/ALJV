using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GAJsonLogger
{
    public static void SaveRunLog(GALog log)
    {
        string folderPath = Path.Combine(Application.dataPath, "GA_Logs");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        int nextIndex = GetNextIndex(folderPath);

        string fileName = $"ga_run_{nextIndex:D3}.json";
        string fullPath = Path.Combine(folderPath, fileName);

        string json = JsonUtility.ToJson(log, true);
        File.WriteAllText(fullPath, json);

        Debug.Log("Saved GA log to: " + fullPath);
    }

    private static int GetNextIndex(string folderPath)
    {
        string[] files = Directory.GetFiles(folderPath, "ga_run_*.json");
        int maxIndex = 0;

        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file); // ga_run_001
            string[] parts = fileName.Split('_');

            if (parts.Length >= 3 && int.TryParse(parts[2], out int index))
            {
                if (index > maxIndex)
                    maxIndex = index;
            }
        }

        return maxIndex + 1;
    }
}
