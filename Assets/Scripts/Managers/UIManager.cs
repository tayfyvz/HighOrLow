using UnityEngine;
using Utils.AddressableLoaders.Utils;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
    
        public UIAtlasLoader UIAtlasLoader { get; private set; }

        private void Awake()
        {
            // Implement singleton pattern.
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

            // Create the UIAtlasLoader instance.
            UIAtlasLoader = new UIAtlasLoader();
            // Note: We are deferring the actual loading of the atlas until GameInitializer calls it.
        }
    }
}
