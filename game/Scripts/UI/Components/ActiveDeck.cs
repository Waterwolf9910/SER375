
using Godot;
using Godot.Collections;

[Tool]
public partial class ActiveDeck : VBoxContainer {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Export] private CardComponent anti_hero_top;
    [Export] private CardComponent anti_hero_bottom;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Export] private Array<CardComponent> helpers = [];

    public CardComponent anti_hero => is_top ? anti_hero_top : anti_hero_bottom;
    public CardComponent helper1 => helpers[0];
    public CardComponent helper2 => helpers[1];
    public CardComponent helper3 => helpers[2];

    public bool _is_top = false;
    [Export]
    private bool is_top {
        get => _is_top; set {
            try { // needed because this is called before editor sets the below variables
                _is_top = value;
                anti_hero_top.Visible = _is_top;
                anti_hero_bottom.Visible = !_is_top;
            } catch { }
        }
    }

    [Signal]
    public delegate void cardClickedEventHandler(bool double_click, int card_index, bool player_card);

    public CardComponent this[int i] {
        get {
            if (i == -1) {
                return this.anti_hero;
            }
            return this.helpers[i];
        }
    }

    public void Reset() {
        this.anti_hero_top.SetCard(null);
        this.anti_hero_bottom.SetCard(null);
        foreach (var cc in this.helpers) {
            cc.SetCard(null);
        }
    }

    public override void _Ready() {
        anti_hero_top.Visible = is_top;
        anti_hero_bottom.Visible = !is_top;
        this.anti_hero.GuiInput += (e) => onCardInput(e, -1);
        for (int i = 0; i < helpers.Count; ++i) {
            var _i = i;
            helpers[i].GuiInput += (e) => onCardInput(e, _i);
        }
    }

    public void onMouseHover(InputEvent @event, int card_index) {
        //hover over a collisionshape2d, make card bigger

    }

    public void onCardInput(InputEvent @event, int card_index) {
        if (@event is InputEventMouseButton button && !button.IsEcho() && button.Pressed) {
            EmitSignalcardClicked(button.DoubleClick, card_index, is_top);
        }
    }

}
