(function () {
    "use strict";

   


    function FieldPickerDirective() {
        return {
            replace: true,
            require: ["^ngModel"],
            restrict: "E",
            template: "<select ng-options=\"field.id as field.caption for field in fields\"></select>",
            link: function (scope) {
                scope.fields = scope.model.fields;
            }
        }
    }

    angular.module("ufx").directive("ufxField", [FieldPickerDirective]);

}());
