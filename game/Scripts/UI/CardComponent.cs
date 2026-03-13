using Godot;
using System;

public partial class CardComponent : Control {

	[Export]
	public Card card { get; set; } = ResourceLoader.Load<Card>("res://Resources/DefaultCard.tres");

	[Export] private AnimatedSprite2D _sprite;
	[Export] private RichTextLabel _nameLabel;
	[Export] private RichTextLabel _descriptionLabel;

	public override void _Ready() {
		UpdateCard();
	}

	public void SetCard(Card newCard) {
		card = newCard;
		UpdateCard();
	}

	private void UpdateCard() {
		if (card == null)
			return;

		if (_nameLabel != null)
			_nameLabel.Text = $"[center]{card.name}[/center]";

		if (_descriptionLabel != null)
			_descriptionLabel.Text = $"[center]{card.description}[/center]";

		if (_sprite != null && card.frames != null) {
			_sprite.SpriteFrames = card.frames;
			_sprite.Play("default");
		}
	}
}