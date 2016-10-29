using UnityEngine;
using System.IO;

public static class IO 
{
    // No need to include extension.
    public static string LoadTextAsset(string path)
    {
        string text = string.Empty;
        var asset = Resources.Load<TextAsset>(path);
        if (asset != null)
        {
            text = asset.text;
        }

        return text;
    }

    // No need to include extension.
    public static T LoadFromJson<T>(string path)
    {
        return JsonUtility.FromJson<T>(LoadTextAsset(path));
    }

#if UNITY_EDITOR
    // Saves and loads files from the root data path.
    public static string DEV_LoadTextAsset(string pathWithExtension)
    {
        var dataPath = Path.Combine(Application.dataPath, Path.Combine("Resources", pathWithExtension));
        return File.ReadAllText(dataPath);
    }

    public static void DEV_SaveTextAsset(string pathWithExtension, string contents)
    {
        var dataPath = Path.Combine(Application.dataPath, Path.Combine("Resources", pathWithExtension));
        File.WriteAllText(dataPath, contents);

        Debug.Log(string.Format("Saved contents to \"{0}\"", dataPath));
    }

    // No need to include extension.
    public static T DEV_LoadFromJsonDev<T>(string path)
    {
        return JsonUtility.FromJson<T>(DEV_LoadTextAsset(path + ".json"));
    }

    // No need to include extension.
    public static void DEV_SaveToJsonDev(string path, object contents)
    {
        DEV_SaveTextAsset(path + ".json", JsonUtility.ToJson(contents));
    }
#endif

}