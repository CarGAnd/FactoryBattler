using System;
using AttributeSystem;
using UnityEngine;

namespace PlayerSystem {

    [Serializable]
    public class BasePlayer : PoolMember, IPlayer
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private string id;
        [SerializeField]
        private IScoreHolderStrategy scoreStrategy;
        [SerializeField]
        private FactoryGrid factoryGrid;
        [SerializeField]
        private IPlayer opponent;
        [SerializeField]
        private bool isDead = false;
        [SerializeField]
        private PlayerModeManager playerModeManager;

        public string Name { get => name; private set => name = value; }
        public string Id { get => id; private set => id = value; }
        public IScoreHolderStrategy ScoreStrategy { get => scoreStrategy; private set => scoreStrategy = value; }
        public bool IsDead { get => isDead; private set => isDead = value; }
        public FactoryGrid FactoryGrid { get => factoryGrid; private set => factoryGrid = value; }
        public IPlayer Opponent { get => opponent; private set => opponent = value; }
        public PlayerModeManager PlayerModeManager { get => playerModeManager; private set => playerModeManager = value; }

        void IKillable.OnDeath(float health)
        {
            if (health > 0)
                return;
                
            IsDead = true;

            if (IsDead) {
                Debug.Log("Player died.");
            }
        }

        public BasePlayer (string name, IScoreHolderStrategy scoreStrategy) 
        {
            Name = name;
            ScoreStrategy = scoreStrategy;
            CalculateNewID();
            (this as IKillable).Initialize();
        }

        public BasePlayer (string name, IScoreHolderStrategy scoreStrategy, ulong playerId) 
        {
            Name = name;
            ScoreStrategy = scoreStrategy;
            this.Id = playerId.ToString();
            (this as IKillable).Initialize();
        }


        public void CalculateNewID() {
            Id = Guid.NewGuid().ToString();
        }
    }
}