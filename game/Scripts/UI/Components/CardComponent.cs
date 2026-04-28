using Godot;
using System;

[Tool]
public partial class CardComponent : Control {

    /// <summary>
    /// current animation frame
    /// </summary>
    private int current_frame = 0;

    /// <summary>
    /// Game frame count
    /// </summary>
    private double frame_count = 0;

    [Export]
    public Card? _card;
    public Card? card { get => _card; private set {
            _card = value;
        }
    }

    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Export] private RichTextLabel _name;
    [Export] private RichTextLabel _description;
    [Export] private RichTextLabel _defense;
    [Export] private RichTextLabel _health;
    [Export] private RichTextLabel _coins;
    [Export] private RichTextLabel _attack;
    [Export] private TextureRect _image;
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    // private bool _create_card = false;
    
    // #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    // public CardComponent() {
    // #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    //     if (this.card == null) {
    //         _create_card = true;
    //     }
    // }
    bool ready = false;

    public override void _Ready() {
        UpdateCard();
        // if (this._create_card) {
        //     try { // For some reason my editor keeps crashing from this initalized at the top
        //         this.card = (Card) ResourceLoader.Load<Card>("Resources/Cards/Pride/DragoJr.tres").DuplicateDeep(Resource.DeepDuplicateMode.All);
        //     } catch {}
        // }
        if (this.card != null) {
            this.card.Changed += UpdateCard;
        }
        ready = true;
    }

    public void SetCard(Card? newCard) {
        GD.PrintS("Card set", card);
        this.card = (Card?) newCard?.DuplicateDeep(Resource.DeepDuplicateMode.All);
        if (this.card != null && ready) {
            this.card.Changed += UpdateCard;
        }
        UpdateCard();
    }

    const string card_animation_name = "default";

    public override void _Process(double delta) {
        base._Process(delta);

        if (this.card == null) {
            if (this._image.Texture != null) {
                this._defense.Text = "";
                this._health.Text = "";
                this._attack.Text = "";
                this._name.Text = "";
                this._description.Text = "";
                this._coins.Text = "";
                this._image.Texture = null;
            }
            return;
        }

        this._defense.Text = $"Def: {this.card.current_defense}/{this.card.max_defense}";
        this._health.Text = $"HP: {this.card.hp}/{this.card.max_hp}";
        this._attack.Text = $"+Attack: {this.card.bonus_attack}";
        this.frame_count += delta;

        if ((this.frame_count * 30) >= (this.card.frames.GetAnimationSpeed(card_animation_name) * this.card.frames.GetFrameDuration(card_animation_name, this.current_frame))) {
        // if ((this.frame_count / 15) >= (this.card.frames.GetAnimationSpeed(card_animation_name) * this.card.frames.GetFrameDuration(card_animation_name, this.current_frame))) {
            // GD.PrintS(this.card.frames.GetAnimationSpeed(card_animation_name) * this.card.frames.GetFrameDuration(card_animation_name, this.current_frame))     ;
            this.frame_count = 0;
            this.current_frame++;
            if (this.current_frame >= this.card.frames.GetFrameCount(card_animation_name)) {
                this.current_frame = 0;
            }
        }
        // this.SetProcessInput(true);
        // this.SetProcessShortcutInput(true);
        // this.SetProcessUnhandledKeyInput(true);
        // GD.PrintS(this.IsProcessingInput(), this.IsProcessingUnhandledInput(), this.IsProcessingUnhandledKeyInput());
        this._image.Texture = this.card.frames.GetFrameTexture(card_animation_name, this.current_frame);
    }

    public override void _GuiInput(InputEvent @event) {
        base._GuiInput(@event);
    }

    private void UpdateCard() {
        if (card == null) {
            return;
        }

        this._name.Text = this.card.name;
        this._description.Text = this.card.description;
        this._coins.Text = $"Coins: {this.card.coinEffects.Count}";


        // if (_sprite != null && card.frames != null) {
        // 	_sprite.SpriteFrames = card.frames;
        // 	_sprite.Play("default");
        // }
    }
}
