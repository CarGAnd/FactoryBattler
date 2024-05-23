namespace PlayerSystem {
    public interface IPlayer : IHealable, IDamagable, IKillable
    {
        string Name {get;}
        string Id {get;}
        IScoreHolderStrategy ScoreStrategy {get;}
        FactoryGrid FactoryGrid {get;}
        IPlayer Opponent {get;}
        PlayerController PlayerModeManager {get;}
        //TODO Hand hand {get;}
        //TODO Deck deck {get;}
    }
}