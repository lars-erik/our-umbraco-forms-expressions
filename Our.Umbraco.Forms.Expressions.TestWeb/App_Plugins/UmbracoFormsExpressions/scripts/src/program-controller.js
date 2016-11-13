(function() {
    angular.module("ufx").controller("ufx.program.controller", [
        "$scope",
        "$http",
        "umbRequestHelper",
        function (scope, http, requestHelper) {
            var urlKey = "ufx-program-evaluator",
                runUrl = requestHelper.getApiUrl(urlKey, "Run");

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

                // Value reset when digested for some reason. Leave it be...
                if (tokens.length === 0) {
                    return;
                }

                addNewTokens(tokens);
                removeUnusedTokens(tokens);
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
                }).then(function(response) {
                    scope.result = response.data;
                    scope.hasResult = response.data.Errors === null;
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

            scope.aceOpts = {
                mode: "forms",
                theme: "razor-chrome",
                onChange: populateFields
            }

        }
    ]);
}());