using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mercier.Scripts.ScriptableObjects;

public class EditorTool : EditorWindow
{
    string[] databaseGUID;
    public Database database;
    public Database[] databaseArray;
    public SerializedObject serializedObj;

    [MenuItem("ITP Tools/Editor Tool")]
    public static void ShowWindow()
    {
        GetWindow(typeof(EditorTool));
    }

    private void OnEnable()
    {
        PopulateDatabaseArray();
    }

    private void PopulateDatabaseArray()
    {
        /*
        databaseGUID = AssetDatabase.FindAssets("EnemyDatabase");
        
        if (databaseGUID.Length > 0)
        {
            if (databaseGUID[0] == null)
            {
                return;
            }

            database = (Database)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(databaseGUID[0]));

            serializedObj = new SerializedObject(database);
        }*/


        databaseGUID = AssetDatabase.FindAssets("t:Database");

        if (databaseGUID.Length > 0)
        {
            databaseArray = new Database[databaseGUID.Length];

            for (int i = 0; i < databaseGUID.Length; i++)
            {
                database = (Database)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(databaseGUID[i]));

                databaseArray[i] = database;
            }
        }
    }

    private void OnGUI()
    {
        /*
        serializedObj.Update();

        EditorGUILayout.PropertyField(serializedObj.FindProperty("databaseList"), new GUIContent("Database List"), true);
        
        serializedObj.ApplyModifiedProperties();
        */


        for (int i = 0; i < databaseArray.Length; i++)
        {
            if (databaseArray[i] != null)
            {
                databaseArray[i] = (Database)EditorGUILayout.ObjectField(databaseArray[i], typeof(Database), true);
                //EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15f);
                
                for (int b = 0; b < databaseArray[i].databaseList.Count; b++)
                {
                    databaseArray[i].databaseList[b].name = EditorGUILayout.TextField("Name", databaseArray[i].databaseList[b].name);
                    databaseArray[i].databaseList[b].iD = EditorGUILayout.IntField("ID", databaseArray[i].databaseList[b].iD);
                    databaseArray[i].databaseList[b].prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", databaseArray[i].databaseList[b].prefab, typeof(GameObject), false);

                    GUILayout.Space(5f);
                }

                GUILayout.Space(15f);
                //EditorGUILayout.EndHorizontal();
            }
        }
    }
}
