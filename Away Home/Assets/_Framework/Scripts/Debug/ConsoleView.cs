using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleView : MonoBehaviour {

    ConsoleService console;

    bool didShow = false;

    public GameObject consoleView;
    public Text logTextArea;
    public InputField inputField;

    void Awake() {
        if (console == null) {
            console = new ConsoleService();
            Service.Set<ConsoleService>(console);
        }
    }

	// Use this for initialization
	void Start () {
		if (console != null) {
            console.onVisibilityChanged += OnVisibilityChanged;
            console.onLogChanged += OnLogChanged;
            UpdateLogStr(console.Log);
        }
	}

    void OnDestroy() {
        console.onVisibilityChanged -= OnVisibilityChanged;
        console.onLogChanged -= OnLogChanged;
        Service.Clear<ConsoleService>();
    }
	
	// Update is called once per frame
	void Update () {
		// Toggle visibility when ~ is pressed.
        if (Input.GetKeyUp("`")) {
            ToggleVisibility();
        }

        // Toggle visibility when 5 fingers tough.
        if (Input.touches.Length == 5) {
            if (!didShow) {
                ToggleVisibility();
                didShow = true;
            }
        }
        else {
            didShow = false;
        }
	}

    void ToggleVisibility() {
        SetVisibility(!consoleView.activeSelf);
    }
    void SetVisibility(bool visible) {
        consoleView.SetActive(visible);
    }

    void OnVisibilityChanged(bool visible) {
        SetVisibility(visible);
    }

    void OnLogChanged(string[] newLog) {
        UpdateLogStr(newLog);
    }

    void UpdateLogStr(string[] newLog) {
        if (newLog == null) {
            logTextArea.text = "";
        }
        else {
            logTextArea.text = string.Join("\n", newLog);
        }
    }

    public void RunCommand() {
        console.ParseAndRunCommand(inputField.text);
        inputField.text = "";
    }
}
