using Godot;
using System;

public partial class InteractBattle : Control {
    [Export]
    public InteractBattle interactBattle {
        get; set;
    }
    [Export] private CharacterBody2D playersprite;
    [Export] private CharacterBody2D enemysprite;





}