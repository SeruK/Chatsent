// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using IMVU.JSON;
using System.Linq;

namespace IMVU {
    public class HttpWebGLResponse: IHttpResponse {
        public uint status{ get; private set; }
        public string error { get; private set;}
        public Uri url { get; private set; }
        public Dictionary<string, List<string>> headers { get; private set; }


        public byte[] bodyBytes { get; private set;}
        public string bodyText { get { return System.Text.Encoding.UTF8.GetString(this.bodyBytes); }}

        private Texture2D _bodyTexture; 
        public Texture2D bodyTexture { 
            get {
                if(!this._bodyTexture) {
                    this._bodyTexture = new Texture2D(1,1);
                    this._bodyTexture.LoadImage(this.bodyBytes);
                }
                return this._bodyTexture;
            }
        }

        public HttpWebGLResponse(int requestId, Uri uri) {
            this.url = uri;
            this.headers = ParseRawHeaderString(ResponseHeaders(requestId));

            this.bodyBytes = new byte[ResponseSize(requestId)];
            var pinnedBuffer = GCHandle.Alloc(this.bodyBytes, GCHandleType.Pinned);
            Response(requestId, pinnedBuffer.AddrOfPinnedObject(), this.bodyBytes.Length);
            pinnedBuffer.Free();
            
            this.error = Error(requestId);
            this.error = this.error == "" ? null : this.error;
            this.status = Status(requestId);
        }
        
        private Dictionary<string, List<string>> ParseRawHeaderString(string headersString) {
            Dictionary<string, List<string>> localHeaders = new Dictionary<string, List<string>>();
            foreach(var header in headersString.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)) {
                List<string> parts = header.Split(new char[] { ':' }, 2).Select( s => s.Trim()).ToList();
                if (parts.Count == 2) {
                    string key = parts[0].ToLower();
                    List<string> singleHeader;
                    singleHeader = localHeaders.TryGetValue(key, out singleHeader) ? singleHeader : new List<string>();
                    singleHeader.Add(parts[1]);
                    localHeaders[key] = singleHeader;
               }
            }

            return localHeaders;
        }
 #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string ResponseHeaders(int requestId);

        [DllImport("__Internal")]
        private static extern void Response(int requestId, IntPtr buffer, int size);

        [DllImport("__Internal")]
        private static extern int ResponseSize(int requestId);

        [DllImport("__Internal")]
        private static extern uint Status(int requestId);
        
        [DllImport("__Internal")]
        private static extern string Error(int requestId);
#else
        private static string ResponseHeaders(int requestId) {
            return "";
        }
        private static void Response(int requestId, IntPtr buffer, int size) {
        }
        private static int ResponseSize(int requestId) {
            return 0;
        }
        private static uint Status(int requestId) {
            return 0;
        }
        private static string Error(int requestId) {
            return "";
        }
#endif    
    }

    public class HttpWebGL : IHttp {
        public Promise<IHttpResponse, Error> Get(Uri url, HttpOptions options = null) {
            return Request("GET", url, new Dictionary<string, string>(), null, options);
        }

        public Promise<IHttpResponse, Error> Get(Uri url, Dictionary<string, string> headers, HttpOptions options = null) {
            return Request("GET", url, headers, null, options);
        }

        public Promise<IHttpResponse, Error> Post(Uri url, byte[] postData, HttpOptions options = null) {
            return Request("POST", url, new Dictionary<string, string>(), postData, options);
        }

        public Promise<IHttpResponse, Error> Post(Uri url, byte[] postData, Dictionary<string, string> headers, HttpOptions options = null) {
            return Request("POST", url, headers, postData, options);
        }

        private Promise<IHttpResponse, Error> Request(string method, Uri url, Dictionary<string, string> headers, byte[] postData = null, HttpOptions options = null) {
            options = options == null ? new HttpOptions() : options;
            var promise = new Promise<IHttpResponse, Error>();
            var headersStr = ObjectParser.Save(headers);
            var optionsStr = ObjectParser.Save(options);
            var requestId = 0;
            var callbackInfo = ServiceProvider.Get().externalCallback.Make(
                (data) => promise.Accept(new HttpWebGLResponse(requestId, url))
            );
            if (method == "GET") {
                requestId = MakeGetRequest(url.AbsoluteUri, headersStr, optionsStr, callbackInfo);
            } else if(method == "POST") {
                var pinnedBuffer = GCHandle.Alloc(postData, GCHandleType.Pinned);
                requestId = MakePostRequest(url.AbsoluteUri, headersStr, pinnedBuffer.AddrOfPinnedObject(), postData.Length, optionsStr, callbackInfo);
                pinnedBuffer.Free();
            }
            return promise;
        }
        
#if UNITY_WEBGL && !UNITY_EDITOR
         [DllImport("__Internal")]
        private static extern int MakeGetRequest(string url, string headers, string options, string callbackInfo);

        [DllImport("__Internal")]
        private static extern int MakePostRequest(string url, string headers, IntPtr postData, int size, string options, string callbackInfo);
#else
        private static int MakeGetRequest(string url, string headers, string options, string callbackInfo) {
            return 0;
        }
        private static int MakePostRequest(string url, string headers, IntPtr postData, int size, string options, string callbackInfo) {
            return 0;
        }
#endif
    }
}
