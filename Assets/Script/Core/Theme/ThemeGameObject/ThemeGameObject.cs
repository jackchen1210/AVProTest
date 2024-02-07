using Core;
using UnityEngine;
using UniRx;

namespace Platform
{
    public partial class ThemeGameObject : MonoBehaviour
    {
        private void Awake()
        {
            NetUtility.PLATFORM_INFO_DATA.PlatformThemeType.Subscribe(x => Updatetheme(x)).AddTo(this);
        }

        private void Updatetheme(ThemeType theme)
        {
            themeState = theme;
            ResetSubItemActive();
        }
    }
}
