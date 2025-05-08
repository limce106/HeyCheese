using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerUtil
{
    public static Speaker ParseSpeakerID(string speakerID)
    {
        switch (speakerID.Trim())
        {
            case "부기":
                return Speaker.Boogi;
            case "치즈":
                return Speaker.Cheese;
            case "시스템":
                return Speaker.System;
            case "유저":
                return Speaker.User;
            default:
                return Speaker.Other;
        }
    }
}
