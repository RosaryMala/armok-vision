using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

//  This script will be updated in Part 2 of this 2 part series.
public class ModalPanel : MonoBehaviour
{

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

    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice(string question, UnityAction yesEvent, UnityAction noEvent, UnityAction cancelEvent, string yesText = "Yes", string noText = "No", string cancelText = "Cancel")
    {
        modalPanelObject.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(yesEvent);
        yesButton.onClick.AddListener(ClosePanel);
        yesButton.GetComponentInChildren<Text>().text = yesText;

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(noEvent);
        noButton.onClick.AddListener(ClosePanel);
        noButton.GetComponentInChildren<Text>().text = noText;

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(cancelEvent);
        cancelButton.onClick.AddListener(ClosePanel);
        cancelButton.GetComponentInChildren<Text>().text = cancelText;

        this.question.text = question;

        iconImage.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice(string question, UnityAction yesEvent, UnityAction noEvent, string yesText = "Yes", string noText = "No")
    {
        modalPanelObject.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(yesEvent);
        yesButton.onClick.AddListener(ClosePanel);
        yesButton.GetComponentInChildren<Text>().text = yesText;

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(noEvent);
        noButton.onClick.AddListener(ClosePanel);
        noButton.GetComponentInChildren<Text>().text = noText;

        this.question.text = question;

        this.iconImage.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(false);
    }

    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice(string question, UnityAction yesEvent, string yesText = "Yes")
    {
        modalPanelObject.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(yesEvent);
        yesButton.onClick.AddListener(ClosePanel);
        yesButton.GetComponentInChildren<Text>().text = yesText;


        this.question.text = question;

        this.iconImage.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }

    void ClosePanel()
    {
        modalPanelObject.SetActive(false);
    }
}