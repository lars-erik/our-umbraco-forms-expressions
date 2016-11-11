(function() {
    angular.module("ufx").controller("ufx.program.controller", [
        "$scope",
        function (scope) {

            scope.toggleFullScreen = function() {
                scope.fullScreen = true;
            }

            scope.closeFullScreen = function() {
                scope.fullScreen = false;
                scope.$digest();
            }

            function changed(args) {
                var editor = args[1],
                    lines = editor.session.getLength();

            }

            scope.fullScreen = false;

            scope.aceOpts = {
                mode: "forms",
                theme: "razor-chrome",
                onChange: changed
            }

        }
    ]);
}());