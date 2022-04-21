using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public int maxMessages = 25;

    public GameObject CombatLog, TextObject;

    public HUDAnimationManager HUDanimationManager;

    [SerializeField]
    List<Message> messageList = new List<Message>();

    public void Awake()
    {
        HUDanimationManager = GetComponent<HUDAnimationManager>();
    }

    // This function sends a message to the combat log
    public void SendMessageToCombatLog(string text)
    {
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();
        newMessage.text = text;

        GameObject newText = Instantiate(TextObject, CombatLog.transform);
        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();
        newMessage.textObject.text = newMessage.text;

        messageList.Add(newMessage);
        HUDanimationManager.UpdateScrollBar();
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public TextMeshProUGUI textObject;
}
