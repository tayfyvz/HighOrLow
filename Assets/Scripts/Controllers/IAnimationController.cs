using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Models;

namespace Controllers
{
    public interface IAnimationController
    {
        UniTask AnimateCardDistributionAsync(Card[] roundCards, CancellationToken cancellationToken);
        UniTask AnimateRoundWinningAsync(int winningIndex, CancellationToken cancellationToken);
        UniTask AnimateBetResultAsync(BetResult result, int winningPlayerIndex, CancellationToken cancellationToken);
        UniTask AnimateSessionWinningAsync(int winnerIndex, CancellationToken cancellationToken);
    }
}