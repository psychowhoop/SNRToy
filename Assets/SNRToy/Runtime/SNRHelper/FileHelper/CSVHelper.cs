using System;
using System.Collections.Generic;
using UnityEngine;

public static class CSVHelper
{
    public static Dictionary<string, T> Read<T>(string resourceBasePath) where T : class, new()
    {
        Dictionary<string, T> retValue = new Dictionary<string, T>();
        TextAsset txtData = Resources.Load(resourceBasePath, typeof(TextAsset)) as TextAsset;
        //row data
        string[] rowAry = txtData.text.Split('\n');
        if (rowAry.Length <= 0)
        {
            Debug.LogWarning($"Not find data in csv file {resourceBasePath}");
            return retValue;
        }

        string[][] rowColAry = new string[rowAry.Length][];
        for (int i = 0; i < rowAry.Length; ++i)
        {
            rowColAry[i] = rowAry[i].Split(',');
        }

        int rowNumWithFieldName = rowAry.Length;
        int colNum = rowColAry[0].Length;

        for (int i = 1; i < rowNumWithFieldName; ++i)
        {
            if (string.IsNullOrEmpty(rowColAry[i][0].Trim()))
            {
                continue; // Skip empty rows
            }

            string id = rowColAry[i][0].Trim();
            T data = new T();
            var properties = typeof(T).GetProperties();

            for (int j = 0; j < colNum; ++j)
            {
                if (j >= properties.Length)
                {
                    Debug.LogWarning($"More columns found than properties in class {typeof(T).Name}");
                    break;
                }

                var property = properties[j];
                string value = rowColAry[i][j].Trim();
                // Convert string value to property type and set it
                try
                {
                    object convertedValue = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(data, convertedValue);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to convert value '{value}' to type '{property.PropertyType}'. Error: {ex.Message}");
                }
            }

            // Add data to dictionary with ID as key
            retValue[id] = data;
        }

        return retValue;
    }
}
