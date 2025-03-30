using Models;

namespace Utils
{
    /// <summary>
    /// Observer for card draw events.
    /// </summary>
    public interface ICardObserver
    {
        void OnCardDrawn(Card card);
    }
}