using System.Threading;
using Cysharp.Threading.Tasks;
using Models;
using UnityEngine;

namespace Views
{
    public interface IDeckView
    {
        void UpdateDeckCount(int count);
        void PlayDrawCardAmountAnim(int amount);
        UniTask PlayDistributeCardAnim(Card roundCard, Vector2 destination, CancellationToken cancellationToken);
        UniTask PlayFlipCardAnim();
        UniTask PlayDiscardCardsAnim();
    }
}