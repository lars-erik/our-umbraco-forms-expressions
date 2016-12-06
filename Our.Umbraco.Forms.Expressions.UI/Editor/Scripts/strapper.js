(function() {

    angular.module("ufx", []);

    angular.module("umbraco.directives", []);
    angular.module("umbraco", ["umbraco.directives", "ufx"]);
    angular.module("umbraco").service("assetsService", function() {});
    angular.module("umbraco").service("umbRequestHelper", function() {
        this.getApiUrl = function() {
            return "/api/formsexpressions/run";
        }
    });


}());