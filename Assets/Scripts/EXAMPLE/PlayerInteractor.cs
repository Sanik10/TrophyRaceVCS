using UnityEngine;

namespace TrophyRace.Architecture {
    public class PlayerInteractor : Interactor {
        
        public Player player { get; private set; }

        public override void Initialize() {
            base.Initialize();

            var goPlayer = new GameObject("PlayerExample");
            this.player = goPlayer.AddComponent<Player>(); // создается игрок и вешает Player
        }
    }
}