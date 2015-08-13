﻿// variable used for cross site CSOM calls
var context;

//Wait for the page to load
$(document).ready(function () {

    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    //load all appropriate scripts for the page to function
    $.getScript(layoutsRoot + 'SP.Runtime.js',
        function () {
            $.getScript(layoutsRoot + 'SP.js',
                function () {
                    //Load the SP.UI.Controls.js file to render the App Chrome
                    $.getScript(layoutsRoot + 'SP.UI.Controls.js', renderSPChrome);

                    //load scripts for cross site calls (needed to use the people picker control in an IFrame)
                    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
                        context = new SP.ClientContext(appWebUrl);
                        var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                        context.set_webRequestExecutorFactory(factory);
                    });

                    //load scripts for calling taxonomy APIs
                    $.getScript(layoutsRoot + 'init.js',
                        function () {
                            $.getScript(layoutsRoot + 'sp.taxonomy.js',
                                function () {
                                    //termset used for dependant selection
                                    var termId = "9df7c69b-267c-4b8b-ab3c-ac5c15cbbfae";

                                    //bind the taxonomy picker to the default keywords termset
                                    $('#taxPickerKeywords').taxpicker({ isMulti: true, allowFillIn: true, useKeywords: true }, context);

                                    //bind taxpickers that depend on eachothers choices
                                    $('#taxPickerContinent').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: termId, levelToShowTerms: 1 }, context, function () {
                                        $('#taxPickerCountry').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: termId, filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 2 }, context, function () {
                                            $('#taxPickerRegion').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: termId, filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 3 }, context);
                                        });
                                    });

                                    taxPickerIndex["#taxPickerContinent"] = 0;
                                    taxPickerIndex["#taxPickerCountry"] = 4;
                                    taxPickerIndex["#taxPickerRegion"] = 5;
                                });
                        });
                });
        });
});


//function to get a parameter value by a specific key
function getQueryStringParameter(urlParameterKey) {
    var params = document.URL.split('?')[1].split('&');
    var strParams = '';
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split('=');
        if (singleParam[0] == urlParameterKey)
            return singleParam[1];
    }
}