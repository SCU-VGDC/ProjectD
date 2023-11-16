using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Manager : Interactable
{
    public static Dialogue_Manager DialogueInAction;
    private int chillTime;

    public bool startWithScene;

    public bool isRepeatable;

    public string dialoguePlace;

    public Dictionary<int, DialogueSlide> currentDialogueDB = new Dictionary<int, DialogueSlide>();

    private List<DialogueSlide> dsfromimport;

    public DialogueSlide currentSlide;

    public bool HaveSpawnedOptions;

    [Header("SetUp")]
    public Image Photo;
    public TMP_Text NameBox;
    public TMP_Text TextBox;
    public List<Button> Options;
    public AudioSource audioSource;

    private void FixedUpdate()
    {
        chillTime++;
        if(chillTime > 100)
        {
            chillTime = 100;
        }
    }

    void Start()
    {
        if(startWithScene)
        {
            Activation();
        }
    }

    public override void Activation()
    {
        GetData();
        SetUpDictionary();
        currentSlide = currentDialogueDB[1];
        Photo.transform.parent.gameObject.SetActive(true);
        DialogueInAction = this;
        chillTime = 90;
        PerformAction(0);
    }

    public override void Deactivation()
    {
        Photo.transform.parent.gameObject.SetActive(false);
        DialogueInAction = null;
        if(!isRepeatable && !startWithScene)
        {
            Destroy(gameObject);
        }
    }


    //-------------------Dialogue SetUp-----------------

    void GetData()
    {
        string textFile = Resources.Load(dialoguePlace).ToString();

        List<string> strings = new List<string>(textFile.Split('\n'));

        dsfromimport = new List<DialogueSlide>();

        for (int i = 0; i < strings.Count; i++)
        {
            int test = performSyntaxCheck(strings[i]);

            if(test >= 1)
            {
                dsfromimport.Add(CreateDS(strings[i]));
            }
            else if(test < 0)
            {
                Debug.LogError("Error occured in line " + i + " of type - " + ThrowErrorOnDialogue(test));
            }
        }
    }

    DialogueSlide CreateDS(string s)
    {
        DialogueSlide ds = new DialogueSlide();

        ds.Functions = new List<DialogueFunction>();

        for (int i = 1; i < s.Length; i++)
        {
            if (s[i] == '=')  //getting id
            {
                string idText = "";

                for(int j = 1; j < i; j++)
                {
                    idText += s[j];
                }

                ds.id = int.Parse(idText);
            }

            if (s[i] == '(') //function start
            {
                int closingPlace = i;
                string insideText = "";

                for (int j = i + 1; j < s.Length; j++) //gets insides of a function
                {                  
                    if (s[j] == ')')
                    {
                        closingPlace = j;
                        break;
                    }

                    insideText += s[j];
                }

                if (s[i - 1] == 'S')
                {
                    string[] innerShowText = new string[4];
                    innerShowText[3] = "";
                    int stringCounter = 0;

                    for (int k = 0; k < insideText.Length; k++) //gets insides of show
                    {
                        if (k == (insideText.Length - 1)) //add last letter explicitly
                        {
                            innerShowText[stringCounter] = innerShowText[stringCounter] + insideText[k];
                        }

                        if (insideText[k] == '|' || k == (insideText.Length - 1))
                        {
                            stringCounter++;
                            continue;
                        }

                        innerShowText[stringCounter] = innerShowText[stringCounter] + insideText[k];
                    }

                    ds.Functions.Add(new ShowText(innerShowText[0], innerShowText[1], innerShowText[2], innerShowText[3]));
                }
                
                if (s[i - 1] == 'M')
                {
                    ds.Functions.Add(new MoveSlide(int.Parse(insideText)));
                }
                
                if (s[i - 1] == 'O')
                {
                    List<int> fo = new List<int>();

                    string OinsideText = "";

                    for (int k = 0; k < insideText.Length; k++) //gets insides of Options
                    { 
                        if(k == (insideText.Length - 1)) //add last letter explicitly
                        {
                            OinsideText = OinsideText + insideText[k];
                        }

                        if (insideText[k] == '|' || k  == (insideText.Length - 1))
                        {
                            //Debug.Log(OinsideText + " " + k + " " + i); 
                            int temp_option = int.Parse(OinsideText);
                            fo.Add(temp_option);
                            OinsideText = "";
                            continue;
                        }

                        OinsideText = OinsideText + insideText[k];
                    }

                    ds.Functions.Add(new ShowOptions(fo));
                }
                
                if (s[i - 1] == 'C')
                {
                    ds.Functions.Add(new CodeSlide(int.Parse(insideText)));
                }

                i = closingPlace;
            }
        }

        return ds;
    }

    int performSyntaxCheck(string s)
    {
        if (s[0] != '+') //check that correct string
        {
            return 0; //not a dialogue
        }

        int openBrackets = 0;
        int closedBrackets = 0;

        for (int i = 1; i < s.Length; i++)
        {
            if (s[i] == '=') //check that int
            {
                //TODO
            }

            if (s[i] == '(') //function start
            {
                openBrackets++;

                if (s[i-1] == 'S')
                {
                    
                }
                else if (s[i - 1] == 'M')
                {

                }
                else if (s[i - 1] == 'O')
                {

                }
                else if (s[i - 1] == 'C')
                {

                }
                else
                {
                    return -1; //unknown function
                }
            }

            if(s[i] == ')')
            {
                closedBrackets++;
            }

        }

        if(openBrackets != closedBrackets)
        {
            return -2; //syntax error
        }


        return 1; //test passed
    }

    string ThrowErrorOnDialogue(int test)
    {
        switch (test)
        {
            case 1:
                return "no Errors";
            case 0:
                return "Not a Dialogue";
            case -1:
                return "Unknown function was used";
            case -2:
                return "Syntax error with function declarations";
            case -3:
                return "Syntax error inside S() function";
        }

        return "Unknown Error";
    }

    void SetUpDictionary()
    {
        currentDialogueDB =  new Dictionary<int, DialogueSlide>();
        foreach(DialogueSlide ds in dsfromimport)
        {
            currentDialogueDB.Add(ds.id, ds);
        }
    }

    //-------------------Dialogue Actions-----------------
    public void PerformAction(int actionID)
    {
        if(chillTime < 20)
        {
            return;
        }

        chillTime = 0;

        DialogueSlide ds;

        if(HaveSpawnedOptions)
        {
            HaveSpawnedOptions = false;
            foreach (Button bop in Options)
            {
                bop.gameObject.SetActive(false);
            }
        }

        if(actionID <= 0)
        {
            ds = currentSlide;
        }
        else
        {
            ds = currentDialogueDB[actionID];
        }

        foreach (DialogueFunction func in ds.Functions)
        {
            func.Function(this);
        }    
    }

    public void codeDecipher(int codeReceived)
    {
        switch(codeReceived)
        {
            case 0: //code 0 for exiting dialogue
                Deactivation();
                break;
        }
    }
}

