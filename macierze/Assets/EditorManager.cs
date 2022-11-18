using TMPro;
using UnityEngine;

public class EditorManager : MonoBehaviour
{
    public static EditorManager Instance;

    [SerializeField] private Transform EditorObj;

    [SerializeField] private TMP_InputField Input;

    private void Start()
    {
        Instance = this;

        Input.onValueChanged.AddListener(OnInput);
    }

    private void OnInput(string arg0)
    {
        MatrixGenerator.Instance.Edited(arg0);
    }

    public void StartEdit(string currentInput)
    {
        Input.text = currentInput;
        EditorObj.gameObject.SetActive(true);
    }

    public void EndEdit()
    {
        Input.text = "";
        EditorObj.gameObject.SetActive(false);
    }
}