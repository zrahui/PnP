﻿var $pnpcore = {
    onStartPromise: null,
    getContextPromise: null,
    spContext: null,
    scriptbasepath: '/_layouts/15/',

    // gets the Add-In host web url
    getHostWebUrl: function () {
        return $pnpcore.getUrlParamByName('SPHostUrl');
    },

    // gets the Add-In web url
    getAddInWebUrl: function () {
        return $pnpcore.getUrlParamByName('SPAppWebUrl');
    },

    // a custom onStart method to ensure anything we need in our Add-In is ready
    // includes the full DOM load as resolved by $(function() {...}) syntax
    onStart: function (/*function()*/ onStartFunc) {
        var hostUrl = $pnpcore.getHostWebUrl();
        var scriptbase = hostUrl + $pnpcore.scriptbasepath;

        if ($pnpcore.onStartPromise === null) {
            // these scripts will be loaded ahead of other files and scripts
            var fileUrls = ['SP.Runtime.js', 'SP.js', 'SP.RequestExecutor.js'].select(function (s) {
                return scriptbase + s;
            });

            $pnpcore.onStartPromise = $.when($pnpcore.loadFiles(fileUrls), $.Deferred(function (d) { $(d.resolve); }).then(function () { $pnpcore.ensureContextQueryString(); })).promise();

            // our default post-onstart actions.
            $pnpcore.onStartPromise.then(function () {
                $pnpcore.extendSPClientContext();
            });
        }

        // if a function was supplied we register it as a done handler
        if ($.isFunction(onStartFunc)) {
            $pnpcore.onStartPromise.done(onStartFunc);
        }

        // return the promise so the onStart().done() syntax works
        return $pnpcore.onStartPromise;
    },

    // provides an easy way to use a client context from a centralized location
    // also ensures one context is created for all functions in your Add-In
    withSPContext: function (/*function(context)*/ action) {
        if ($pnpcore.getContextPromise === null) {
            $pnpcore.getContextPromise = $.Deferred(function (def) {
                // use custom onstart to ensure we have the files loaded and are ready to do stuff
                $pnpcore.onStart(function () {                    
                    try {
                        
                        var addinWebUrl = $pnpcore.getAddInWebUrl();
                        $pnpcore.spContext = new SP.ClientContext(addinWebUrl);
                        $pnpcore.spContext.set_webRequestExecutorFactory(new SP.ProxyWebRequestExecutorFactory(addinWebUrl));

                        def.resolveWith($pnpcore.spContext, [$pnpcore.spContext]);
                    }
                    catch (e) {
                        
                        // if we have a problem just reject with the associated error
                        def.rejectWith(e, [e]);
                    }
                });                
            }).promise();
        }

        // if a function was passed in helpfully register it
        if ($.isFunction(action)) {
            $pnpcore.getContextPromise.done(action);
        }

        // return the promise to support withSPContext().done().fail() syntax
        return $pnpcore.getContextPromise;
    },

    // there are lots of ways to extend the SP.ClientContext object
    // it could be done in a file (see arrayextensions.js for an example of this)
    // if could be done by passing a context to a method
    // here we show using an explicit call and the jQuery $.extend method
    // the benefit of this approach is that you can call it at any time to ensure the extended functionality is present
    // another option would be to pass a ClientContext instance to this method instead of extending prototype directly
    extendSPClientContext: function () {
        $.extend(SP.ClientContext.prototype, {
            /**
             * Executes the current pending request asynchronously on the server using a jQuery promise.
             * @param {object} context - Context passed to the doneCallbacks as the this object.
             * @returns {jquery.promise} Promise for the request.
             */
            executeQueryPromise: function (context) {                
                var self = this, // Maintain self-awareness.
                    def = $.Deferred(); // Create a deferred.

                // Resolve/reject with whatever would have been supplied by executeQueryAsync.
                self.executeQueryAsync(
                    function () { 
                        if (!context) {
                            context = this;
                        }
                        def.resolveWith(context, arguments); 
                    }, 
                    function () { 
                        def.rejectWith(this, arguments); 
                    });

                // Return a promise.
                return def.promise();
            },

            /**
             * Executes the current pending request asynchronously on the server using a jQuery promise
             * and retry if a failure occurs.
             * @param {object} context - Context passed to the doneCallbacks as the this object.
             * @returns {jquery.promise} Promise for the request.
             */
            executeQueryRetry: function (context) {
                var self = this, // Maintain self-awareness.
                    def = $.Deferred(), // Create a deferred.
                    // Create a context to track our async retries.
                    retryContext = {
                        // Used to track how many times we have tried.
                        retryAttempts: 0,
                        // The max number of times to retry the operation.
                        retryCount: 5,
                        // The starting delay in ms, increased with each retry.
                        delay: 100,
                        // Hold a ref to the deferred object.
                        deferred: def,
                        // Track all the errors as we retry.
                        errors: []
                    };

                // Call the implementation method to start the cycle maintaining the 'self' c
                $pnpcore.__executeQueryRetryImpl.apply(self, [context, retryContext]);

                // Return a promise.
                return def.promise();
            }
        });
    },

    // implements the retry logic
    __executeQueryRetryImpl: function (context, retryContext) {        
        var self = this; // Maintain self-awareness.

        // Call our extension method to execute query with a promise.
        self.ext_executeQueryPromise()
            .done(function () { 
                retryContext.deferred.resolveWith(context, arguments); 
            })
            .fail(function (sender, error) {
                var retry = false,
                    delay = retryContext.delay; // Grab our current delay value.

                // Record our error.
                retryContext.errors.push(error);

                // See if we should retry, other logic can be added here.
                if (error && error.get_errorTypeName) {
                    retry = /Microsoft.SharePoint.Client.ClientServiceTimeoutException/i.test(error.get_errorTypeName());
                }

                if (!retry) {
                    // If we can't retry, reject.
                    retryContext.deferred.rejectWith(
                        sender, 
                        [sender, $pnpcore.__getNoRetryAvailableException(retryContext)]);
                }

                // Increment our counters.
                retryContext.delay *= 2;
                retryContext.retryAttempts++;

                // If we have exceeded the retry count, reject.
                if (retryContext.retryCount <= retryContext.retryAttempts) {
                    retryContext.deferred.rejectWith(
                        sender, 
                        [sender, $pnpcore.__getRetryLimitReachedException(retryContext)]);
                }

                // Set our retry timeout for {delay} milliseconds.
                setTimeout($pnpcore.getCtxCallback(self, $pnpcore.__executeQueryRetryImpl, retryContext), delay);
            });
    },

    __getNoRetryAvailableException: function (retryContext) {
        return {
            message: 'Retry not available.',
            retryCount: retryContext.retryAttempts,
            errors: retryContext.errors,
            get_message: function () { return this.message; }
        };
    },

    __getRetryLimitReachedException: function (retryContext) {
        return {
            message: 'Exceeded retry limit.',
            retryCount: retryContext.retryAttempts,
            errors: retryContext.errors,
            get_message: function () { return this.message; }
        };
    },

    // allows for the calling pattern getCtxCallback(thisobj, method, methodarg1, methodarg2, etc...)
    getCtxCallback: function (context, method) {
        var args = [].slice.call(arguments).slice(2);

        return function () {
            method.apply(context, args);
        };
    },

    // loads a set of specificed files, returning a promise
    loadFiles: function (/*string[]*/ files) {
        // create a promise
        var promise = $.Deferred();

        // see if we have bad data supplied
        if (typeof (files) === undefined || ($.isArray(files) && files.length < 1)) {
            promise.resolve();
            return promise.promise();
        }

        // if it isn't an array stick it in one, this allows the calling pattern loadFiles('filename.js') with a singular filename
        if (!$.isArray(files)) {
            files = [files];
        }

        // this function will be used to recursively load all the files
        var engine = function () {            
            var self = this, // Maintain context.
                file = self.files.shift(); // get the next file to load.

            // load the remote script file
            $.getScript(file).done(function () {
                if (self.files.length > 0) {
                    engine.call(self);
                }
                else {
                    self.promise.resolve();
                }
            }).fail(self.promise.reject);
        };

        // create our "this" we will apply to the engine function
        var ctx = {
            files: files,
            promise: promise
        };

        // call the engine with our context
        engine.call(ctx);

        // give back the promise
        return promise.promise();
    },

    // tests if a url param exists
    urlParamExists: function (name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        return regex.test(location.search);
    },

    // gets a url param value by name
    getUrlParamByName: function (name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        var results = regex.exec(location.search);
        return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    },

    // gets a url param by name and attempts to parse a bool value
    getUrlParamBoolByName: function (name) {
        var p = $pnpcore.getUrlParamByName(name);
        var isFalse = (p === '' || /[false|0]/i.test(p));
        return !isFalse;
    },

    // inserts the string s into the string target as the index specified by index
    stringInsert: function (target, index, s) {
        if (index > 0) {
            return target.substring(0, index) + s + target.substring(index, target.length);
        }
        return s + target;
    },

    // appends the required SP parameters to the supplied url, great for web api calls to maintain context on the server side
    appendSPQueryToUrl: function (/*string*/ url) {
        // we already have the SPHostUrl param, just give back the url
        if (url.indexOf('SPHostUrl=') > -1) {
            return url;
        }

        // add the required parameters
        url += url.indexOf('?') > -1 ? '&' : '?';
        url += 'SPHostUrl=' + encodeURIComponent($pnpcore.getUrlParamByName('SPHostUrl'));
        if ($pnpcore.urlParamExists('SPAppWebUrl')) {
            url += '&SPAppWebUrl=' + encodeURIComponent($pnpcore.getUrlParamByName('SPAppWebUrl'));
        }
        url += '&SPLanguage=' + encodeURIComponent($pnpcore.getUrlParamByName('SPLanguage'));
        url += '&SPClientTag=' + encodeURIComponent($pnpcore.getUrlParamByName('SPClientTag'));
        url += '&SPProductNumber=' + encodeURIComponent($pnpcore.getUrlParamByName('SPProductNumber'));

        return url;
    },

    // gets the authority from the supplied url
    getAuthorityFromUrl: function (/*string*/ url) {
        if (url) {
            var match = /^(?:https:\/\/|http:\/\/|\/\/)([^\/\?#]+)(?:\/|#|$|\?)/i.exec(url);
            if (match) {
                return match[1].toUpperCase();
            }
        }
        return null;
    },

    // ensures that all the appropriate links in the page have the SP parameters attached
    ensureContextQueryString: function () {
        // remove the redirect flag
        var SPHasRedirectedToSharePointParam = "&SPHasRedirectedToSharePoint=1";
        var queryString = window.location.search;
        if (queryString.indexOf(SPHasRedirectedToSharePointParam) >= 0) {
            window.location.search = queryString.replace(SPHasRedirectedToSharePointParam, "");
        }

        $pnpcore.ensureSPHostUrlInLinks($('a'));
    },

    // process all the supplied tags
    ensureSPHostUrlInLinks: function (/*jquery*/ parentNode) {
        var currentAuthority = $pnpcore.getAuthorityFromUrl(window.location.href);

        parentNode.filter(function () {
            var authority = $pnpcore.getAuthorityFromUrl(this.href);
            if (!authority && /^#|:/.test(this.href)) {
                // Filters out anchors and urls with other unsupported protocols.
                return false;
            }
            return authority !== null && authority.toUpperCase() === currentAuthority;
        })
        .each(function () {
            this.href = $pnpcore.appendSPQueryToUrl(this.href);
        });
    }
};