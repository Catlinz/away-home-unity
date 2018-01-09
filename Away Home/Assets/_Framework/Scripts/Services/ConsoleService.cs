using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public delegate void ConsoleCommandHandler(string[] args);

public class ConsoleService {

    #region Event declarations
    public delegate void LogChanged(string[] log);
    public event LogChanged onLogChanged;

    public delegate void VisibilityChanged(bool visible);
    public event VisibilityChanged onVisibilityChanged;
    #endregion

    class ConsoleCommand
    {
        public string Command { get; private set; }
        public ConsoleCommandHandler Handler { get; private set; }
        public string Help { get; private set; }

        public ConsoleCommand(string command, ConsoleCommandHandler handler, string help) {
            Command = command;
            Handler = handler;
            Help = help;
        }
    }

    const int scrollbackSize = 20;

    Queue<string> scrollback = new Queue<string>(scrollbackSize);
    List<string> commandHistory = new List<string>();
    Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();

    public string[] Log { get; private set; }

    public ConsoleService() {
        RegisterCommand("echo", Echo, "Echos arguments back as an array (for testing argument parser)");
        RegisterCommand("help", Help, "List the commands and their help strings.");
#if UNITY_EDITOR
        RegisterCommand("install_module", InstallModule, "Install a module on a ship.");
#endif
        RegisterCommand("reload", Reload, "Reload game.");
    }

    public void RegisterCommand(string command, ConsoleCommandHandler handler, string help) {
        commands.Add(command, new ConsoleCommand(command, handler, help));
    }

    public void AppendLogLine(string line) {
        Debug.Log(line);

        if (scrollback.Count >= ConsoleService.scrollbackSize) {
            scrollback.Dequeue();
        }
        scrollback.Enqueue(line);

        Log = scrollback.ToArray();
        if (onLogChanged != null) { onLogChanged(Log);  }
    }

    public void ParseAndRunCommand(string commandString) {
        if (commandString == "") {
            return;
        }
        AppendLogLine("$" + commandString);

        string[] commandSplit = ParseArguments(commandString);
        string[] args = new string[0];

        if (commandSplit.Length < 1) {
            AppendLogLine(string.Format("Unable to process command '{0}'", commandString));
            return;
        }
        else if (commandSplit.Length > 1) {
            int numArgs = commandSplit.Length - 1;
            args = new string[numArgs];
            for (int i = 1; i < commandSplit.Length; ++i) {
                args[i - 1] = commandSplit[i];
            }
        }

        RunCommand(commandSplit[0].ToLower(), args);
        commandHistory.Add(commandString);
    }

    public void RunCommand(string command, string[] args) {
        ConsoleCommand cmd = null;
        if (!commands.TryGetValue(command, out cmd)) {
            AppendLogLine(string.Format("Unknown command '{0}', type 'help' for list.", command));
        }
        else {
            if (cmd.Handler == null) {
                AppendLogLine(string.Format("Unable to process command '{0}', handler was null", command));
            }
            else {
                cmd.Handler(args);
            }
        }
    }

    static string[] ParseArguments(string commandString) {
        LinkedList<char> parmChars = new LinkedList<char>(commandString.ToCharArray());
        bool inQuote = false;
        var node = parmChars.First;
        while (node != null) {
            var next = node.Next;
            if (node.Value == '"') {
                inQuote = !inQuote;
                parmChars.Remove(node);
            }
            if (!inQuote && node.Value == ' ') {
                node.Value = '\n';
            }
            node = next;
        }
        char[] parmCharsArr = new char[parmChars.Count];
        parmChars.CopyTo(parmCharsArr, 0);
        return (new string(parmCharsArr)).Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
    }

    #region Command Handlers
    void Echo(string[] args) {
        StringBuilder sb = new StringBuilder();
        foreach (string arg in args) {
            sb.AppendFormat("{0},", arg);
        }
        sb.Remove(sb.Length - 1, 1);
        AppendLogLine(sb.ToString());
    }

    void Help(string[] args) {
        foreach(ConsoleCommand cmd in commands.Values) {
            AppendLogLine(string.Format("{0}: {1}", cmd.Command, cmd.Help));
        }
    }

#if UNITY_EDITOR
    void InstallModule(string[] args) {
        string shipName = args[0];
        string assetPath = args[1];
        string socketName = args[2];

        AppendLogLine(shipName);

        GameObject obj = GameObject.Find(shipName);
        if (obj) {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
            if (prefab != null) {
                OperationResult result = obj.GetComponent<CoreSystemComponent>().InstallModuleIn(socketName, prefab);
                if (result.status != OperationResult.Status.OK) {
                    AppendLogLine(result.message);
                }
            }
            else {
                AppendLogLine("Failed to find Asset.");
            }
            
        }
        else {
            AppendLogLine("Failed to find GameObject.");
        }
    }
#endif

    void Reload(string[] args) {
        Application.LoadLevel(Application.loadedLevel);
    }

    void FindByName(string[] args) {
        string name = args[0];
        GameObject obj = GameObject.Find(name);
        if (obj) {
            AppendLogLine(obj.name);
        }
        else {
            AppendLogLine("Object '" + name + "' not found.");
        }
    }
    #endregion

}
