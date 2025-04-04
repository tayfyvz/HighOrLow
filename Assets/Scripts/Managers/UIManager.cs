using UnityEngine;
using Utils.AddressableLoaders;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
    
        public UIAtlasLoader UIAtlasLoader { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            UIAtlasLoader = new UIAtlasLoader();
        }
    }
}
