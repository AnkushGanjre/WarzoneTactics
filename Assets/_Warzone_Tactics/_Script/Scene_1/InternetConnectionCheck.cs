using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InternetConnectionCheck : MonoBehaviour
{
    private float _connectionTimeout = 5f;
    private GameObject _internetUnavailable;
    private Button _tryAgainInternetConnectBtn;

    private void Awake()
    {
        _internetUnavailable = GameObject.Find("Internet_Unavailable");
        _tryAgainInternetConnectBtn = GameObject.Find("TryAgain_InternetConnect_Btn").GetComponent<Button>();
        _internetUnavailable.SetActive(false);
    }

    private void Start()
    {
        _tryAgainInternetConnectBtn.onClick.AddListener(() => { OnTryAgain(); });

        StartCoroutine(CheckInternetConnection());
    }

    private IEnumerator CheckInternetConnection()
    {
        UnityWebRequest www = new UnityWebRequest("https://www.google.com");
        www.timeout = Mathf.CeilToInt(_connectionTimeout);

        yield return www.SendWebRequest();

        // Wait for 5 seconds before displaying the result
        yield return new WaitForSeconds(5f);

        if (www.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("Internet connection is available");
            SceneManager.LoadScene("Warzone");
        }
        else
        {
            //Debug.Log("No internet connection. Please check your network.");
            _internetUnavailable.SetActive(true);
        }
    }

    private void OnTryAgain()
    {
        _internetUnavailable.SetActive(false);
        StartCoroutine(CheckInternetConnection());
    }
}
