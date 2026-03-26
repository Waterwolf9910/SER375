using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class CardEffect : Resource {

    public enum EffectType {
        Attack, Defense, Heal, Enhance, Linger
    }

    [Export] public EffectType type { get; set; } = EffectType.Attack;

    /// <summary>
    /// The value applied when this coin lands heads (e.g. +10 attack)
    /// </summary>
    [Export] public double value { get; set; } = 0;

    public CardAction? _custom_action = null;
    /// <summary>
    /// Optional script for custom effects
    /// </summary>
    [Export]
    public Script? custom_action_script {
        get => _custom_action?.GetScript().As<Script>();
        set {
            if (value == null) {
                _custom_action = null;
                return;
            }
            if (!value.CanInstantiate() || value is not CSharpScript script || script.New().AsGodotObject() is not CardAction action) {
                return;
            }
            _custom_action = action;
        }
    } // for editor

    public CardAction? custom_action => _custom_action; // for us

    /// <summary>
    /// Extra arguments for card actions
    /// </summary>
    [Export]
    Dictionary<string, Variant> custom_args = [];


    [Export]
    public Element element { get; private set; } = Element.None;

    [Export]
    public Selection selection { get; private set; } = Selection.Target;

}
