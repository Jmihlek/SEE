using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueMan : MonoBehaviour
{
    //=========== Check the example for better understanding of how this is implemented :D ===========

    //Dialogue Scriptable object
    public Dialogue Dl;
    //Speed of text written
    public float textSpeed;
    //Bools for commands (I dont know if they are needed but i dont wanna
    //think)
    bool canExcute = false;
    bool clicked = true;
    bool CantContinue = false;
    //AudioSource for clicking 
    public AudioSource click;
    public AudioSource VoiceMan;
    //Player stats can be added to it 
    public int playerCharisma = 6;
    public int playerSpeech = 6;
    //Texts
    public TextMeshProUGUI DialogueTxt;
    public TextMeshProUGUI[] ChoiceTxt;
    int dialogueID;
    int choiceID;
    //Which choise , list of choices .
    //public List<int> choiceID = new List<int>();

    //[END] Command stuff
    public bool hasEnded = false;
    float timer;
    public float timeToDisable;

    private void Start()
    {
        //When conersation starts with the first dialogue
        ChooseDialogue(0);
    }
    private void Update()
    {
        //Timer after dialogue ends
        if(hasEnded)
        {
            if(timer >= timeToDisable)
            {
                hasEnded = false;
                DialogueTxt.gameObject.SetActive(false);
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        //Skiping with E or left mouse
        if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            VoiceMan.Stop();
            if (DialogueTxt.text != Dl.DialogueTs[dialogueID].text)
            {
                StopAllCoroutines();
                DialogueTxt.text = Dl.DialogueTs[dialogueID].text;
            }
            else
            {
                clicked = true;
                Commands();
            }
        }
        //For continue dialogue 
        if(VoiceMan.isPlaying == false && CantContinue)
        {
            Commands();
            CantContinue = false;
        }
     }

    //Choosing dialogue
    void ChooseDialogue(int i)
    {
        //Stop voice
        if (Dl.DialogueTs[i].Voice != null)
        {
            VoiceMan.Stop();
        }
        //Debug.Log("Updated");
        //Update dialogue text
        dialogueID = i;
        DialogueTxt.text = "";
        //Update buttons for choices 
        UpdateChoices(i);
        //
        canExcute = false;
        Commands();
        //Writing effect
        StartCoroutine(TypeDialogue(Dl.DialogueTs[i].text));
        //Play voice
        if (Dl.DialogueTs[i].Voice != null)
        {
            VoiceMan.clip = Dl.DialogueTs[i].Voice;
            VoiceMan.Play();
        }
    }
    //Updating choices
    void UpdateChoices(int i)
    {

        //clean out 
        for (int j = 0; j < ChoiceTxt.Length; j++)
        {
            ChoiceTxt[j].text = "";
        }
        //Add in 
        for (int j = 0; j < Dl.DialogueTs[i].ChoiceIDs.Length; j++)
        {
            ChoiceTxt[j].text = Dl.choices[Dl.DialogueTs[i].ChoiceIDs[j] - 1].text;
        }
    }
    //Button
    public void ChooseChoice(int choiceX)
    {
        //Clicking Sound effect
        click.pitch = Random.Range(0.55f, 1.25f);
        click.Play();
        //When clicking a button with a value of 1 , 2 , 3 etc
        if (hasEnded == false && ChoiceTxt[choiceX].text != "")
        {
            int nextNodeID = 0;
            for (int i = 0; i < Dl.choices.Count; i++)
            {
                if (Dl.choices[i].text == ChoiceTxt[choiceX].text)
                {
                    nextNodeID = Dl.choices[i].DiloguesID;
                    
                    choiceID = i;
                    Debug.Log(choiceID);
                    break;
                }
            }
            //If coroutine running stop then play the new one
            StopAllCoroutines();
            ChooseDialogue(nextNodeID - 1);
            //Commands for choice like flirting and persuding etc
            ChoiceCommands();
        }
        
    }

    //================Commands and Effect =================

    //Commands
    void Commands()
    {
        //Add more commands like fight and you can use it how ever you want .
        //End the conversation and a timer delay before dialogue gets disabled .
        if (Dl.DialogueTs[dialogueID].commands == DialogueChoice.Commands.End)
        {
            //Debug.Log("Ended");
            if (canExcute)
            {
                hasEnded = true;
            }
            for (int j = 0; j < ChoiceTxt.Length; j++)
            {
                //Remove choices 
                ChoiceTxt[j].text = "";
                ChoiceTxt[j].transform.parent.gameObject.SetActive(false);
            }
        }
        //Continue dialogue with a new dialogue the first ID is the next dialogue 
        if (Dl.DialogueTs[dialogueID].commands == DialogueChoice.Commands.Continue && canExcute)
        {
            if (DialogueTxt.text == Dl.DialogueTs[dialogueID].text && clicked && !VoiceMan.isPlaying)
            {
                ChooseDialogue(Dl.DialogueTs[dialogueID].ChoiceIDs[0] - 1);
                clicked = true;
                CantContinue = false;
            }
            else
            {
                CantContinue = true;
            }
        }
        
        
    }

    void ChoiceCommands()
    {
        //if(player has enough charisma )
        if (Dl.choices[choiceID].commands == ChoiceChoice.Commands.Flirt)
            CheckStat(playerCharisma);
        //Persude stat
        if (Dl.choices[choiceID].commands == ChoiceChoice.Commands.Persude)
            CheckStat(playerSpeech);
        
    }
    void CheckStat(int Stat)
    {
        //If player has enough Stat then play the main dialogue 
        if (Dl.choices[choiceID].reqStat <= Stat)
        {
            Debug.Log(Dl.choices[choiceID].reqStat);
            StopAllCoroutines();
            ChooseDialogue(Dl.choices[choiceID].DiloguesID - 1);
        }
        //or else
        else
        {
            StopAllCoroutines();
            Debug.Log(Dl.choices[choiceID].reqStat);
            ChooseDialogue(Dl.choices[choiceID].alternativeDialogue - 1);
        }
    }
  
    //Typing dialogue And Commands
    IEnumerator TypeDialogue(string dialogue)
    {
        //Typing effect
       foreach(char c in dialogue)
        {
            DialogueTxt.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
       //End dialogue
        canExcute = true;
        clicked = false;
        Commands();
        //Continue dialogue
        yield return new WaitForSeconds(.35f);
        clicked = true;
        Commands();
    }
}
