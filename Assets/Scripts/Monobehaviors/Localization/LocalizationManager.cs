using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class LocalizationManager : MonoBehaviour
{
    private LocalizationLanguageKey currentLanguage;
    private Dictionary<LocalizationTableKey, LocalizationTable> tables = new Dictionary<LocalizationTableKey, LocalizationTable>();

    public LocalizationLanguageKey defaultLanguage;
    public TextAsset[] CSVfiles = new TextAsset[0];
    public LocalizationTableKey[] preloadTables = new LocalizationTableKey[0];

    void Awake()
    {
        if (Application.isPlaying == false)
        {
            ReloadMetadata();
        }

        currentLanguage = defaultLanguage;
        for(int i = 0; i < preloadTables.Length; i++)
        {
            LoadTable(preloadTables[i], currentLanguage);
        }
    }

    public string GetStringAsset(LocalizationAssetKey assetKey)
    {
        foreach (KeyValuePair<LocalizationTableKey, LocalizationTable> entry in tables)
        {
            var result = entry.Value.GetStringAsset(assetKey);
            if(result != null)
            {
                return result.GetValue(currentLanguage);
            }
            
        }
        return null;
    }

    public string GetStringAsset(LocalizationTableKey tableKey, LocalizationAssetKey assetKey)
    {
        return tables[tableKey].GetStringAsset(assetKey).GetValue(currentLanguage);
    }

    public void LoadTable(LocalizationTableKey tableKey, LocalizationLanguageKey languageKey)
    {
        string tableName = tableKey.ToString();
        TextAsset tableTextAsset = new TextAsset(File.ReadAllText(Application.dataPath + "/Localization/" + tableName + ".csv", new System.Text.UTF8Encoding()));
        LocalizationTable newTable = new LocalizationTable(tableName);

        bool result = LocalizationFileParser.ParseAndFillLocalizationStringTable(tableTextAsset.text, newTable, languageKey);
        if(!result)
        {
            throw new UnityException("Table '" + tableName + "', couldn't be loaded");
        }

        tables[tableKey] = newTable;
    }

    public void ChangeLanguage(LocalizationLanguageKey targetLanguage)
    {
        foreach (KeyValuePair<LocalizationTableKey, LocalizationTable> entry in tables)
        {
            TextAsset tableTextAsset = new TextAsset(File.ReadAllText(Application.dataPath + "/Localization/" + entry.Key.ToString() + ".csv"));
            entry.Value.Clear();

            bool result = LocalizationFileParser.ParseAndFillLocalizationStringTable(tableTextAsset.text, entry.Value, targetLanguage);
            if (!result)
            {
                throw new UnityException("Table '" + entry.Key.ToString() + "', couldn't be loaded");
            }
        }
    }

    public void ReloadMetadata()
    {
        if (CSVfiles.Length > 0)
        {
            try
            {
                LocalizationFileParser.ParseAndGenerateMetadata(CSVfiles);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }


    private class LocalizationFileParser
    {
        public static bool ParseAndFillLocalizationStringTable(string source, LocalizationTable target, LocalizationLanguageKey language)
        {
            // TODO try/catch parse errors
            string [,] grid = CSVReader.SplitCsvGrid(source);

            int numOfLanguages = Enum.GetNames(typeof(LocalizationLanguageKey)).Length;

            int languageIndex = (int)language;

            for (int x = 1; x < grid.GetLength(0); x++)
            {
                for (int y = 1; y < grid.GetLength(1); y++)
                {
                    LocalizationAssetKey assetKey = (LocalizationAssetKey) Enum.Parse(typeof(LocalizationAssetKey), grid[x, 0]);

                    LocalizationAsset<string> stringAsset = new LocalizationAsset<string>(assetKey, numOfLanguages);
                    stringAsset.AddValue(grid[x, y], languageIndex);
                    target.AddStringAsset(assetKey, stringAsset);
                }
            }

            return true;
        }

        public static void ParseAndGenerateMetadata(TextAsset[] CSVfiles)
        {
            UTF8Encoding encoder = new UTF8Encoding(true);
            byte[] enumEnd = encoder.GetBytes("}\n");

            // Regenerate languages enum 
            using (FileStream languages = File.Create(Application.dataPath + "/Scripts/Monobehaviors/Localization/LocalizationLanguageKey.cs"))
            {
                byte[] languagesStart = encoder.GetBytes("public enum LocalizationLanguageKey\n{\n");
                languages.Write(languagesStart, 0, languagesStart.Length);

                string[] languagesEntries = CSVReader.GetRow(CSVfiles[0].text, 0);

                if (languagesEntries.Length < 2) // KEYS, ENGLISH, SPANISH, .... so at least 2
                {
                    throw new UnityException("CSV(" + CSVfiles[0].name + "), couldn't be parsed");
                }

                for (int i = 1; i < languagesEntries.Length; i++)
                {
                    string languageEntry = "\t" + languagesEntries[i] + ((i == languagesEntries.Length - 1) ? "\n" : ",\n");
                    languages.Write(encoder.GetBytes(languageEntry), 0, languageEntry.Length);
                }

                languages.Write(enumEnd, 0, enumEnd.Length);
            } // languages

            // Regenerate tables enum
            using (FileStream tables = File.Create(Application.dataPath + "/Scripts/Monobehaviors/Localization/LocalizationTableKey.cs"))
            {
                byte[] tablesStart = encoder.GetBytes("public enum LocalizationTableKey\n{\n");
                tables.Write(tablesStart, 0, tablesStart.Length);

                // Regenerate assets enum
                using (FileStream assets = File.Create(Application.dataPath + "/Scripts/Monobehaviors/Localization/LocalizationAssetKey.cs"))
                {
                    byte[] assetsStart = encoder.GetBytes("public enum LocalizationAssetKey\n{\n");
                    assets.Write(assetsStart, 0, assetsStart.Length);

                    for (int j = 0; j < CSVfiles.Length; j++)
                    {
                        TextAsset currentTextAsset = CSVfiles[j];

                        string tableEntry = "\t" + currentTextAsset.name + ((j == CSVfiles.Length - 1) ? "\n" : ",\n");
                        tables.Write(encoder.GetBytes(tableEntry), 0, tableEntry.Length);

                        string[] assetKeys = CSVReader.GetColumn(currentTextAsset.text, 0);
                        for(int n = 1; n < assetKeys.Length; n++)
                        {
                            string assetEntry = "\t" + assetKeys[n] + (((n == assetKeys.Length - 1) && (j == CSVfiles.Length - 1)) ? "\n" : ",\n");
                            assets.Write(encoder.GetBytes(assetEntry), 0, assetEntry.Length);
                        }
                    }

                    assets.Write(enumEnd, 0, enumEnd.Length);
                } // assets

                tables.Write(enumEnd, 0, enumEnd.Length);
            } // tables

        } // ParseAndGenerateMetadata
    } // LocalizationFileParser
} // LocalizationManager

