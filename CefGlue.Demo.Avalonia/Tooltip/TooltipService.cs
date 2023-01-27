using System;
using System.Collections.Concurrent;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace ServiceStudio.WebViewImplementation.Framework.Tooltip {
    internal class TooltipService : ITooltipService {
        private readonly object tooltipLock = new object();
        private readonly ConcurrentStack<CustomTooltip> tooltips = new ConcurrentStack<CustomTooltip>();
        
        /// <summary>
        /// This custom tooltip replaces the avalonia tooltip which does not play well with the browser on OSX.
        /// </summary>
        private class CustomTooltip {
            private readonly PopupWindow popup;

            public CustomTooltip(IControl content) {
                popup = new PopupWindow { ShowActivated = false, Content = content };
                popup.VerticalContentAlignment = VerticalAlignment.Center;
                popup.PointerEnter += delegate { Hide(); };
            }

            public void Show(IControl target, Window parentWindow, double x, double y) {
                popup.ConfigurePosition(target, new Point(x, y), anchor: PopupAnchor.TopLeft, gravity: PopupGravity.BottomRight);
                popup.Show(parentWindow);
            }

            public void Hide() {
                popup.Close();
            }
        }

        private readonly Func<ITooltip> tooltipContentProvider;
        private IDisposable showTooltipDelayedTask;
        private object tooltipData;

        public TooltipService(Func<ITooltip> tooltipProvider) {
            tooltipContentProvider = tooltipProvider;
        }

        

        public void ShowTooltip(Control target, string tooltipText, double x, double y, bool showDelayed = false) {
            if (!string.IsNullOrEmpty(tooltipText)) {
                void Show() => InnerShowTooltip(target, default, x, y);

                lock (tooltipLock) {
                    showTooltipDelayedTask?.Dispose();

                    if (showDelayed) {
                        var delay = TimeSpan.FromMilliseconds(ToolTip.GetShowDelay(target));
                        showTooltipDelayedTask = Dispatcher.UIThread.DelayedExecute(Show, delay);
                        return;
                    }
                }

                Dispatcher.UIThread.AsyncExecuteInUIThread(Show);
            }
        }

        public void HideTooltip() {
            lock (tooltipLock) {
                showTooltipDelayedTask?.Dispose();
                foreach (var customTooltip in tooltips) {
                    Dispatcher.UIThread.AsyncExecuteInUIThread(() => customTooltip.Hide());
                }
                tooltips.Clear();
            }
        }

        private void InnerShowTooltip(Control target, object data, double x, double y) {
            lock (tooltipLock) {
                HideTooltip();
            
                // only show tooltips when the window is active
                if (target.GetVisualRoot() is Window parentWindow && parentWindow.IsActive && parentWindow.IsVisible) {
                    const int VerticalDistanceFromPointer = 20;
                    tooltipData = data;

                    var tooltipView = tooltipContentProvider();
                    tooltipView.RefreshTooltipContent();
                    var tooltip = new CustomTooltip(tooltipView as UserControl);


                    tooltips.Push(tooltip);
                    tooltip.Show(target, parentWindow, x, y + VerticalDistanceFromPointer);
                }
            }
        }
    }
}
