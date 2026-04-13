using Godot;
using System;

public partial class ModeMenu : PanelContainer {
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }

    public void DeckMenuPressed() {
        this.GetTree().ChangeSceneToFile("res://Scenes/DeckMenu.tscn");
    }
    public void CampaignPressed() {
        //this.GetTree().ChangeSceneToFile("res://Scenes/Campaign.tscn");
    }
    public void PvpPressed() {
        //this.GetTree().ChangeSceneToFile("res://Scenes/Pvp.tscn");
    }

    public void RoguelikePressed() {
        //this.GetTree().ChangeSceneToFile("res://Scenes/Roguelike.tscn");
    }
    public void OnBackPressed() {
        this.GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
    }

    public void OnSavesPressed() {

    }

    public void OnSettingsPressed() {

    }

    public void OnExitPressed() {
        this.QuitGame();
    }
}
