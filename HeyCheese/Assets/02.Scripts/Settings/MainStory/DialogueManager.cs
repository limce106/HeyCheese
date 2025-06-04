using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public Color colorBoogiName = new Color(102, 213, 117, 255); // ÃÊ·Ï
    public Color colorCheeseName = new Color(112, 195, 255, 255); // ÆÄ¶û
    public Color colorSystemName = new Color(179, 179, 179, 255); // È¸»ö
    public Color colorUserName = new Color(253, 212, 113, 255); // ³ë¶û

    public Color colorBoogiDialogue = new Color(205, 244, 211, 255); // ÃÊ·Ï
    public Color colorCheeseDialogue = new Color(194, 229, 255, 255); // ÆÄ¶û
    public Color colorSystemDialogue = new Color(217, 217, 217, 255); // È¸»ö
    public Color colorUserDialogue = new Color(255, 236, 189, 255); // ³ë¶û

    // ´ë»ç »ö º¯°æ
    public (Color nameColor, Color dialogueColor) GetColorBySpeaker(Speaker speaker)
    {
        switch (speaker)
        {
            case Speaker.Boogi:
                return (colorBoogiName, colorBoogiDialogue);
            case Speaker.Cheese:
                return (colorCheeseName, colorCheeseDialogue);
            case Speaker.User:
                return (colorUserName, colorUserDialogue);
            case Speaker.System:
                return (colorSystemName, colorSystemDialogue);
            case Speaker.Other:
                return (colorSystemName, colorSystemDialogue);
            default:
                return (colorSystemName, colorSystemDialogue);
        }
    }
}
