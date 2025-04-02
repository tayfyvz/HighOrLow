using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Views;

namespace Controllers
{
    public interface IGameController
    {
        UniTask PlayRoundAsync(CancellationToken ct);
        void ResetGame();
        void PassView(IView view);
        void PassView(IView[] views, Vector2[] positions);
        void InitializeBetSystem();
    }
}