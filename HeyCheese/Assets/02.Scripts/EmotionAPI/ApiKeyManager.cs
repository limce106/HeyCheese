using System;
using System.Text;
using UnityEngine;

public class ApiKeyManager : MonoBehaviour
{
    public static string GetApiKey()
    {
        string encryptedKey = "W1BGBhM+WhpUC1ISRA==";
        byte xorKey = 0x5A;

        byte[] bytes = Convert.FromBase64String(encryptedKey);
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] ^= xorKey;
        }

        string apiKey = Encoding.UTF8.GetString(bytes);
        return apiKey;
    }
}
