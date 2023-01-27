using System;
using Avalonia.Controls;

namespace ServiceStudio.WebViewImplementation.Framework.Tooltip {
    public static class TooltipServiceProvider {

        private static ITooltipService tooltipService;

        public static void CreateTooltipService(Func<ITooltip> tooltipProvider) {
            tooltipService = new TooltipService(tooltipProvider);
        }

        public static void ShowTooltip(Control target, string tooltipText, double x, double y, bool showDelayed = false) {
            tooltipService.ShowTooltip(target, tooltipText, x, y);
        }

        public static void HideTooltip() {
            tooltipService.HideTooltip();
        }
    }
}
