// StageNodeUI.cs
using UnityEngine;
using UnityEngine.UI;

public class StageNodeUI : MonoBehaviour
{
    public enum NodeState { Locked, Available, Cleared, GreyedOut }

    [Header("Identity")]
    public string nodeId;
    public string debugLabel;

    [Header("Level Data")]
    public string displayName;
    [TextArea] public string description;
    public string sceneName;

    [Header("Runtime")]
    public NodeState State = NodeState.Locked;

    public Image frame;
    public GameObject selectedFx;

    public Color availableColor = Color.white;
    public Color clearedColor = Color.white;
    public Color lockedColor = new Color(1f, 1f, 1f, 0.35f);
    public Color greyedOutColor = new Color(1f, 1f, 1f, 0.25f);

    Button _btn;

    void Awake()
    {
        _btn = GetComponent<Button>();

        // Safety: don't let selectedFx be the root object
        if (selectedFx == gameObject)
        {
            Debug.LogWarning($"{name}: selectedFx is set to the root object. Clearing it to avoid disabling the node.");
            selectedFx = null;
        }
    }

    public void SetState(NodeState _state, bool selected)
    {
        State = _state;

        if (_btn) _btn.interactable = (_state == NodeState.Available);
        if (selectedFx) selectedFx.SetActive(selected);

        Color c;
        switch (_state)
        {
            case NodeState.Locked: c = lockedColor; break;
            case NodeState.Available: c = availableColor; break;
            case NodeState.Cleared: c = clearedColor; break;
            case NodeState.GreyedOut: c = greyedOutColor; break;
            default: c = availableColor; break;
        }

        if (frame) frame.color = c;
    }
}