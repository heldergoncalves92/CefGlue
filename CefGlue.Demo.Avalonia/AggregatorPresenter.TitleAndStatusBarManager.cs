namespace ServiceStudio.Presenter {
    partial class AggregatorPresenter {
        private class TitleAndStatusBarManager {
            private readonly AggregatorPresenter aggregator;


            public TitleAndStatusBarManager(AggregatorPresenter aggregator) {
                this.aggregator = aggregator;
            }

            /// <param name="publishSucceeded"> 
            /// Used when publish is completed in order to display a success/fail icon in the module tab 
            /// Should be null in all scenarios except when a publish was completed (sucessfully or with error)
            /// </param>
           
            public void RefreshTitleBarAndStatusBar(bool? publishSucceeded = null) {
                var caption = "Amazing App [HGO]";
                aggregator.View.Caption = caption;
            }
        }
    }
}
