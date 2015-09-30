// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

var LibraryHttp = {
    $Internal: {
        currentIndex: 0,
        httpRequests: [],
        
        HttpRequest: function(url, method, headers, postData, options, callbackInfo) {
            var requestId = Internal.currentIndex++;
            var callback = function() {
                SendMessage(callbackInfo.gameObject, callbackInfo.functionName, JSON.stringify({id: callbackInfo.id, data: ""}));
                callbackInfo = null;
                delete Internal.httpRequests[requestId];
            };
            
            var instance = {
                xhr: new XMLHttpRequest(),
                error: null
            };
            
            var xhr = instance.xhr;
            xhr.withCredentials = options.useCredentials;
            xhr.onreadystatechange = function () {
                if (xhr.readyState === 4 && xhr.status !== 0) {
                    callback();
                }
            }.bind(this);
            
            var wrapError = function(error) {
                instance.error = error;
                callback();
            };
            xhr.onabort = wrapError.bind("ERROR_ABORT");
            xhr.onerror = wrapError.bind("ERROR_NETWORK");
            xhr.ontimeout = wrapError.bind("ERROR_TIMEOUT");
            
            xhr.open(method, url, true);
            Object.keys(headers).forEach(function(name) {
               xhr.setRequestHeader(name, headers[name]); 
            });
            xhr.responseType = "arraybuffer";
            xhr.send(postData);
    
            Internal.httpRequests[requestId] = instance;
            return requestId;
        },
            
         MakeRequest: function(url, method, headers, postData, options, callbackInfo) {
             return Internal.HttpRequest(url, method, headers, postData, options, callbackInfo);
         },
    
        MakeStringReturn: function(value) {
            if (value === null) {
                value = "";
            }
            var buffer = _malloc(value.length + 1);
            writeStringToMemory(value, buffer);
            return buffer;
        }
    },
     MakeGetRequest: function(url, headers, options, callbackInfo) {
        url = Pointer_stringify(url);
        headers = JSON.parse(Pointer_stringify(headers));
        options = JSON.parse(Pointer_stringify(options));
        callbackInfo = JSON.parse(Pointer_stringify(callbackInfo));

        return Internal.MakeRequest(url, "GET", headers, null, options, callbackInfo);
    },
    
     MakePostRequest: function(url, headers, postData, size, options, callbackInfo) {
        url = Pointer_stringify(url);
        headers = JSON.parse(Pointer_stringify(headers));
        postData = HEAPU32[postData>>2];
        postData = HEAPU8.buffer.slice(postData, postData+size);
        options = JSON.parse(Pointer_stringify(options));
        callbackInfo = JSON.parse(Pointer_stringify(callbackInfo));
        return Internal.MakeRequest(url, "POST", headers, postData, options, callbackInfo);
     },

    ResponseHeaders: function(requestId) {
        return Internal.MakeStringReturn(Internal.httpRequests[requestId].xhr.getAllResponseHeaders());
    },
    
    Response: function(requestId, responseData, size) {
        if (Internal.httpRequests[requestId].error) {
            return;
        }
        var response = Internal.httpRequests[requestId].xhr.response;
        var responseData = HEAPU32[responseData>>2];
        var byteArray = new Uint8Array(response);
        HEAPU8.set(byteArray, responseData);
    },
    
    ResponseSize: function(requestId) {
        if (Internal.httpRequests[requestId].error) {
            return 0;
        }
        return Internal.httpRequests[requestId].xhr.response.byteLength;
    },

    Status: function(requestId) {
        return Internal.httpRequests[requestId].xhr.status;
    },
    
    Error: function(requestId) {
        return Internal.MakeStringReturn(Internal.httpRequests[requestId].error);
    },
};

autoAddDeps(LibraryHttp, '$Internal');
mergeInto(LibraryManager.library, LibraryHttp);
