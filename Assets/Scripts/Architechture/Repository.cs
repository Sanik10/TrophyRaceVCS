namespace TrophyRace.Architecture {
    public abstract class Repository {

        public abstract void Initialize();

        public virtual void OnCreate() { 
            // Ваша логика создания, если нужно
        }

        public virtual void OnStart() { 
            // Ваша логика старта, если нужно
        }

        public abstract void Save();
    }
}