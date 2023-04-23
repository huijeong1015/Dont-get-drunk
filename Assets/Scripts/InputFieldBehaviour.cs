using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class InputFieldBehaviour : MonoBehaviour{
    public TMP_InputField inputField;

    void Awake()
    {
        RestoreFocus();
    }

    // Activate the main input field when the Scene starts.
    void Start() {
        RestoreFocus();
    }
    
    void Update() {
        // Reset input field when input is entered
        if (Input.GetKeyDown(KeyCode.Return)) {
            ClearInputField();
            RestoreFocus();
        }
    }

    void ClearInputField() {
        inputField.text = "";
    }

    void RestoreFocus() {
        inputField.ActivateInputField();
    }
}
