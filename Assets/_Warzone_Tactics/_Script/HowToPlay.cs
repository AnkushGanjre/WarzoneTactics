using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class HowToPlay : MonoBehaviour
    {
        GameObject _howToPlayPanel;
        Button _howToPlay;
        Button _howToPlayNextButton;
        Button _howToPlayCloseButton;

        private void Awake()
        {
            _howToPlayPanel = GameObject.Find("HowToPlay_Panel");
            _howToPlay = GameObject.Find("How_To_Play_Button").GetComponent<Button>();
            _howToPlayNextButton = GameObject.Find("HowToPlayNext_Button").GetComponent<Button>();
            _howToPlayCloseButton = GameObject.Find("HowToPlayClose_Button").GetComponent<Button>();
        }

        void Start()
        {
            _howToPlayPanel.SetActive(false);
            _howToPlay.onClick.AddListener(() => { OnHowToPlayBtnClicked(); });
            _howToPlayNextButton.onClick.AddListener(() => { OnHowToPlayNextBtn(); });
            _howToPlayCloseButton.onClick.AddListener(() => { OnHowToPlayCloseBtn(); });
        }

        private void OnHowToPlayBtnClicked()
        {
            _howToPlayPanel.SetActive(true);
        }

        private void OnHowToPlayNextBtn()
        {

        }

        private void OnHowToPlayCloseBtn()
        {
            _howToPlayPanel.SetActive(false);
        }
    }
}