//1 Show(Privet eto zenya) Move(2)
//2 Show(Privet a eto kirill) Option(3, 4)
//3 Show(Zhto Delayesh) Move(5)
//4 Show(Udi s bahmuta) Move(6)
//5 Show(V Bahnute ebashimsya) Exit(Some Code)
//6 Show(Ne, Ne mogu) Exit(Some Code)

// Move (1)
// show dialogue ("Privet Kak Dela" karitnka1 textboxplace)
// create options (2345)

[Serializable]
public class DialogueSlide
{
    public int id;
    public List<DialogueFunction> Functions;
}

[Serializable]
public class DialogueFunction
{
    public string SlideType;

    public DialogueFunction(string m_ST)
    {
        SlideType = m_ST;
    }

    public virtual void Function(Dialogue_Manager dm)
    {

    }
}

[Serializable]
public class ShowText : DialogueFunction
{
    public string NameData;
    public string textData;
    public string photoPath;
    public string soundPath;

    public ShowText(string m_name, string m_text, string m_photoPath, string m_soundPath) : base("T")
    {
        NameData = m_name;
        textData = m_text;
        photoPath = m_photoPath;
        soundPath = m_soundPath;
    }

    public override void Function(Dialogue_Manager dm)
    {
        //Debug.Log(textData);
        dm.TextBox.text = textData;
        dm.NameBox.text = NameData;
        dm.Photo.sprite = Resources.Load<Sprite>(photoPath);
        if (soundPath != "")
        {
            dm.audioSource.clip = Resources.Load<AudioClip>(soundPath);
            dm.audioSource.Play();
        }

    }
}

[Serializable]
public class MoveSlide : DialogueFunction
{
    public int NextId;

    public MoveSlide(int m_nextID) : base("M")
    {
        NextId = m_nextID;
    }

    public override void Function(Dialogue_Manager dm)
    {
        dm.currentSlide = dm.currentDialogueDB[NextId];
    }
}

public class ShowOptions : DialogueFunction
{
    public List<int> Options;

    public ShowOptions(List<int> m_options) : base("O")
    {
        Options = m_options;
    }

    public override void Function(Dialogue_Manager dm)
    {
        if(Options.Count > 4)
        {
            Debug.LogError("Too many options cut the lowest ones");
        }

        foreach (Button bop in dm.Options)
        {
            bop.gameObject.SetActive(false);
        }

        for (int i = 0; i < Options.Count; i++)
        {
            dm.Options[i].gameObject.SetActive(true);
            dm.Options[i].onClick.RemoveAllListeners();

            DialogueSlide ds = dm.currentDialogueDB[Options[i]];

            foreach (DialogueFunction df in ds.Functions)
            {
                if(df.SlideType == "T")
                {
                    var st = (ShowText)df;
                    dm.Options[i].transform.GetChild(0).GetComponent<TMP_Text>().text = st.textData;
                    dm.Options[i].onClick.AddListener(delegate { dm.PerformAction( ds.id ); });
                    dm.HaveSpawnedOptions = true;
                    //TODO add functionality
                    break;
                }
            }           
        }
    }
}

public class CodeSlide : DialogueFunction
{
    public int code;

    public CodeSlide(int m_code) : base("C")
    {
        code = m_code;
    }

    public override void Function(Dialogue_Manager dm)
    {
        //code sending
        Debug.Log("A code " + code + " was received");
        dm.codeDecipher(code);
    }
}
