using Godot;
using System;

public class FireAttack : ICardAction {

    public static CardActionResult attack {get;} = new() {
        action_attack = 3,
        type = Element.Fire
    };

    CardActionResult ICardAction.onAttack(bool heads, Element type, Entity calling_entity, Entity effected_entity) {
        return attack;
    }
}
