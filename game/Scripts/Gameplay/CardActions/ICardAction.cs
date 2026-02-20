
using Godot;

public interface ICardAction {

    public virtual ModifierType modifier => ModifierType.Additive;
    public virtual Element element => Element.None;

    /// <summary>
    /// Will be called when the card for an attack
    /// </summary>
    /// <param name="heads"></param>
    /// <param name="type"></param>
    /// <param name="calling_entity"></param>
    /// <param name="effected_entity"></param>
    /// <returns></returns>
    public virtual CardActionResult onAttack(bool heads, Element type, Entity calling_entity, Entity effected_entity) {
        return CardActionResult.EMPTY;
    }

    /// <summary>
    /// Will be used when the entity using this card is attacked
    /// </summary>
    /// <param name="heads"></param>
    /// <returns></returns>
    public virtual CardActionResult onDefend(bool heads, Element type, Entity calling_entity, Entity effected_entity) {
        return CardActionResult.EMPTY;
    }

    /// <summary>
    /// Will be called when a linger or over time effects are calculated
    /// </summary>
    /// <param name="heads"></param>
    /// <returns></returns>
    public virtual CardActionResult onLinger(bool heads, Element type, Entity calling_entity, Entity effected_entity) {
        return CardActionResult.EMPTY;
    }

}
public class CardActionResult {
    public Element type;
    public double action_attack = 0d;
    public double action_defense = 0d;
    public Selection new_target = Selection.Same;

    public static CardActionResult EMPTY {get;} = new();
}
