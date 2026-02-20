
public interface ILingeringCardEffect : ICardAction {
    /// <summary>
    /// The amount of turns left for this effect (decrements each turn)
    /// </summary>
    public ushort turns_left {get; set;}
    
    /// <summary>
    /// Will this card last for the entire encounter
    /// </summary>
    public bool infinite {get; set;}
}
