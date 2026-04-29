using Godot;

public partial class MouseController : PanelContainer  // match your card's root node type
{
    private static readonly Vector2 HoverScale = new Vector2(1.15f, 1.15f);
    private static readonly Vector2 NormalScale = Vector2.One;
    private const float TweenDuration = 0.15f;

    public override void _Ready() {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        MouseFilter = MouseFilterEnum.Stop;

        // Wait one frame so size is calculated before setting pivot
        ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame).OnCompleted(() => {
            PivotOffset = Size / 2;
        });
    }

    private void OnMouseEntered() {
        ZIndex = 1;
        ScaleCard(HoverScale);
    }

    private void OnMouseExited() {
        ZIndex = 0;
        ScaleCard(NormalScale);
    }

    private void ScaleCard(Vector2 targetScale) {
        Tween tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out);
        tween.SetTrans(Tween.TransitionType.Back);
        tween.TweenProperty(this, "scale", targetScale, TweenDuration);
    }
}