using TMPro;
using UnityEngine;

public class LoadingTextAnimation : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    public float dotInterval = 0.5f; // Interval between dot additions
    private float timer;

    private void Awake()
    {
        loadingText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        if (loadingText == null)
        {
            Debug.LogError("TextMeshProUGUI component not assigned!");
            return;
        }

        timer = dotInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            UpdateDots();
            timer = dotInterval;
        }
    }

    void UpdateDots()
    {
        string currentText = loadingText.text;

        // Check if there are already three dots
        if (currentText.EndsWith("..."))
        {
            loadingText.text = "Loading"; // Reset to "Loading" without dots
        }
        else
        {
            loadingText.text += "."; // Add a dot
        }
    }
}
