using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class PublicSession : MonoBehaviour
    {
        private Button _publicSessionBtn;

        private GameObject _publicPanel;
        private Button _publicSessionBackButton;


        private void Awake()
        {
            _publicSessionBtn = GameObject.Find("Public_Button").GetComponent<Button>();

            _publicPanel = GameObject.Find("Public_Panel");
            _publicSessionBackButton = GameObject.Find("PublicSessionBack_Button").GetComponent<Button>();
        }

        private void Start()
        {
            _publicSessionBtn.onClick.AddListener(() => { OnPublicSessionBtnClick(); });
            _publicSessionBackButton.onClick.AddListener(() => { OnPublicSessionBackBtnClick(); });

            _publicPanel.SetActive(false);
        }

        private void OnPublicSessionBtnClick()
        {
            _publicPanel.SetActive(true);

            FusionManager.Instance.GameRoomAutoMatch();
        }

        public void OnPublicSessionBackBtnClick()
        {
            _publicPanel.SetActive(false);
            FusionManager.Instance.Runner.Shutdown();
        }
    }
}

