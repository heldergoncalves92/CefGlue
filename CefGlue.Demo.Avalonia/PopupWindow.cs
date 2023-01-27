using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace ServiceStudio.WebViewImplementation.Framework {

    public class PopupWindow : Window {

        private Window parent;
        private ManagedPopupPositioner positioner;
        private PopupPositionerParameters positionerParameters;

        public PopupWindow() {
            ShowInTaskbar = false;
            SystemDecorations = SystemDecorations.BorderOnly;
            SizeToContent = SizeToContent.WidthAndHeight;
            VerticalContentAlignment = VerticalAlignment.Top;
            HorizontalContentAlignment = HorizontalAlignment.Left;
        }

        public void ConfigurePosition(IVisual target, Point offset, PopupAnchor anchor = PopupAnchor.TopLeft, PopupGravity gravity = PopupGravity.BottomRight) {
            var newParent = (Window)target.GetVisualRoot();

            if (newParent == null) {
                return;
            }

            if (parent != newParent) {
                parent = newParent;
                positioner = new ManagedPopupPositioner(new ManagedPopupPositionerPopupImplHelper(parent.PlatformImpl, MoveResizeDelegate));
            }

            positionerParameters = GetNewPositionerParametersRelativeToOffset(target, offset, anchor, gravity);

            if (positionerParameters.Size != default) {
                UpdatePosition();
            }
        }

        private PopupPositionerParameters GetNewPositionerParametersRelativeToOffset(IVisual target, Point offset, PopupAnchor anchor = PopupAnchor.TopLeft, PopupGravity gravity = PopupGravity.BottomRight) {
            var matrix = target.TransformToVisual(parent);
            if (matrix.HasValue) {
                var bounds = new Rect(default, target.Bounds.Size);
                var anchorRect = new Rect(offset, new Size(1, 1));

                var newpositionerParameters = new PopupPositionerParameters() {
                    ConstraintAdjustment = PopupPositionerConstraintAdjustment.All,
                    AnchorRectangle = anchorRect.Intersect(bounds).TransformToAABB(matrix.Value),
                    Anchor = anchor,
                    Gravity = gravity,
                    Size = positionerParameters.Size
                };

                return newpositionerParameters;
            }

            return positionerParameters;
        }

        private void MoveResizeDelegate(PixelPoint position, Size size, double scaling) {
            Position = position;
            Height = size.Height;
            Width = size.Width;
        }

        private void UpdatePosition() {
            positioner?.Update(positionerParameters);
        }

        protected override Size ArrangeOverride(Size finalSize) {
            positionerParameters.Size = finalSize;
            UpdatePosition();
            return base.ArrangeOverride(finalSize);
        }

        public void ShowPopup() {
            if (parent != null) {
                Show(parent);
            } else {
                Show();
            }
        }
    }
}
