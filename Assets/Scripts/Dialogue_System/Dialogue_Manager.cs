using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Manager : MonoBehaviour
{
    public List<String> dialToString;
    public int currentReadString;
    public bool skipped;
    public TextMeshProUGUI leftSpeech;
    public TextMeshProUGUI rightSpeech;
    public GameObject QuestionSpawner;

    private List<GameObject> questions;
    private bool inQA;
    private int amountOfQuestions;
    
    void Start()
    {
        string textFile = Resources.Load("Dialogues/Dialogue_test").ToString();
        dialToString = new List<string>(textFile.Split('\n'));
        questions = new List<GameObject>();

        //ReadNextLine();
    }

    void Update()
    {
        if (skipped == true)
        {
            skipped = false;
            ReadNextLine();
        }
    }

    public void Askedquestion(int questID)
    {
        Debug.Log(questID);
        if(questID == amountOfQuestions)
        {
            currentReadString = currentReadString + amountOfQuestions * 2;
            foreach(GameObject question in questions)
            {
                Destroy(question);
            }
            questions = new List<GameObject>();
            ReadNextLine();
            return;
        }

        string rightSting = dialToString[currentReadString + questID * 2];
        string leftSting = dialToString[currentReadString - 1 + questID * 2];
        leftSpeech.text = leftSting.Substring(2);
        rightSpeech.text = rightSting.Substring(2);
    }

    public void SpawnQuestions()
    {
        for(int i = 1; i <= amountOfQuestions; i++)
        {
            GameObject spawned = Instantiate(QuestionSpawner, 
                new Vector3(QuestionSpawner.transform.position.x, 
                QuestionSpawner.transform.position.y - (1 * i), QuestionSpawner.transform.position.z),                 
                Quaternion.identity, QuestionSpawner.transform.parent);
           
            spawned.name = "Quest" + i;
            spawned.SetActive(true);

            string textin = dialToString[currentReadString + i * 2 - 1].Substring(2);
            spawned.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = textin;
            int temp = i;
            spawned.GetComponent<Button>().onClick.AddListener(() => Askedquestion(temp));

            questions.Add(spawned.gameObject);
        }
    }

    public void ReadNextLine()
    {        
        string curSting = dialToString[currentReadString];
        char special = curSting[1];
        char caller = curSting[0];

        if (special == ';')
        {
            switch (caller)
            {
                case 'S': //Start the speaking sequence
                    currentReadString++;
                    ReadNextLine();
                    return;
                case 'L':
                    leftSpeech.text = "";
                    rightSpeech.text = "";
                    leftSpeech.text = curSting.Substring(2);
                    currentReadString++;
                    break;
                case 'R':
                    leftSpeech.text = "";
                    rightSpeech.text = "";
                    rightSpeech.text = curSting.Substring(2);
                    currentReadString++;                 
                    break;
                case 'B':
                    char temp = curSting[2];
                    amountOfQuestions = temp - '0';
                    inQA = true;
                    SpawnQuestions();
                    break;
                case 'E':
                    //exit
                    break;
            }
        }
    }
}
