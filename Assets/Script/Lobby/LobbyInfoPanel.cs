using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class LobbyInfoPanel : MonoBehaviour
{
    public Text infoText;
    public Text buttonText;
    public Button singleButton;

    public void Display(string info, string buttonInfo, UnityAction buttonClbk)
    {
        infoText.text = info;

        buttonText.text = buttonInfo;

        singleButton.onClick.RemoveAllListeners();

        if (buttonClbk != null)
        {
            singleButton.onClick.AddListener(buttonClbk);
        }

        singleButton.onClick.AddListener(() => { gameObject.SetActive(false); });

        gameObject.SetActive(true);
    }
}