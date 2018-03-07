using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : Singleton<UI> {

    private GameMenuUI _gameMenu;
    private HUDUI _HUD;
    private InterfaceUI _interface;

    /// <summary>
    /// The GameMenuUI that is used to display things like the pause and settings menu to the user.
    /// </summary>
    public static GameMenuUI GameMenu {
        get {
            UI instance = UI.Instance;
            if (instance != null) {
                if (instance._gameMenu == null) {
                    instance._gameMenu = instance.GetComponentInChildren<GameMenuUI>(true);
                }
                return instance._gameMenu;
            }
            return null;
        }
    }

    /// <summary>
    /// The HUD UI that is used to display the Ship HUD to the user.
    /// </summary>
    public static HUDUI HUD {
        get {
            UI instance = UI.Instance;
            if (instance != null) {
                if (instance._HUD == null) {
                    instance._HUD = instance.GetComponentInChildren<HUDUI>(true);
                }
                return instance._HUD;
            }
            return null;
        }
    }

    /// <summary>
    /// The InterfaceUI that is used to display the Ship and other interactive UI's to the user.
    /// </summary>
    public static InterfaceUI Interface {
        get {
            UI instance = UI.Instance;
            if (instance != null) {
                if (instance._interface == null) {
                    instance._interface = instance.GetComponentInChildren<InterfaceUI>(true);
                }
                return instance._interface;
            }
            return null;
        }
    }
}
