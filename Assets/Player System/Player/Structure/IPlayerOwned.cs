namespace PlayerSystem {
    public interface IPlayerOwned {
        IPlayer Owner {get; set;}

        virtual void SetOwner(IPlayer owner) 
        {
            this.Owner = owner;
        }
    }
}