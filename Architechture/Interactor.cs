namespace TrophyRace.Architecture {
    public abstract class Interactor {

        public virtual void OnCreate() { } // Когда все репозитории и интеракторы созданы

        public virtual void Initialize() { } // Когда все репозитории и интеракторы прошли OnCreate

        public virtual void OnStart() { } // Когда все репозитории и интеракторы проинициализированы
    }
}