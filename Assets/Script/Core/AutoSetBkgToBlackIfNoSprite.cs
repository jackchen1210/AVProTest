using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    [RequireComponent(typeof(Image))]
    public class AutoSetBkgToBlackIfNoSprite : MonoBehaviour
    {

        private void Awake()
        {
            if(TryGetComponent<Image>(out var img))
            {
                if(img.sprite == null)
                {
                    img.color = Color.black;
                }
            }
        }
    }

}
