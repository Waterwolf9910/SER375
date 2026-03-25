using Godot;
using Godot.Collections;
using System;

public partial class ElementalAttack : CardAction {

    public override CardActionResult onAction(CardEffect.EffectType type, Element effect_element, Selection selection, Entity calling_entity, Entity effected_entity, Dictionary<string, Variant> custom_args) {
        if (type != CardEffect.EffectType.Attack) {
            return CardActionResult.EMPTY;
        }
        return new() {
            amount = 1,
            modifier = ModifierType.Multiplicative,
            type = effect_element
        };
    }
}
