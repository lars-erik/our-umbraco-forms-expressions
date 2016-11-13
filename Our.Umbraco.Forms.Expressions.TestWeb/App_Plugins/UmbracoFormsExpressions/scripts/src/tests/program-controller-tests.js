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
            httpMock,
            editor = {
                session: {
                    getLength: function () { return lineTokens.length; },
                    getTokens: function (line) {
                        return lineTokens[line];
                    }
                }
            },
            requestHelper = {
                getApiUrl: function () { return "/run" }
            };

        beforeEach(module("umbraco"));
        beforeEach(module("ufx"));
        beforeEach(inject([
            "$controller",
            "$rootScope",
            "$httpBackend",
            function (controllerFactory, rootScope, httpBackend) {
                scope = rootScope.$new();
                scope.setting = {
                    value: ""
                };
                controller = controllerFactory("ufx.program.controller", { "$scope": scope, "umbRequestHelper": requestHelper });
                httpMock = httpBackend;
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

        it("keeps values for unmodified fields", function() {
            scope.fields = [
                { name: "field a", value: 5 },
                { name: "field b", value: 0 }
            ];

            lineTokens = [[
                { type: "identifier", value: "x" },
                { type: "operator", value: "=" },
                { type: "support", value: "[field a]" }
            ]];

            scope.aceOpts.onChange([null, editor]);

            scope.$digest();

            expect(scope.fields).toEqual([
                {name:"field a", value:5}
            ]);
        });

        it("posts program and values to server for calculation", function() {
            var response = {};
            httpMock.expectPOST("/run", {
                Program: "x = [field a]",
                Values: { "field a": 5 }
            }).respond(200, response);

            scope.setting.value = "x = [field a]";

            scope.fields = [
                { name: "field a", value: 5 }
            ];

            scope.run();

            httpMock.flush();
            httpMock.verifyNoOutstandingExpectation();

            expect(scope.result).toBe(response);
        });

    });

}());
