using UnityEngine;
namespace Core
{
    public class SafeArea : MonoBehaviour
    {
        RectTransform Panel;
        public Rect LastSafeArea { private set; get; } = new Rect(0, 0, 0, 0);

        private Vector2 originAnchorMin;
        private Vector2 originAnchorMax;

        void Awake()
        {
            Panel = GetComponent<RectTransform>();
            originAnchorMin = Panel.anchorMin;
            originAnchorMax = Panel.anchorMax;
            NetUtility.CameraViewportChanged += RefreshByCameraViewChanged;
            NetUtility.referenceResolutionChanged += ResetBaseSize;
        }
        private void OnDestroy()
        {
            NetUtility.CameraViewportChanged -= RefreshByCameraViewChanged;
            NetUtility.referenceResolutionChanged -= ResetBaseSize;
        }
        private void ResetBaseSize(float x, float y)
        {
            Refresh();
        }
        private void Start()
        {
            Refresh();
        }

        //void Update() {
        //    //Refresh();
        //}

        public static Rect? UseOnlySafeArea { private set; get; } = null;
        Rect GetSafeArea(bool forceToSetOnlySafeArea) {
            if (forceToSetOnlySafeArea) {
                if (UseOnlySafeArea != null) {
                    if (UseOnlySafeArea.Value != Screen.safeArea) {
                        return Screen.safeArea;
                    } else{
                        return UseOnlySafeArea.Value;
                    }
                } else {
                    return Screen.safeArea;
                }
            } else {
                if (UseOnlySafeArea != null) {
                    return UseOnlySafeArea.Value;
                } else {
                    return Screen.safeArea;
                }
            }
        }

        public void RefreshByCameraViewChanged(){
            // CLog.PrintLogError($"RefreshByCameraViewChanged !!!", $"SafeArea[{name}]", "red");
            Refresh(true);
        }
        public void Refresh(bool refreshByCameraViewChanged = false)
        {
            Rect safeArea = GetSafeArea(refreshByCameraViewChanged);
            if (safeArea == LastSafeArea){
                return;
            }
            
            // if(NetUtility.SCREEN_LIST_TYPE == DirectionType.VERTICAL){
            if (safeArea.height / safeArea.width > NetUtility.MaxDirectionRatio)
            {
                var _center = safeArea.center;
                safeArea.width = safeArea.width;
                safeArea.height = safeArea.width * NetUtility.MaxDirectionRatio;
                safeArea.center = _center;
            }
            else if(safeArea.width / safeArea.height > NetUtility.MaxDirectionRatio_H)
            {
                var _center = safeArea.center;
                safeArea.height = safeArea.height;
                safeArea.width = safeArea.height;
                safeArea.center = _center;
            }
            //}else if(NetUtility.SCREEN_LIST_TYPE == DirectionType.HORIZONTAL){
            //}

            //安全區跟底層寬高比同方向才處理
            //if (Screen.width > Screen.height == safeArea.width > safeArea.height) {
                    ApplySafeArea(safeArea, refreshByCameraViewChanged);
            //}
        }
        void ApplySafeArea(Rect r, bool refreshByCameraViewChanged)
        {
            if (UseOnlySafeArea == null || refreshByCameraViewChanged) {
                UseOnlySafeArea = r;
            }
            LastSafeArea = r;

            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;


            CLog.PrintLog($"" +
                $"localPosition:[{Panel.name}]\n" +
                $"localPosition:[{Panel.localPosition}]\n" +
                $"SafeArea:[{LastSafeArea}]\n" +
                $"r.position:[{r.position}]\n" +
                $"TheSafeArea:[{UseOnlySafeArea.Value}]\n" +
                $"Screen:[{Screen.width},{Screen.height}]\n" +
                $"anchorMin:[{anchorMax}]\n" +
                $"anchorMin:[{anchorMin}]"
                , $"SafeArea[{name}]", "red");
        }

        public void ResetToOrigin()
        {
            Panel.anchorMin = originAnchorMin;
            Panel.anchorMax = originAnchorMax;
            LastSafeArea = new Rect(0, 0, 0, 0);
        }
    }
}