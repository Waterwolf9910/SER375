public enum Selection {
    /// <summary>
    /// This card will affect the user
    /// </summary>
    Self,
    /// <summary>
    /// This card will affect a target
    /// </summary>
    Target,
    /// <summary>
    /// This card will affect multiple targets
    /// </summary>
    MultiTarget,
    /// <summary>
    /// This card will affect all targets
    /// </summary>
    AllTargets,
    /// <summary>
    /// This card will affect everyone
    /// </summary>
    Everyone,
    /// <summary>
    /// Only used for ICardActions
    /// </summary>
    Same
}
