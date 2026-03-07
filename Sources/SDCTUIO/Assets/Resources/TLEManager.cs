using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class RealDebrisEntry
{
    public string NoradId;
    public string Name;
    public string TleLine1;
    public string TleLine2;
    
    //  DISCOS CSV/JSON in the future
    public float Mass = 100f;
    public DebrisShape Shape = DebrisShape.Cube;
    public float Height = 1f, Length = 1f, Width = 1f;
}

public class TLEManager : MonoBehaviour
{
    public static TLEManager Instance { get; private set; }

    // All real debris
    public Dictionary<string, RealDebrisEntry> AvailableRealDebris { get; private set; } = new();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        LoadDefaultTLEData();
    }

    private void LoadDefaultTLEData()
    {
        TextAsset tleFile = Resources.Load<TextAsset>("iridium_debris");
        if (tleFile == null) 
        {
            Debug.LogWarning("[TLE Manager] No default TLE found in Resources.");
            return;
        }

        string[] lines = tleFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        ParseTLELines(lines, "Default Resources");
    }

    public void LoadTLEDataFromExternalFile(string absolutePath)
    {
        if (string.IsNullOrEmpty(absolutePath) || !File.Exists(absolutePath))
        {
            Debug.LogWarning("[TLE Manager] Invalid file path!");
            return;
        }

        string fileText = File.ReadAllText(absolutePath);
        string[] lines = fileText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        ParseTLELines(lines, "External File");
    }

    private void ParseTLELines(string[] lines, string sourceName)
    {
        AvailableRealDebris.Clear();

        for (int i = 0; i < lines.Length - 1; i++)
        {
            string currentLine = lines[i].Trim();
            string nextLine = lines[i + 1].Trim();
            
            if (currentLine.StartsWith("1 ") && nextLine.StartsWith("2 "))
            {
                string noradId = currentLine.Substring(2, 5).Trim();

                if (!AvailableRealDebris.ContainsKey(noradId))
                {
                    AvailableRealDebris.Add(noradId, new RealDebrisEntry {
                        NoradId = noradId,
                        Name = noradId,
                        TleLine1 = currentLine,
                        TleLine2 = nextLine
                    });
                }
            }
        }
        
        Debug.Log($"[TLE Manager] Success to analyze {AvailableRealDebris.Count} data from {sourceName}!");
    }
}