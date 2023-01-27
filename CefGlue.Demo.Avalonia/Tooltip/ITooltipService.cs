using Avalonia.Controls;
using ServiceStudio.View;

namespace ServiceStudio.WebViewImplementation.Framework.Tooltip {
    public interface ITooltipService {
        void ShowTooltip(Control target, string tooltipText, double x, double y, bool showDelayed = false);
        void HideTooltip();
    }
}
