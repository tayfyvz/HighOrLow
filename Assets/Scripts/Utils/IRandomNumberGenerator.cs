namespace Utils
{
    /// <summary>
    /// decouples core logic from Unity-specific dependencies
    /// and makes it easier to test or swap out different RNG implementations.
    /// </summary>
    public interface IRandomNumberGenerator
    {
        /// <summary>
        /// Returns a double between 0.0 (inclusive) and 1.0 (exclusive).
        /// </summary>
        double NextDouble();
    }
}