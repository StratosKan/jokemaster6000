using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;   // for [Serializable] property

#if UNITY_EDITOR                                  //opens on editor only
[ExecuteInEditMode, UnityEditor.InitializeOnLoad] 
[CanEditMultipleObjects]                          //probably overkill
#endif
public class MotivationTool : EditorWindow , ISerializable
{
    private string myString = "Enter text here...";  //default strings
    private string dummyString = "ELEOS";

    private string jokeMachineDefaultString = "Hello my name is JOKEMACHIN3 v6000 and im here to entertain you and keep you motivated. " +
        " How may i be of service today? ";

    private string jokeButton = "Tell me a joke";

    private bool groupEnabled;  //For toggle group
    private bool myBool = false;
    
    private double timeSinceStartup; //Time refs for update method
    private double timeOnLoad;
    private double timeToPop = 3600; //1 hour
    private bool firstLoad = true;
    
    //SerializedProperty serializedPropertyMyInt;     // Commented code refers to my attempts to do this with scriptable object. Stays here for future attempts.
    //SerializedObject serializedObject;

    JokeDB jokeDB; //joke database

    private string fileRoute = "";               //full filepath ref Application.dataPath/Resources/save_data_motiv_tool.json
    private string motivationToolProjectFilePath = "/Resources/save_data_motiv_tool.json";

    [MenuItem("Window/Motivation Tool")]
    static void Init() //Called when "Motivation Tool" is pressed. Apply references here. Has to be static.
    {
        MotivationTool window = (MotivationTool)EditorWindow.GetWindow(typeof(MotivationTool)); //ref to the window. EditorWindow.GetWindow can be used in Monobehaviour as well.
        window.Show();     //open it
    }
    void OnDisable()
    {
        jokeDB = null;
    }

    void OnGUI() //Occurs one time on each frame
    {
        
        GUILayout.Label("J0K3M4CH1N3_6000", EditorStyles.boldLabel);        //trying to be funny already kappa
        
        EditorGUILayout.SelectableLabel(jokeMachineDefaultString, EditorStyles.wordWrappedLabel,GUILayout.ExpandHeight(true)); //Using SelectableLabel for copy-pasta.
        
        if (jokeDB == null)   //running once
        {
            jokeDB = new JokeDB();

            fileRoute = Application.dataPath + motivationToolProjectFilePath; //setting up the path
            
            if (File.Exists(fileRoute))                                       //checking path location
            {
                string dataAsJson = File.ReadAllText(fileRoute);              //TODO: <T> DeserializeFromJson(<T>,string filepath); where <T> -> JokeDB
                jokeDB = JsonUtility.FromJson<JokeDB>(dataAsJson);
            }
            else
            {
                Debug.Log("ERROR: FILE NOT FOUND");
                Debug.Log("Please make sure your .json file is named/located properly.");
            }
            // Can't use json with scriptable class. Explanation @ the bottom.
            //myObj = ScriptableObject.CreateInstance<JokeDB>();              //Creating an instance of our : ScriptableObject ...
            //this.serializedObject = new UnityEditor.SerializedObject(myObj); //... and then Serializing it to edit its properties.
            //try
            //{
            //    this.serializedPropertyMyInt = serializedObject.FindProperty("myObjectInt"); // Reference to the property myString.
            //    Debug.Log("Found myInt on " + myObj.name);
            //}
            //catch
            //{
            //    throw new MissingReferenceException("DEBUG: Can't find myInt on " + myObj.name);
            //}
            //myObj = Resources.Load<JokeDB>("save_data_motiv_tool");
        }
        
        if (myBool)
        {
            EditorGUILayout.SelectableLabel(dummyString, EditorStyles.wordWrappedLabel, GUILayout.ExpandHeight(true));
            
            jokeButton = "ONE MORE JOKE PLS, HAHA";  //jokeButton works as nextJokeButton so we just replace text.
        }

        if (GUILayout.Button(jokeButton))               //If pressed...
        {
            myBool = true;                              //...enables a label that represents the presentation area

            float randomNumber = UnityEngine.Random.Range(0, jokeDB.jokelist.Count); //...and then generates a random number

            dummyString ="#" + jokeDB.jokelist[(int)randomNumber].number + " " + jokeDB.jokelist[(int)randomNumber].joke; //...which locates a joke in our list
            
            //TODO: Enable the LAUGH-O-METER rating functionality.
        }
        
        //LAUGH-O-METER example with scriptableObject
        //serializedObject.Update();        
        //EditorGUILayout.IntSlider(serializedPropertyMyInt, 0, 10, new GUIContent("LAUGH-O-METER"));
        //serializedObject.ApplyModifiedProperties();
            
        groupEnabled = EditorGUILayout.BeginToggleGroup("Let me tell you a joke", groupEnabled);     //Begins toggle group

        myString = EditorGUILayout.TextField(myString);

        if (GUILayout.Button("Save Joke"))           //Save Button
        {
            Joke testJoke = new Joke { number = jokeDB.jokelist.Count + 1, joke = myString }; //creates a new joke...
                                               //We use +1 because we create Joke before Adding it.
            jokeDB.jokelist.Add(testJoke);                                                //...and then adds it to the list

            jokeDB.myListCountInt = jokeDB.jokelist.Count;

            string fileRoute = Application.dataPath + motivationToolProjectFilePath;

            if (File.Exists(fileRoute))
            {
                Serialize(fileRoute);                                                    //...finally it saves it.
            }
            else
            {
                Debug.Log("ERROR: FILE NOT FOUND");
                Debug.Log("Please make sure your .json file is named/located properly.");
            }
            myString = "Saved Successfully";
        }
        //TODO: Remove joke from list utility.
            
        EditorGUILayout.EndToggleGroup();                                                           //Ends toggle group
        
    }

    private void Update() //Called at least 3x more often than GUI.
    {
        Application.targetFrameRate = 60; //Probably pointless to be here.

        if (firstLoad)                //always true on start
        {
            timeOnLoad = EditorApplication.timeSinceStartup;
            firstLoad = false;            
        }

        timeSinceStartup = EditorApplication.timeSinceStartup;        

        if (timeSinceStartup - timeOnLoad >= timeToPop)        //timeToPop is the time required to autopop but sadly
        {                                                      //...it has to be on monobehaviour
            Debug.Log("Popped " + (int)timeSinceStartup);

            firstLoad = true;
        }
    }

    public void Deserialize(string filePath)
    {
        throw new System.NotImplementedException();
    }

    public void Serialize(string filePath)            //Can be improved with generics
    {
        string dataAsJson = JsonUtility.ToJson(jokeDB); //TODO: <T> SerializeToJson(<T>,string filepath); where <T> -> JokeDB
        File.WriteAllText(filePath, dataAsJson);
    }
}

//Unity Engine does not support serializing classes objects within scriptable objects,
// since on deserialization it would have no idea what class to serialize it as. 
//
//public class JokeDB : ScriptableObject             //Creates an object without the need to be attached on gameObject.
//{
//    public int myObjectInt = 5;
//    public string myObjectString = "roflCopter";
//    public List<Joke> jokelist;
//}

[Serializable]
public class JokeDB
{
    public int myListCountInt; 

    public List<Joke> jokelist;  //TODO: Make dictionary
}
[Serializable]
public class Joke
{
    public int number;          //using this number instead of dictionary to identify Joke #
    public string joke;
}
