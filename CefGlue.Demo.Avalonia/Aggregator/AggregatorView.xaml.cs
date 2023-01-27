using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ServiceStudio.WebViewImplementation.Framework;

namespace ServiceStudio.WebViewImplementation {
    internal partial class AggregatorView : UserControl {
        public AggregatorView() { }

        public AggregatorView(TabHeaderInfo tabHeaderInfo) {
            AvaloniaXamlLoader.Load(this);

            TabHeader = tabHeaderInfo;
        }

        public TabHeaderInfo TabHeader { get; }
    }
}
