using UnityEngine;
using UnityEngine.UI;

public class StageNodeUI : MonoBehaviour
{
    public string nodeId;
    public string debugLabel;

    public Image frame;
    public GameObject selectedFx;

    public Color availableColor = Color.white;
    public Color clearedColor = Color.white;
    public Color lockedColor = new Color(1f, 1f, 1f, 0.35f);
    public Color greyedOutColor = new Color(1f, 1f, 1f, 0.25f);

    Button _btn;

    public enum NodeState { Locked, Available, Cleared, GreyedOut }
    public NodeState State { get; private set; } = NodeState.Locked;

    void Awake() => _btn = GetComponent<Button>();

    public void SetState(NodeState state, bool selected)
    {
        State = state;
        if (_btn) _btn.interactable = (state == NodeState.Available);
        if (selectedFx) selectedFx.SetActive(selected);

        Color c = availableColor;
        switch (state)
        {
            case NodeState.Locked: c = lockedColor; break;
            case NodeState.Available: c = availableColor; break;
            case NodeState.Cleared: c = clearedColor; break;
            case NodeState.GreyedOut: c = greyedOutColor; break;
        }

        if (frame) frame.color = c;
    }
}