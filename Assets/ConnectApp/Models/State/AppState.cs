using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.UIWidgets.foundation;
using UnityEngine;

namespace ConnectApp.models {
    [Serializable]
    public class AppState {
        public int Count { get; set; }
        public LoginState loginState { get; set; }
        public ArticleState articleState { get; set; }
        public EventState eventState { get; set; }
        public PopularSearchState popularSearchState { get; set; }
        public SearchState searchState { get; set; }
        public NotificationState notificationState { get; set; }
        public UserState userState { get; set; }
        public TeamState teamState { get; set; }
        public PlaceState placeState { get; set; }
        public MineState mineState { get; set; }
        public MessageState messageState { get; set; }
        public SettingState settingState { get; set; }

        public static AppState initialState() {

            var searchHistory = PlayerPrefs.GetString("searchHistoryKey");
            var searchHistoryList = new List<string>();
            if (searchHistory.isNotEmpty())
                searchHistoryList = JsonConvert.DeserializeObject<List<string>>(searchHistory);
            
            var articleHistory = PlayerPrefs.GetString("articleHistoryKey");
            var articleHistoryList = new List<Article>();
            if (articleHistory.isNotEmpty())
                articleHistoryList = JsonConvert.DeserializeObject<List<Article>>(articleHistory);
            
            var eventHistory = PlayerPrefs.GetString("eventHistoryKey");
            var eventHistoryList = new List<IEvent>();
            if (eventHistory.isNotEmpty())
                eventHistoryList = JsonConvert.DeserializeObject<List<IEvent>>(eventHistory);
            
            return new AppState {
                Count = PlayerPrefs.GetInt("count", 0),
                loginState = new LoginState {
                    email = "",
                    password = "",
                    loginInfo = new LoginInfo(),
                    isLoggedIn = false,
                    loading = false
                },
                articleState = new ArticleState {
                    articleList = new List<string>(),
                    articleDict = new Dictionary<string, Article>(),
                    articlesLoading = false,
                    articleDetailLoading = false,
                    articleTotal = 0,
                    pageNumber = 1,
                    articleHistory = articleHistoryList
                },
                eventState = new EventState {
                    ongoingEvents = new List<string>(),
                    eventsDict = new Dictionary<string, IEvent>(),
                    ongoingEventTotal = 0,
                    completedEvents = new List<string>(),
                    completedEventTotal = 0,
                    pageNumber = 1,
                    completedPageNumber = 1,
                    eventsLoading = false,
                    eventHistory = eventHistoryList,
                    channelId = ""
                },
                popularSearchState = new PopularSearchState {
                    popularSearchs = new List<PopularSearch>()
                },
                searchState = new SearchState {
                    loading = false,
                    keyword = "",
                    searchArticles = new List<Article>(),
                    currentPage = 0,
                    pages = new List<int>(),
                    searchHistoryList = searchHistoryList,
                },
                notificationState = new NotificationState {
                    loading = false,
                    notifications = new List<Notification>()
                },
                userState = new UserState {
                    userDict = new Dictionary<string, User>()
                },
                teamState = new TeamState {
                    teamDict = new Dictionary<string, Team>()
                },
                placeState = new PlaceState {
                    placeDict = new Dictionary<string, Place>()
                },
                mineState = new MineState {
                    futureEventsList = new List<IEvent>(),
                    pastEventsList = new List<IEvent>(),
                    futureListLoading = false,
                    pastListLoading = false,
                    futureEventTotal = 0,
                    pastEventTotal = 0
                },
                messageState = new MessageState {
                    channelMessageDict = new Dictionary<string, Dictionary<string, Message>>(),
                    channelMessageList = new Dictionary<string, List<string>>()
                },
                settingState = new SettingState {
                    fetchReviewUrlLoading = false,
                    reviewUrl = ""
                }
            };
        }
    }
}