angular.module("ufx").directive("ufxFieldAnimation", [
    function () {
        var speed = 250,
            borders = [
                "borderTopWidth",
                "borderRightWidth",
                "borderBottomWidth",
                "borderLeftWidth",
                "borderTopColor",
                "borderRightColor",
                "borderBottomColor",
                "borderLeftColor",
                "borderTopStyle",
                "borderRightStyle",
                "borderBottomStyle",
                "borderLeftStyle"
            ];
        return {
            restrict: "A",
            link: function (scope, elm, attrs) {
                var css = {}, i, hl, orig;
                for (i = 0; i < borders.length; i++) {
                    css[borders[i]] = elm.css(borders[i]);
                }
                hl = $.extend({ backgroundColor: "rgb(90, 255, 90)" }, css);
                orig = $.extend({ backgroundColor: "rgb(255, 255, 255)" }, css);
                scope.$on("ufx.field.set", function(evt, updatedField) {
                    if (updatedField === attrs.ufxFieldAnimation) {
                        elm.animate(hl, speed, "easeOutCubic", function() {
                            elm.animate(orig, speed, "easeOutCubic", function() {
                                elm.css(css);
                            });
                        });
                    }
                });
            }
        };
    }]);