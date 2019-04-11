using System;
using System.Collections.Generic;
using ConnectApp.api;
using ConnectApp.canvas;
using ConnectApp.components;
using ConnectApp.components.pull_to_refresh;
using ConnectApp.constants;
using ConnectApp.models;
using ConnectApp.Models.ActionModel;
using ConnectApp.Models.ViewModel;
using ConnectApp.redux;
using ConnectApp.redux.actions;
using RSG;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.scheduler;
using Unity.UIWidgets.widgets;

namespace ConnectApp.screens {
    public class NotificationScreenConnector : StatelessWidget {
        public override Widget build(BuildContext context) {
            return new StoreConnector<AppState, NotifcationScreenViewModel>(
                converter: (state) => new NotifcationScreenViewModel {
                    notifationLoading = state.notificationState.loading,
                    total = state.notificationState.total,
                    notifications = state.notificationState.notifications,
                    userDict = state.userState.userDict
                },
                builder: (context1, viewModel, dispatcher) => {
                    var actionModel = new NotificationScreenActionModel {
                        startFetchNotifications = () => dispatcher.dispatch(new StartFetchNotificationsAction()),
                        fetchNotifications = pageNumber =>
                            dispatcher.dispatch<IPromise>(Actions.fetchNotifications(pageNumber)),
                        pushToArticleDetail = id => dispatcher.dispatch(
                            new MainNavigatorPushToArticleDetailAction {
                                articleId = id
                            }
                        )
                    };
                    return new NotificationScreen(viewModel, actionModel);
                }
            );
        }
    }
    public class NotificationScreen : StatefulWidget {

        public NotificationScreen(
            NotifcationScreenViewModel viewModel = null,   
            NotificationScreenActionModel actionModel = null,   
            Key key = null
        ) : base(key)
        {
            this.viewModel = viewModel;
            this.actionModel = actionModel;
        }
        
        public readonly NotifcationScreenViewModel viewModel;
        public readonly NotificationScreenActionModel actionModel;

        public override State createState() {
            return new _NotificationScreenState();
        }
    }

    public class _NotificationScreenState : State<NotificationScreen> {
        private const float headerHeight = 140;
        private const int firstPageNumber = 1;
        private float _offsetY;
        private int _pageNumber = firstPageNumber;
        private RefreshController _refreshController;

//        protected override bool wantKeepAlive {
//            get => true;
//        }
        public override void initState() {
            base.initState();
            _offsetY = 0;
            _refreshController = new RefreshController();
            SchedulerBinding.instance.addPostFrameCallback(_ => {
                widget.actionModel.startFetchNotifications();
                widget.actionModel.fetchNotifications(_pageNumber);
            });
        }
        
        public override Widget build(BuildContext context)
        {
            object content = new Container();
            if (widget.viewModel.notifationLoading) 
                content = new GlobalLoading();
            else {
                if (widget.viewModel.notifications.Count <= 0) 
                    content = new BlankView("暂无通知消息");
                else {
                    var isLoadMore = widget.viewModel.notifications.Count == widget.viewModel.total;
                    content = new SmartRefresher(
                        controller: _refreshController,
                        enablePullDown: true,
                        enablePullUp: !isLoadMore,
                        headerBuilder: (cxt, mode) => new SmartRefreshHeader(mode),
                        footerBuilder: (cxt, mode) => new SmartRefreshHeader(mode),
                        onRefresh: _onRefresh,
                        child: ListView.builder(
                            physics: new AlwaysScrollableScrollPhysics(),
                            itemCount: widget.viewModel.notifications.Count,
                            itemBuilder: (cxt, index) => {
                                var notification = widget.viewModel.notifications[index];
                                var user = widget.viewModel.userDict[notification.userId];
                                return new NotificationCard(
                                    notification,
                                    user,
                                    onTap: () => widget.actionModel.pushToArticleDetail(notification.data.projectId),
                                    new ObjectKey(notification.id)
                                );
                            }
                        )
                    );
                }
            }
                
            return new Container(
                color: CColors.White,
                child: new Column(
                    children: new List<Widget> {
                        new CustomNavigationBar(
                            new Text(
                                "通知",
                                style: new TextStyle(
                                    height: 1.25f,
                                    fontSize: 32 / headerHeight * (headerHeight - _offsetY),
                                    fontFamily: "Roboto-Bold",
                                    color: CColors.TextTitle
                                )
                            ),
                            null,
                            CColors.White,
                            _offsetY
                        ),
                        new CustomDivider(
                            color: CColors.Separator2,
                            height: 1
                        ),
                        new Flexible(
                            child: new NotificationListener<ScrollNotification>(
                                onNotification: _onNotification,
                                child: (Widget)content
                            )
                        )
                    }
                )
            );
        }

        private bool _onNotification(ScrollNotification notification) {
//            var pixels = notification.metrics.pixels;
//            if (pixels >= 0) {
//                if (pixels <= headerHeight) setState(() => { _offsetY = pixels / 2.0f; });
//            }
//            else {
//                if (_offsetY != 0) setState(() => { _offsetY = 0; });
//            }
            return true;
        }

        private void _onRefresh(bool up) {
            if (up)
                _pageNumber = 1;
            else
                _pageNumber++;
            widget.actionModel.fetchNotifications(_pageNumber)
                .Then(() => _refreshController.sendBack(up, up ? RefreshStatus.completed : RefreshStatus.idle))
                .Catch(_ => _refreshController.sendBack(up, RefreshStatus.failed));
        }
    }
}