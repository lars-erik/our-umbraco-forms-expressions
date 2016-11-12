/// <reference path="../../../../../Umbraco/lib/angular/1.1.5/angular.js" />
/// <reference path="../../../../../Umbraco/lib/angular/1.1.5/angular-mocks.js" />
/// <reference path="umbraco-mocks.js" />
/// <reference path="../module.js" />
/// <reference path="../program-controller.js" />
(function () {

    describe("program controller", function() {

        var scope,
            controller,
            lineTokens,
            editor = {
                session: {
                    getLength: function () { return lineTokens.length; },
                    getTokens: function (line) {
                        return lineTokens[line];
                    }
                }
            };

        beforeEach(module("umbraco"));
        beforeEach(module("ufx"));
        beforeEach(inject([
            "$controller",
            "$rootScope",
            function (controllerFactory, rootScope) {
                scope = rootScope.$new();
                controller = controllerFactory("ufx.program.controller", {"$scope":scope});
            }
        ]));

        it("builds list of unique fields from tokens", function() {
            lineTokens = [[
                { type: "identifier", value: "x" },
                { type: "operator", value: "=" },
                { type: "support", value: "[field a]" }
                ], [
                { type: "text", value: "y" },
                { type: "operator", value: "=" },
                { type: "support", value: "[field b]" },
                { type: "operator", value: "+" },
                { type: "support", value: "[field a]" }
            ]];

            scope.aceOpts.onChange([null, editor]);

            expect(scope.fields).toEqual([
                {name:"field a", value:0},
                {name:"field b", value:0}
            ]);
        });

        it("removes unused tokens from fields", function() {
            scope.fields = [
                { name: "field a", value: 0 },
                { name: "field b", value: 0 }
            ];

            lineTokens = [[
                { type: "identifier", value: "x" },
                { type: "operator", value: "=" },
                { type: "support", value: "[field a]" }
            ]];

            scope.aceOpts.onChange([null, editor]);

            expect(scope.fields).toEqual([
                {name:"field a", value:0}
            ]);
        });

    });

}());
