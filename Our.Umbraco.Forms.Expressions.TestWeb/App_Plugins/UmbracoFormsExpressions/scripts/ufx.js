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

(function () {

    angular.module("ufx").controller("ufx.program.controller", [
        "$scope",
        "$http",
        "umbRequestHelper",
        function (scope, http, requestHelper) {
            var urlKey = "ufx-program-evaluator",
                runUrl = requestHelper.getApiUrl(urlKey, "Run"),
                completers = {},
                langTools = ace.require("ace/ext/language_tools");

            completers.fieldCompleter = {
                getCompletions: function (editor, session, pos, prefix, callback) {
                    var tokenAt = session.getTokenAt(pos.row, pos.col),
                        isInside = tokenAt && tokenAt.type === "variable.parameter";
                    var allFields = _.uniq(
                            scope.fields.map(function(f) {
                                return f.name;
                            }).concat(scope.model.fields.map(function(f) {
                                return f.caption;
                            }))
                        ),
                        fields = allFields.map(function (f) {
                            return {
                                caption: f,
                                value: isInside ? f : "[" + f + "]",
                                meta: "field",
                                type: "variable.parameter",
                                score: 100
                            }
                        });
                    callback(null, fields);
                }
            };

            completers.variableCompleter = {
                getCompletions: function (editor, session, pos, prefix, callback) {
                    var variables = findTokens(editor, "variable.other"),
                        names = variables.map(function(v) {
                            return v.value;
                        }),
                        completions = _.uniq(names).map(function (name) {
                            return {
                                caption: name,
                                value: name,
                                meta: "variable",
                                type: "variable.other",
                                score: 100
                            }
                        });
                    callback(null, completions);
                }
            };

            completers.functionCompleter = {
                getCompletions: function (editor, session, pos, prefix, callback) {
                    var functions = session.getMode().$highlightRules.functions.map(function(f) {
                        return {
                            caption: f + "()",
                            value: f,
                            meta: "function",
                            type: "support.function"
                        }
                    });
                    callback(null, functions);
                }
            };

            function setupCompletion() {
                langTools.setCompleters([langTools.keyWordCompleter, completers.fieldCompleter, completers.variableCompleter, completers.functionCompleter]);
            }

            function fieldIsForToken(field, token) {
                return token.value.toLowerCase() === "[" + field.name.toLowerCase() + "]";
            }

            function findTokens(editor, tokenType) {
                var lines = editor.session.getLength(),
                    lineTokens,
                    tokens = [],
                    i,
                    j;

                tokenType = tokenType || "variable.parameter";

                for (i = 0; i < lines; i++) {
                    lineTokens = editor.session.getTokens(i);
                    for (j = 0; j < lineTokens.length; j++) {
                        if (lineTokens[j].type === tokenType) {
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

                // Value reset when digested for some reason. Leave it be...
                if (tokens.length === 0) {
                    return;
                }

                addNewTokens(tokens);
                removeUnusedTokens(tokens);

                setTimeout(function() {
                    scope.$digest();
                }, 50);
            }

            scope.run = function () {
                var values = {},
                    i;

                for (i = 0; i < scope.fields.length; i++) {
                    values[scope.fields[i].name] = scope.fields[i].value == null ? "" : scope.fields[i].value;
                }

                http.post(runUrl, {
                    Program: scope.setting.value,
                    Values: values
                }).then(function (response) {
                    var name, field;
                    scope.result = response.data;
                    scope.hasResult = response.data.Errors === null;

                    for (name in scope.result.SetFields) {
                        field = $.grep(scope.fields, function(n) {
                            return function (f) { return f.name.toLowerCase() === n.toLowerCase(); }
                        }(name))[0];
                        field.value = scope.result.SetFields[name];
                        scope.$broadcast("ufx.field.set", field.name);
                    }
                });
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
            scope.result = {};
            scope.hasResult = false;

            scope.populateFields = populateFields;

            scope.aceOpts = {
                mode: "forms",
                theme: "chrome",
                onChange: populateFields, 
                require:['ace/ext/language_tools'], 
                advanced: {
                    enableBasicAutocompletion: true,
                    enableLiveAutocompletion: true
                }
            }

            setupCompletion();
        }
    ]);
}());
define("ace/mode/forms_highlight_rules", ["require", "exports", "module", "ace/lib/oop", "ace/lib/lang", "ace/mode/text_highlight_rules"], function (require, exports, module) {
    "use strict";

    var oop = require("../lib/oop");
    var TextHighlightRules = require("./text_highlight_rules").TextHighlightRules;

    var FormsHighlightRules = function () {

        var keywords = (
            "if|else|end"
        );

        var builtinConstants = (
            "true|false"
        );

        var builtinFunctions = (
            "power|ceiling|floor|round|ifblank"
        );

        var dataTypes = (
            ".+|\\[.+\\]"
        );

        var keywordMapper = this.createKeywordMapper({
            "keyword": keywords,
            "constant.language": builtinConstants
        }, "variable.other", true);

        this.$rules = {
            "start": [{
                token: "constant.numeric", // float
                regex: "[+-]?\\d+(?:(?:\\.\\d*)?(?:[eE][+-]?\\d+)?)?\\b"
            }, {
                token: "variable.parameter",
                regex: "\\[[^\\]]*\\]"
            }, {
                token: "support.function",
                regex: builtinFunctions
            }, {
                token: keywordMapper,
                regex: "[a-zA-Z_$][a-zA-Z0-9_$]*\\b"
            }, {
                token: "variable.other",
                regex: "\w+"
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

        this.functions = builtinFunctions.split("|");
    };

    oop.inherits(FormsHighlightRules, TextHighlightRules);

    exports.FormsHighlightRules = FormsHighlightRules;
});

define("ace/mode/forms", ["require", "exports", "module", "ace/lib/oop", "ace/mode/text", "ace/mode/forms_highlight_rules", "ace/ext/language_tools"], function (require, exports, module) {

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