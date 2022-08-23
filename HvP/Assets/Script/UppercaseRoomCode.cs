using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UppercaseRoomCode : MonoBehaviour
{
    public InputField inputField;
    void Start()
    {
        inputField.onValidateInput +=
            delegate (string s, int i, char c) { return char.ToUpper(c); };
    }

}
