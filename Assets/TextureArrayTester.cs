using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextureArrayTester : MonoBehaviour {
    Text text;
    void Awake()
    {
        text = GetComponent<Text>();
    }

    // Use this for initialization
    void Start ()
    {

        text.text =
            "I want to implement some more advanced graphical features into " +
            "Armok Vision, but I would like to first survey the users to see " +
            "if said features are actually supported by the user base. " +
            "Kindly fill out the linked survey according " +
            "to the info given here:\n\n" +
            "Supports 2D Array Textures: " + SystemInfo.supports2DArrayTextures + "\n" +
            "Supports Instancing: " + SystemInfo.supportsInstancing + "\n\n" +
            "After you click the close button, this message will not appear again.";
	}
}
