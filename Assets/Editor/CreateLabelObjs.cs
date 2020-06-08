/*
 * Author : Jeewon Park 
 * Description : Generate static variables of Addressable Labels to manage it EASY.
 * How to use : 
 *      1. just put the script in /Assets/Editor/ 
 *      2. make Labels in Addressable
 *      3. assign labels to your addressable assets
 *      4. Hit Deckard Utils / Get All Labels in menu bar
 *      5. then you can see the AutoGenLabels.cs in Scripts/AllLabels folder
 *      6. it is private static variables, however, you can access in anywhere with AutoGenLabels.GetLabels() to get all labels (variable type = List<String>).
 *      7. It should be handy, if you make a downloader.
 * Worked in : 
 *      1. Unity 2019.3.0f
 *      2. Addressable 1.5
 */


using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class CreateObjs : EditorWindow
{
    private const string SCRIPT_PATH = "Scripts";
    private const string OBJ_SCRIPT_NAME = "AllLabels";
    private const string AUTO_GENERATE_FILE_NAME = "AutoGenLabels.cs";
    private const string AutoGenFormat =
@"//-----------------------------------------------------------------------
// This file is AUTO-GENERATED.
// Changes for this script by hand might be lost when auto-generation is run.
//-----------------------------------------------------------------------

using System.Collections.Generic;

class AutoGenLabels
{
";
    private static string AutoGenTemplate { 
        get 
        { 
            return string.Format(AutoGenFormat, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")); 
        } 
    }



    [MenuItem("Deckard Utils/Get All Labels")] 
    private static void Apply()
    {
        var targetPath = Path.Combine(Application.dataPath, SCRIPT_PATH, OBJ_SCRIPT_NAME).ToString();

        if(!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
            //AssetDatabase.Refresh();
        }
        var allLabels = GetAllLabels();
        Debug.Log(allLabels.Count);
        var targetFile = Path.Combine(targetPath, AUTO_GENERATE_FILE_NAME);
        WriteLabelFile(targetFile, builder => GenerateContext(builder, allLabels));

    }

    static List<string> GetAllLabels()
    {
        List<string> allLabels = new List<string>();
        var grps = AddressableAssetSettingsDefaultObject.Settings.groups;
        foreach (var grp in grps)
        {
            foreach (var entries in grp.entries)
            {
                foreach (var label in entries.labels)
                {
                    allLabels.Add(label.ToString());
                }
            }
        }
        var distintItem = allLabels.Distinct().ToList<string>();
        return distintItem;

    }

    static bool DeleteAutoGenFileExist(string path)
    {
        if(File.Exists(path))
        {
            File.Delete(path);
            //AssetDatabase.Refresh();
            return true;
        }
        return false;
    }

    private static void WriteLabelFile(string path, System.Action<StringBuilder> action)
    {
        DeleteAutoGenFileExist(path);
        try
        {
            using (FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    StringBuilder builder = new StringBuilder();
                    action(builder);
                    Debug.Log(builder);
                    writer.Write(builder.ToString());
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Critical Error occured : {e}");
            DeleteAutoGenFileExist(path);
            return;
        }
        AssetDatabase.Refresh();
    }

    static void GenerateContext(StringBuilder builder, List<string> labels)
    {
        builder.Append($"// Generated date: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
        builder.AppendLine();
        builder.Append(AutoGenFormat);
        builder.Append($"    private static int TOTAL_LABEL = {labels.Count}; ");
        builder.AppendLine();

        foreach(var i in labels)
        {
            builder.Append($@"    private static string {i.ToString().ToUpper()} = " + '"' + i + "\";");
            builder.AppendLine();
        }

        builder.AppendLine();
        builder.Append($@"    private static List<string> labels = new List<string>()");
        builder.AppendLine();
        builder.AppendLine("    {");

        for(int i = 0; i < labels.Count; i++)
        {
            string temp = i == labels.Count - 1 ? $"        {labels[i].ToUpper()}" : $"        {labels[i].ToUpper()},";
            builder.AppendLine($"{temp}");
        }
        builder.AppendLine("    };");
        builder.AppendLine("    public static List<string> GetLabels() { return labels; } ");
        builder.AppendLine("    public static int GetLabelsCount() { return labels.Count; } ");


        builder.AppendLine("}");
    }
}
