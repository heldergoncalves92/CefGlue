using Avalonia.Controls;

namespace ServiceStudio.WebViewImplementation.Framework {
    internal interface IExtendedReactView : IControl, ISetLogicalParent {

        SizeToContent AdjustSizeToContent { get; set; }
    }
}
