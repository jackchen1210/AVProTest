using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Core
{
    public class PlatformStateData
    {
        [TableColumnWidth(50), ValueDropdown("GetGlobalNames", IsUniqueList = true)]
        public string ViewState;
        [TableColumnWidth(50), ValueDropdown("GetAllSubNames", IsUniqueList = true)]
        public string SubKey;
#if UNITY_EDITOR

        private IEnumerable<ValueDropdownItem> GetGlobalNames()
        {
            return GlobalStateSetting.Instance.GlobalState.Keys.Select(x => new ValueDropdownItem(x, x));
        }
        private IEnumerable<ValueDropdownItem> GetAllSubNames()
        {
            return GlobalStateSetting.Instance.GlobalState[ViewState].Select(x => new ValueDropdownItem(x, x));
        }
#endif
        public PlatformStateData SetValue(string viewState, string subKey)
        {
            ViewState = viewState;
            SubKey = subKey;
            return this;
        }
        public override string ToString()
        {
            return $"{ViewState},{SubKey}";
        }

        public override bool Equals(object obj)
        {
            return obj is PlatformStateData data &&
                   ViewState == data.ViewState &&
                   SubKey == data.SubKey;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ViewState, SubKey);
        }

        public static bool operator ==(PlatformStateData a, PlatformStateData b)
        {
            bool result = false;
            if (a is not null && b is not null)
            {
                result = a.ViewState == b.ViewState && a.SubKey == b.SubKey;
            }
            return result;
        }

        public static bool operator !=(PlatformStateData a, PlatformStateData b)
        {
            return !(a == b);
        }
    }
}