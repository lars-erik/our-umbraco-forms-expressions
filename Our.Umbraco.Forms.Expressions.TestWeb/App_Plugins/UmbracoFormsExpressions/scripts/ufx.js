angular.module("ufx", []);
angular.module("umbraco").requires.push("ufx");
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

(function() {
    angular.module("ufx").controller("ufx.program.controller", [
        "$scope",
        function (scope) {

            function fieldIsForToken(field, token) {
                return token.value.toLowerCase() === "[" + field.name.toLowerCase() + "]";
            }

            function findTokens(editor) {
                var lines = editor.session.getLength(),
                    lineTokens,
                    tokens = [],
                    i,
                    j;

                for (i = 0; i < lines; i++) {
                    lineTokens = editor.session.getTokens(i);
                    for (j = 0; j < lineTokens.length; j++) {
                        if (lineTokens[j].type === "support") {
                            tokens.push(lineTokens[j]);
                        }
                    }
                }

                return tokens;
            }

            function addNewTokens(tokens) {
                var i, j, fieldIndex;

                for (i = 0; i < tokens.length; i++) {
                    fieldIndex = -1;
                    for (j = 0; j < scope.fields.length; j++) {
                        if (fieldIsForToken(scope.fields[j], tokens[i])) {
                            fieldIndex = j;
                            break;
                        }
                    }
                    if (fieldIndex === -1) {
                        scope.fields.push({
                            name: tokens[i].value.substring(1, tokens[i].value.length - 1),
                            value: 0
                        });
                    }
                }
            }

            function removeUnusedTokens(tokens) {
                var i, j, tokenIndex;

                for (i = scope.fields.length - 1; i >= 0; i--) {
                    tokenIndex = -1;
                    for (j = 0; j < tokens.length; j++) {
                        if (fieldIsForToken(scope.fields[i], tokens[j])) {
                            tokenIndex = i;
                            break;
                        }
                    }
                    if (tokenIndex === -1) {
                        scope.fields.splice(i, 1);
                    }
                }
            }

            function populateFields(args) {
                var editor = args[1],
                    tokens = findTokens(editor);

                addNewTokens(tokens);
                removeUnusedTokens(tokens);
            }

            scope.toggleFullScreen = function () {
                scope.fullScreen = true;
            }

            scope.closeFullScreen = function() {
                scope.fullScreen = false;
                scope.$digest();
            }

            scope.fullScreen = false;
            scope.fields = [];

            scope.aceOpts = {
                mode: "forms",
                theme: "razor-chrome",
                onChange: populateFields
            }

        }
    ]);
}());
define("ace/mode/forms_highlight_rules", ["require", "exports", "module", "ace/lib/oop", "ace/lib/lang", "ace/mode/text_highlight_rules"], function (require, exports, module) {
    "use strict";

    var oop = require("../lib/oop");
    var TextHighlightRules = require("./text_highlight_rules").TextHighlightRules;

    var FormsHighlightRules = function () {

        var keywords = (
            ""
        );

        var builtinConstants = (
            ""
        );

        var builtinFunctions = (
            "ceiling|floor|abs"
        );

        var dataTypes = (
            ""
        );

        var keywordMapper = this.createKeywordMapper({
            "support.function": builtinFunctions
            //,
            //"keyword": keywords,
            //"constant.language": builtinConstants,
            //"storage.type": dataTypes
        }, "variable.other", true);

        this.$rules = {
            "start": [{
                token: "constant.numeric", // float
                regex: "[+-]?\\d+(?:(?:\\.\\d*)?(?:[eE][+-]?\\d+)?)?\\b"
            }, {
                token: "support",
                regex: "\\[[^\\\"\\]]*\\b\\]"
            }, {
                token: keywordMapper,
                regex: "[a-zA-Z_$][a-zA-Z0-9_$]*\\b"
            }, {
                token: "keyword.operator",
                regex: "\\+|\\-|\\/|\\*|="
            }, {
                token: "paren.lparen",
                regex: "[\\(]"
            }, {
                token: "paren.rparen",
                regex: "[\\)]"
            }, {
                token: "text",
                regex: "\\s+"
            }]
        };
        this.normalizeRules();
    };

    oop.inherits(FormsHighlightRules, TextHighlightRules);

    exports.FormsHighlightRules = FormsHighlightRules;
});

define("ace/mode/forms", ["require", "exports", "module", "ace/lib/oop", "ace/mode/text", "ace/mode/forms_highlight_rules"], function (require, exports, module) {

    var oop = require("../lib/oop");
    var TextMode = require("./text").Mode;
    var FormsHighlightRules = require("./forms_highlight_rules").FormsHighlightRules;

    var Mode = function () {
        this.HighlightRules = FormsHighlightRules;
    };
    oop.inherits(Mode, TextMode);

    (function () {
        this.lineCommentStart = "#";
        this.$id = "ace/mode/forms";

        this.getNextLineIndent = function (state, line, tab) {
            var indent = this.$getIndent(line);
            return indent;
        };
    }).call(Mode.prototype);

    exports.Mode = Mode;
});