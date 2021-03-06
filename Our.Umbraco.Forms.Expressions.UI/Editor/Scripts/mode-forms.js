﻿define("ace/mode/forms_highlight_rules", ["require", "exports", "module", "ace/lib/oop", "ace/lib/lang", "ace/mode/text_highlight_rules"], function (require, exports, module) {
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
