using UnityEngine;

namespace TrophyRace.Architecture {
    public class BankRepository : Repository {

        private const string KEY_PREFIX = "BANK_KEY_";
        private const string TYPTOL_KEY = KEY_PREFIX + "TYPTOL";
        private const string QBIT_KEY = KEY_PREFIX + "QBIT";

        // Game currency
        public int typtol { get; set; }
        public int qbit { get; set; }
        
        public override void Initialize() {
            this.typtol = PlayerPrefs.GetInt(TYPTOL_KEY, 0);
            this.qbit = PlayerPrefs.GetInt(QBIT_KEY, 0);
        }

        public override void Save() {
            PlayerPrefs.SetInt(TYPTOL_KEY, this.typtol);
            PlayerPrefs.SetInt(QBIT_KEY, this.qbit);
        }
    }
}

