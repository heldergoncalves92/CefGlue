using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Markup.Xaml;
using AvaloniaStyles = Avalonia.Styling.Styles;

namespace ServiceStudio.WebViewImplementation.Themes {
    internal class LightTheme : AvaloniaStyles, ITheme {

        public LightTheme() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
