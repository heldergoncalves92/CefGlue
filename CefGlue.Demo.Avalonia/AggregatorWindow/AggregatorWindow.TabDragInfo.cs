using System;
using Avalonia;
using Avalonia.Controls;

namespace ServiceStudio.WebViewImplementation {

    /// <summary>
    /// Aggregator Window code section defining the TabDragInfo helper class used in the drag drop of tab items
    /// </summary>
    partial class AggregatorWindow {
        /// <summary>
        /// TabDragInfo helper class
        /// </summary>
        /// <remarks>
        /// Represents a linked list of tab items
        /// </remarks>
        private class TabDragInfo {
            public readonly TabItem tabItem;
            public readonly int width;
            public readonly int halfWidth;

            public int index;
            public PixelPoint originPoint;

            private TabDragInfo tabBefore;
            private TabDragInfo tabAfter;

            public TabDragInfo(TabItem tabItemArg, int indexArg, PixelPoint originPointArg, int widthArg, TabDragInfo tabBeforeArg) {
                tabItem = tabItemArg;
                index = indexArg;
                originPoint = originPointArg;
                width = widthArg;
                halfWidth = width / 2;
                tabBefore = tabBeforeArg;
                if (!(tabBefore is null)) {
                    tabBefore.tabAfter = this;
                }
                tabAfter = null;
            }

            public int GetMinWidth() {
                // Move to first tab drag info
                var tab = this;
                while (!(tab.tabBefore is null)) {
                    tab = tab.tabBefore;
                }

                var minWidth = int.MaxValue;
                do {
                    minWidth = Math.Min(minWidth, tab.width);
                    tab = tab.tabAfter;
                } while (!(tab is null));
                return minWidth;
            }

            public bool AdjustPosition(int xOriginAdjusted, int tabOffset, out int originalIndex, out int moveToIndex) {
                // Determine if the tab being dragged has moved sufficiently close enough to the previous or next tab
                var isAdjusted = false;
                originalIndex = index;
                if (!(tabBefore is null) && (xOriginAdjusted < (tabBefore.originPoint.X + tabOffset))) {
                    // Switch this tab with the previous tab

                    // Move this tab's origin point to the tabBefore's origin point
                    originPoint = tabBefore.originPoint;
                    // Move the tabBefore's origin point by this tab's width
                    tabBefore.originPoint += new PixelPoint(width, 0);

                    // Swap indices
                    index = tabBefore.index; // --index;
                    tabBefore.index = originalIndex; // ++tabBefore.index;

                    // Swap tab links
                    SwapTabs(tabBefore, this);

                    isAdjusted = true;
                } else if (!(tabAfter is null) && ((xOriginAdjusted + width) > ((tabAfter.originPoint.X + tabAfter.width) - tabOffset))) {
                    // Switch this tab with the next tab

                    // Move the tab after's origin point to this tab's origin point
                    tabAfter.originPoint = originPoint;
                    // Move this tab's origin by the tabAfter's width
                    originPoint += new PixelPoint(tabAfter.width, 0);

                    // Swap indices
                    index = tabAfter.index; // ++index;
                    tabAfter.index = originalIndex; // --tabAfter.index;

                    // Swap tab links
                    SwapTabs(this, tabAfter);

                    isAdjusted = true;
                }
                moveToIndex = index;

                return isAdjusted;
            }

            private static void SwapTabs(TabDragInfo firstTab, TabDragInfo secondTab) {
                // firstTab's tabAfter gets secondTab's tabAfter
                firstTab.tabAfter = secondTab.tabAfter;
                // secondTab's tabBefore gets firstTab's tabBefore
                secondTab.tabBefore = firstTab.tabBefore;
                // firstTab's tabBefore is now secondTab 
                firstTab.tabBefore = secondTab;
                // secondTab's tabAfter is now firstTab
                secondTab.tabAfter = firstTab;
            }
        }
    }
}
