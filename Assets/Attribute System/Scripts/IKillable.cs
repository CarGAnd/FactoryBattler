using AttributeSystem;

public interface IKillable {
    AttributeManager AttributeManager { get; }
    bool IsDead { get; set;}
    void OnDeath(float health);
    void Initialize() {
        AttributeManager.GetBaseAttributeInstance("HealthBase").ValueChangedTo.AddListener(OnDeath);
    }
}