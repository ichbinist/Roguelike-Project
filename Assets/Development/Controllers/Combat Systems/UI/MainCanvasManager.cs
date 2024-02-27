using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MainCanvasManager : Singleton<MainCanvasManager>
{
    private List<IPanel> panels = new List<IPanel>();
    private List<KeyCode> keys = new List<KeyCode>();

    [HideInInspector]
    public UnityEvent<IPanel> OnPanelOpened = new UnityEvent<IPanel>();

    [HideInInspector]
    public UnityEvent<IPanel> OnPanelClosed = new UnityEvent<IPanel>();

    public bool IsAnyLockedPanelActive { get { return panels.Any(x => x.PanelType == PanelType.Locked && x.IsOpen); } }
    public bool IsAnyStandartPanelActive { get { return panels.Any(x => x.PanelType == PanelType.Standart && x.IsOpen); } }
    public IPanel GetActiveStandartPanel { get { return panels.Find(x => x.PanelType == PanelType.Standart && x.IsOpen); } }
    public IPanel GetTargetPanelByKeyCode(KeyCode key) { return panels.Find(x => x.PanelKey == key); }

    private void OnEnable()
    {
        OnPanelOpened.AddListener(PanelOpened);
        OnPanelClosed.AddListener(PanelClosed);
    }

    private void OnDisable()
    {
        OnPanelOpened.RemoveListener(PanelOpened);
        OnPanelClosed.RemoveListener(PanelClosed);
    }

    private void Start()
    {
        foreach (Transform child in transform)
        {
            IPanel panel = child.GetComponentInChildren<IPanel>();
            if(panel != null)
            {
                panels.Add(panel);
                keys.Add(panel.PanelKey);
            }
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (IsAnyLockedPanelActive) return;

            if (IsAnyStandartPanelActive)
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    GetActiveStandartPanel.ClosePanel();
                    return;
                }

                foreach (KeyCode keyCode in keys)
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        if(GetActiveStandartPanel == GetTargetPanelByKeyCode(keyCode))
                        {
                            GetTargetPanelByKeyCode(keyCode).ClosePanel();
                        }
                        break;
                    }
                }
            }
            else
            {
                foreach (KeyCode keyCode in keys)
                {
                    // Check if the current key is pressed
                    if (Input.GetKeyDown(keyCode))
                    {
                        GetTargetPanelByKeyCode(keyCode).OpenPanel();
                        break;
                    }
                }
                
            }
        }
    }

    private void PanelOpened(IPanel panel)
    {
        if(panel.PanelType == PanelType.Locked || panel.PanelType == PanelType.Standart)
        {
            foreach (IPanel _panel in panels)
            {
                if(_panel.IsOpen && _panel != panel && (_panel.PanelType == PanelType.Elastic || _panel.PanelType == PanelType.Standart))
                {
                    _panel.ClosePanel();
                }
            }
        }
    }

    private void PanelClosed(IPanel panel)
    {
        if (panel.PanelType == PanelType.Locked || panel.PanelType == PanelType.Standart)
        {
            foreach (IPanel _panel in panels)
            {
                if (_panel != panel && (_panel.PanelType == PanelType.Elastic))
                {
                    _panel.OpenPanel();
                }
            }
        }
    }
}