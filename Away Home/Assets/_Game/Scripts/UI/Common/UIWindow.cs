using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindow : MonoBehaviour {

    private static readonly float[] WIDTHS = { 0.0f, 0.3f, 0.5f, 0.7f, 0.9f, 1.0f };
    private static readonly float[] HEIGHTS = { 0.0f, 0.4f, 0.6f, 0.8f, 0.95f, 1.0f };

    #region FIELDS
    /** The title bar for the window, if any. */
    public UITitleBar titleBar;

    /** The scroll view for the window. */
    public ScrollRect view;

    public UISize width = UISize.Auto;

    public UISize height = UISize.Auto;

    private RectTransform _rectTx = null;
    #endregion FIELDS

    public UIWindow SetWidth(UISize width) {
        return SetSize(width, this.height);
    }

    public UIWindow SetHeight(UISize height) {
        return SetSize(this.width, height);
    }

    public UIWindow SetSize(UISize width, UISize height) {
        if (width != this.width) {
            float actual_width = _rectTx.rect.width;

            switch (width) {
                case UISize.Full:
                    actual_width = _rectTx
                
            }
        }
    }

    // Use this for initialization
    void Start () {
        _rectTx = GetComponent<RectTransform>();


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
