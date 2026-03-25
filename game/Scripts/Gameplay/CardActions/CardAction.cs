
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class CardAction : Resource {

    // public virtual Element element => Element.None; // shouldn't need

    /// <summary>
    /// Called when an action is used for a card
    /// </summary>
    /// <param name="type">The type of action this is called for</param>
    /// <param name="effect_element">The element of the effect</param>
    /// <param name="selection">Who this is affecting</param>
    /// <param name="calling_entity">The entity using the effect</param>
    /// <param name="effected_entity">The entity targeted by the effect</param>
    /// <returns></returns>
    public virtual CardActionResult onAction(CardEffect.EffectType type, Element effect_element, Selection selection, Entity calling_entity, Entity effected_entity, Dictionary<string, Variant> custom_args) {
        return CardActionResult.EMPTY;
    }

}
public class CardActionResult {
    /// <summary>
    /// modifier of return to the inital card effect
    /// </summary>
    public ModifierType modifier = ModifierType.Additive;

    public Element type;
    public double amount = 0d;
    public Selection new_target = Selection.Same;

    public static CardActionResult EMPTY {get;} = new();
}
