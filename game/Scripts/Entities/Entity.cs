
using Godot;
using System.Linq;

[GlobalClass]
public partial class Entity : Resource {
    
    [Export] public double health {get; protected set;} = 10;

    [Export] public double max_health {get; protected set;} = 10;

    /// <summary>
    /// This field will be reset each turn
    /// </summary>
    [Export] public double defense {get; protected set;} = 0;

    [Export] public Element element {get; protected set;} = Element.None;

    [Export] public Inventory inventory {get; set;} = Inventory.of(5);

    public void handleDamage(double amount) {
        var dmg = Mathf.Max(amount - this.defense, 0);
        this.defense -= amount;
        this.health -= dmg;
    }
}

public interface IEntityHolder {
    [Export] public Entity entity {get; set;}
}
