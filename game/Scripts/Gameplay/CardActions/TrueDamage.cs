
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class TrueDamage : CardAction {

    public override CardActionResult onAction(CardEffect.EffectType type, Element effect_element, Selection selection, Entity calling_entity, Entity effected_entity, Dictionary<string, Variant> custom_args) {
        if (type != CardEffect.EffectType.Attack) {
            return CardActionResult.EMPTY;
        }
        double amount = 5;
        if (custom_args.TryGetValue("amount", out var _amount)) {
            amount = (double) _amount;
        }
        return new () {
            amount = amount,
            modifier = ModifierType.True,
            type = effect_element
        };
    }
}
