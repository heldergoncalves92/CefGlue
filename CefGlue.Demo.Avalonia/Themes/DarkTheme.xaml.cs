using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Markup.Xaml;
using AvaloniaStyles = Avalonia.Styling.Styles;

namespace ServiceStudio.WebViewImplementation.Themes {
    internal class DarkTheme : AvaloniaStyles, ITheme {

        public DarkTheme() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
