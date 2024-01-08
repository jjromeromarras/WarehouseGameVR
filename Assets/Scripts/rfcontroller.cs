using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class rfcontroller : MonoBehaviour
{
    public static rfcontroller instance;

    [Header("UI elements")]
    public TextMeshProUGUI[] lines;
    public GameObject[] buttons;
    public TextMeshProUGUI title;
    public TextMeshProUGUI prompt;

    public
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ResetRf();
    }

    public void EnableButton(int button)
    {
        buttons[button].SetActive(true);
    }

    public void ResetRf()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            this.writeText(lines[i], string.Empty);
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false);
            this.writeText(buttons[i].GetComponentInChildren<TextMeshProUGUI>(), string.Empty);
        }
        writeText(title, string.Empty);
        writeText(prompt, string.Empty);

    }
    public void writeButtonText(int button, string text)
    {
        this.writeText(buttons[button].GetComponentInChildren<TextMeshProUGUI>(), text);
    }
    public void writeText(TextMeshProUGUI UIText, string text)
    {
        if (UIText != null)
            UIText.text = text;
    }
    // Update is called once per frame
}
