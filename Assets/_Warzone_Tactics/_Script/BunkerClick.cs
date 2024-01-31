using UnityEngine;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class BunkerClick : MonoBehaviour
    {
        [SerializeField] int _bunkerNum;

        CannonFireScript cannonFireScript;

        private void Start()
        {
            cannonFireScript = GameObject.Find("[GameManager]").GetComponent<CannonFireScript>();
        }

        private void OnMouseDown()
        {
            if (_bunkerNum < 6)
            {
                if (UIController.Instance.WorldCanvasPlayer.activeInHierarchy)
                {
                    GamePlayController.Instance.OnTroopPlaced(_bunkerNum);
                }
            }
            else
            {
                if (UIController.Instance.WorldCanvasOpponent.activeInHierarchy)
                {
                    UIController.Instance.OnAttackBunkerSelected(_bunkerNum);
                }
            }
        }
    }
}

