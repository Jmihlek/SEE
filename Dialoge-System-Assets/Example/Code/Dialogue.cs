using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueChoice
{
    //Change era of text in editor
    [TextArea(5,7)]
    //Text and ids for which choices to load in plus 1 for each choice
    //the 0 in the list is 1 and etc
    public string text;
    public int[] ChoiceIDs;
    //Voice if you want to add it in
    public AudioClip Voice;
    public enum Commands // your custom Commands
    {
        NONE,
        End,
        Continue
    };
    public Commands commands;
}
[System.Serializable]
public class ChoiceChoice
{
    [TextArea(5, 7)]
    public string text;
    //Which dialogue to show and add 1 from number in the list of dialogue
    public int DiloguesID;
    public enum Commands // your custom Choice Commands
    {
        NONE,
        Flirt,
        Persude
    };
    public Commands commands;
    //req stat and other dialogue to play if player doesnt have that Stat
    public int reqStat = 0;
    public int alternativeDialogue = 0;
}
[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/New", order = 1)]
public class Dialogue : ScriptableObject
{
    //Each dialogue and choice you have to add another to the list
    //with the id of choice or dialogue that is in the list.
    //public List<string> DialogueT = new List<string>();
    //public List<string> choice = new List<string>();
    public List<DialogueChoice> DialogueTs = new List<DialogueChoice>();
    public List<ChoiceChoice> choices = new List<ChoiceChoice>();
}
