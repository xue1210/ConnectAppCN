using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ConnectApp.constants;
using ConnectApp.models;
using ConnectApp.utils;
using Newtonsoft.Json;
using RSG;
using Unity.UIWidgets.async;
using Unity.UIWidgets.ui;
using UnityEngine;
using UnityEngine.Networking;

namespace ConnectApp.api {
    public static class LoginApi {
        public static IPromise<LoginInfo> LoginByEmail(string email, string password) {
            // We return a promise instantly and start the coroutine to do the real work
            var promise = new Promise<LoginInfo>();
            Window.instance.startCoroutine(_LoginByEmail(promise, email, password));
            return promise;
        }

        private static IEnumerator _LoginByEmail(IPendingPromise<LoginInfo> promise, string email, string password) {
            var para = new LoginParameter {
                email = email,
                password = password
            };
            var body = JsonConvert.SerializeObject(para);
            var request = HttpManager.initRequest(Config.apiAddress + "/auth/live/login", "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(body);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.isNetworkError) {
                // something went wrong
                promise.Reject(new Exception(request.error));
            }
            else if (request.responseCode != 200) {
                // or the response is not OK
                promise.Reject(new Exception(request.downloadHandler.text));
            }
            else {
                
                if (request.GetResponseHeaders().ContainsKey("SET-COOKIE")) {
                    var cookie = request.GetResponseHeaders()["SET-COOKIE"];
                    HttpManager.updateCookie(cookie);
                }
                    
                // Format output and resolve promise
                var json = request.downloadHandler.text;
                var loginInfo = JsonConvert.DeserializeObject<LoginInfo>(json);
                if (loginInfo != null)
                    promise.Resolve(loginInfo);
                else
                    promise.Reject(new Exception("No user under this username found!"));
            }
        }

        
        public static IPromise<LoginInfo> LoginByWechat(string code) {
            // We return a promise instantly and start the coroutine to do the real work
            var promise = new Promise<LoginInfo>();
            Window.instance.startCoroutine(_LoginByWechat(promise, code));
            return promise;
        }

        private static IEnumerator _LoginByWechat(IPendingPromise<LoginInfo> promise, string code) {
            var para = new WechatLoginParameter
            {
                code = code
            };
            var body = JsonConvert.SerializeObject(para);
            var request = HttpManager.initRequest(Config.apiAddress + "/auth/live/wechat", "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(body);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.Send();

            if (request.isNetworkError) {
                // something went wrong
                promise.Reject(new Exception(request.error));
            }
            else if (request.responseCode != 200) {
                // or the response is not OK
                promise.Reject(new Exception(request.downloadHandler.text));
            }
            else {
                if (request.GetResponseHeaders().ContainsKey("SET-COOKIE")) {
                    var cookie = request.GetResponseHeaders()["SET-COOKIE"];
                    HttpManager.updateCookie(cookie);
                }
                var json = request.downloadHandler.text;
                Debug.Log($"wechat login request {json}");
                var loginInfo = JsonConvert.DeserializeObject<LoginInfo>(json);
                if (loginInfo != null)
                    promise.Resolve(loginInfo);
                else
                    promise.Reject(new Exception("No user under this username found!"));
            }
        }
        
        public static IPromise<string> FetchCreateUnityIdUrl() {
            // We return a promise instantly and start the coroutine to do the real work
            var promise = new Promise<string>();
            Window.instance.startCoroutine(_FetchCreateUnityIdUrl(promise));
            return promise;
        }

        private static IEnumerator _FetchCreateUnityIdUrl(Promise<string> promise) {
            var request =
                HttpManager.GET(Config.apiAddress + "/api/authUrl?redirect_to=%2F&locale=zh_CN&is_reg=true");
            yield return request.SendWebRequest();
            if (request.isNetworkError) {
                // something went wrong
                promise.Reject(new Exception(request.error));
            }
            else if (request.responseCode != 200) {
                // or the response is not OK
                promise.Reject(new Exception(request.downloadHandler.text));
            }
            else {
                if (request.GetResponseHeaders().ContainsKey("SET-COOKIE")) {
                    var cookie = request.GetResponseHeaders()["SET-COOKIE"];
                    HttpManager.updateCookie(cookie);
                }
                // Format output and resolve promise
                var responseText = request.downloadHandler.text;
                var urlDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);
                if (urlDictionary != null)
                    promise.Resolve(urlDictionary["url"]);
                else
                    promise.Reject(new Exception("No user under this username found!"));
            }
        }
    }
}