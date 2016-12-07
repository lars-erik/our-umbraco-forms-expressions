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
                    var fields = scope.fields.map(function(f) {
                        return {
                            caption: f.name,
                            value: isInside ? f.name : "[" + f.name + "]",
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
                    var variables = findTokens(editor, "variable.other");
                    var completions = variables.map(function (v) {
                        return {
                            caption: v.value,
                            value: v.value,
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

                scope.$digest();
            }

            scope.run = function () {
                var values = {},
                    i;

                for (i = 0; i < scope.fields.length; i++) {
                    values[scope.fields[i].name] = scope.fields[i].value;
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
                            return function (f) { return f.name === n; }
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
                theme: "razor-chrome",
                onChange: populateFields
            }

            setupCompletion();
        }
    ]);
}());