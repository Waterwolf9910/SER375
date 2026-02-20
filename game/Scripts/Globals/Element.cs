public enum Element {
    None,
    /// <summary>
    /// Aelethia - Escapism
    /// </summary>
    Wind = 1 << 1,
    /// <summary>
    /// Nayuko - Nihilism
    /// </summary>
    Comsic = 1 << 2,
    /// <summary>
    /// Dei - Fear
    /// </summary>
    Dark = 1 << 3,
    /// <summary>
    /// Iris - Pride
    /// </summary>
    Fire = 1 << 4,
    /// <summary>
    /// Amos - Gloom
    /// </summary>
    Ice = 1 << 5,
    /// <summary>
    /// Akira - Physical
    /// </summary>
    Physical = 1 << 6
}

public partial class Utils {
    /// <summary>
    /// Gets the source's effectiveness against a target
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static double getElementEffectiveness(this Element source, Element target) {
        return source.isElementWeakTo(target) ? 0.5 : target.isElementWeakTo(source) ? 1.5 : 1;
    }

    /// <summary>
    /// Checks if the source element is weaker than the target
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool isElementWeakTo(this Element source, Element target) {
        switch (source) {
            case Element.Wind: {
                return target == Element.Comsic;
            }
            case Element.Comsic: {
                return target == Element.Fire;
            }
            case Element.Dark: {
                return target == Element.Wind;
            }
            case Element.Fire: {
                return target == Element.Ice;
            }
            case Element.Ice: {
                return target == Element.Dark;
            }
            default: {
                return false;
            }
        }
    }
}
