using Godot;
using System;

public partial class InteractBattle : Control {
    [Export]
    public InteractBattle interactBattle {
        get; set;
    }
    [Export] private CharacterBody2D playersprite;
    [Export] private CharacterBody2D enemysprite;

    public void startBattle(CharacterBody2D playersprite, CharacterBody2D enemysprite) {
        this.playersprite = playersprite;
        this.enemysprite = enemysprite;

        if (Input.IsKeyPressed(Key.Space) && playersprite.GetGlobalPosition().DistanceTo(enemysprite.GetGlobalPosition()) < 5) {
            //Battle Start Function Here
            Console.WriteLine("Battle Started!");
        }
    }

}