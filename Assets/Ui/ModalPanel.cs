using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

//  This script will be updated in Part 2 of this 2 part series.
public class ModalPanel : MonoBehaviour
{
    class QuestionContents
    {
        internal string question;
        internal UnityAction yesEvent;
        internal UnityAction noEvent;
        internal UnityAction cancelEvent;
        internal string yesText;
        internal string noText;
        internal string cancelText;
        internal Sprite sprite;
    }

    Queue<QuestionContents> pendingQuestions = new Queue<QuestionContents>();

    public Text question;
    public Image iconImage;
    public Button yesButton;
    public Button noButton;
    public Button cancelButton;
    public GameObject modalPanelObject;

    private static ModalPanel modalPanel;

    public static ModalPanel Instance
    {
        get
        {
            if (!modalPanel)
            {
                modalPanel = FindObjectOfType(typeof(ModalPanel)) as ModalPanel;
                if (!modalPanel)
                    Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
            }

            return modalPanel;
        }
    }

    void DequeueQuestion()
    {
        if (pendingQuestions.Count == 0)
            return;
        var question = pendingQuestions.Dequeue();

        modalPanelObject.SetActive(true);

        iconImage.sprite = question.sprite;

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(question.yesEvent);
        yesButton.onClick.AddListener(ClosePanel);
        yesButton.GetComponentInChildren<Text>().text = question.yesText;

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(question.noEvent);
        noButton.onClick.AddListener(ClosePanel);
        noButton.GetComponentInChildren<Text>().text = question.noText;

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(question.cancelEvent);
        cancelButton.onClick.AddListener(ClosePanel);
        cancelButton.GetComponentInChildren<Text>().text = question.cancelText;

        this.question.text = question.question;

        iconImage.gameObject.SetActive(question.sprite != null);
        yesButton.gameObject.SetActive(question.yesEvent != null);
        noButton.gameObject.SetActive(question.noEvent != null);
        cancelButton.gameObject.SetActive(question.cancelEvent != null);
    }


    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice(string question, UnityAction yesEvent, UnityAction noEvent, UnityAction cancelEvent, string yesText = "Yes", string noText = "No", string cancelText = "Cancel")
    {
        var contents = new QuestionContents();
        contents.question = question;
        contents.yesEvent = yesEvent;
        contents.noEvent = noEvent;
        contents.cancelEvent = cancelEvent;
        contents.yesText = yesText;
        contents.noText = noText;
        contents.cancelText = cancelText;
        contents.sprite = null;
        pendingQuestions.Enqueue(contents);
        if (!modalPanelObject.activeSelf)
            DequeueQuestion();
    }

    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice(string question, UnityAction yesEvent, UnityAction noEvent, string yesText = "Yes", string noText = "No")
    {
        Choice(question, yesEvent, noEvent, null, yesText, noText, null);
    }

    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice(string question, UnityAction yesEvent, string yesText = "Yes")
    {
        Choice(question, yesEvent, null, null, yesText, null, null);
    }

    void ClosePanel()
    {
        modalPanelObject.SetActive(false);
        DequeueQuestion();
    }
}