(function() {

    angular.module("umbraco").controller("test.controller", [
        "$scope",
        function(scope) {
            scope.setting = {
                value: "x = 5"
            };

            scope.something = "xyz";

            scope.changed = function() {
                
            }
        }
    ]);

}());