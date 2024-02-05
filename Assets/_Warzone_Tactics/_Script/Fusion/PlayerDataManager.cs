using Fusion;
using System.Collections.Generic;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class PlayerDataManager: NetworkBehaviour
    {
        public string NickName;
        public string OpponentName;

        public int PlayerAvatarNum;
        public int OpponentAvatarNum;

        public int PlayerTrophyCount;
        public int OpponentTrophyCount;

        public int PlayerRankNum;
        public int PlayerCoinCount;

        public NetworkObject LocalPlayerObj;
        public NetworkObject RemotePlayerObj;

        public PlayerRef LocalPlayerRef;
        public PlayerRef RemotePlayerRef;

        public int PlayerTroopSelection;
        public int OpponentTroopSelection;
        public int PlayerAttackSelection;
        public int OpponentAttackSelection;

        public List<int> PlayerTroopList = new List<int>();
        public List<int> OpponentTroopList = new List<int>();

        public bool didHostReqRematch = false;
        public bool didClientReqRematch = false;
    }
}