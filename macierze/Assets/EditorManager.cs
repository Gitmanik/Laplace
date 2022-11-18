using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class EditorManager : MonoBehaviour
{
    public static EditorManager Instance;

    [SerializeField] private Transform EditorObj;

    [SerializeField] private TMP_InputField Input;

    private void Awake()
    {
        Instance = this;

        Input.onSubmit.AddListener(OnInput);
    }

    private void OnInput(string arg0)
    {
        MatrixGenerator.Instance.EditSubmitted(arg0);
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