using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Utils.AddressableLoaders
{
    public abstract class BaseAddressableLoader<T>
    {
        protected AsyncOperationHandle handle;

        public abstract UniTask<T> LoadAsync();

        public abstract void Release();
    }
}