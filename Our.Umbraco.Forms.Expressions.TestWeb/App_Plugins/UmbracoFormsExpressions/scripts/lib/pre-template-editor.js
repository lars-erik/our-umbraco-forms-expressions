/// <reference path="C:\Users\bi-ahu\Source\Repos\our-umbraco-forms-expressions\Our.Umbraco.Forms.Expressions.UI\Editor/Scripts/mode-forms.js" />
(function () {
    'use strict';

    function calcAceEditorDirective(umbAceEditorConfig, assetsService) {

        if (angular.isUndefined(window.ace)) {
            //throw new Error('ui-ace need ace to work... (o rly?)');

           

        }

        /**
         * Sets editor options such as the wrapping mode or the syntax checker.
         *
         * The supported options are:
         *
         *   <ul>
         *     <li>showGutter</li>
         *     <li>useWrapMode</li>
         *     <li>onLoad</li>
         *     <li>theme</li>
         *     <li>mode</li>
         *   </ul>
         *
         * @param acee
         * @param session ACE editor session
         * @param {object} opts Options to be set
         */
        var setOptions = function (acee, session, opts) {

            // sets the ace worker path, if running from concatenated
            // or minified source
            if (angular.isDefined(opts.workerPath)) {
                var config = window.ace.require('ace/config');
                config.set('workerPath', opts.workerPath);
            }

            // ace requires loading
            if (angular.isDefined(opts.require)) {
                opts.require.forEach(function (n) {
                    window.ace.require(n);
                });
            }

            // Boolean options
            if (angular.isDefined(opts.showGutter)) {
                acee.renderer.setShowGutter(opts.showGutter);
            }
            if (angular.isDefined(opts.useWrapMode)) {
                session.setUseWrapMode(opts.useWrapMode);
            }
            if (angular.isDefined(opts.showInvisibles)) {
                acee.renderer.setShowInvisibles(opts.showInvisibles);
            }
            if (angular.isDefined(opts.showIndentGuides)) {
                acee.renderer.setDisplayIndentGuides(opts.showIndentGuides);
            }
            if (angular.isDefined(opts.useSoftTabs)) {
                session.setUseSoftTabs(opts.useSoftTabs);
            }
            if (angular.isDefined(opts.showPrintMargin)) {
                acee.setShowPrintMargin(opts.showPrintMargin);
            }

            // commands
            if (angular.isDefined(opts.disableSearch) && opts.disableSearch) {
                acee.commands.addCommands([{
                    name: 'unfind',
                    bindKey: {
                        win: 'Ctrl-F',
                        mac: 'Command-F'
                    },
                    exec: function () {
                        return false;
                    },
                    readOnly: true
                }]);
            }

            // Basic options
            if (angular.isString(opts.theme)) {
                acee.setTheme('ace/theme/' + opts.theme);
            }
            if (angular.isString(opts.mode)) {
                session.setMode('ace/mode/' + opts.mode);
            }
            // Advanced options
            if (angular.isDefined(opts.firstLineNumber)) {
                if (angular.isNumber(opts.firstLineNumber)) {
                    session.setOption('firstLineNumber', opts.firstLineNumber);
                } else if (angular.isFunction(opts.firstLineNumber)) {
                    session.setOption('firstLineNumber', opts.firstLineNumber());
                }
            }

            // advanced options
            var key, obj;
            if (angular.isDefined(opts.advanced)) {
                for (key in opts.advanced) {
                    // create a javascript object with the key and value
                    obj = {
                        name: key,
                        value: opts.advanced[key]
                    };
                    // try to assign the option to the ace editor
                    acee.setOption(obj.name, obj.value);
                }
            }

            // advanced options for the renderer
            if (angular.isDefined(opts.rendererOptions)) {
                for (key in opts.rendererOptions) {
                    // create a javascript object with the key and value
                    obj = {
                        name: key,
                        value: opts.rendererOptions[key]
                    };
                    // try to assign the option to the ace editor
                    acee.renderer.setOption(obj.name, obj.value);
                }
            }

            // onLoad callbacks
            angular.forEach(opts.callbacks, function (cb) {
                if (angular.isFunction(cb)) {
                    cb(acee);
                }
            });
        };

        function linkStuff(scope, el, attr, ngModel) {

            /**
             * Corresponds the umbAceEditorConfig ACE configuration.
             * @type object
             */
            var options = umbAceEditorConfig.ace || {};

            /**
             * umbAceEditorConfig merged with user options via json in attribute or data binding
             * @type object
             */
            var opts = angular.extend({}, options, scope.$eval(attr.umbAceEditor));


            //load ace libraries here... 

            /**
             * ACE editor
             * @type object
             */
            var acee = window.ace.edit(el[0]);
            acee.$blockScrolling = Infinity;

            /**
             * ACE editor session.
             * @type object
             * @see [EditSession]{@link http://ace.c9.io/#nav=api&api=edit_session}
             */
            var session = acee.getSession();

            /**
             * Reference to a change listener created by the listener factory.
             * @function
             * @see listenerFactory.onChange
             */
            var onChangeListener;

            /**
             * Reference to a blur listener created by the listener factory.
             * @function
             * @see listenerFactory.onBlur
             */
            var onBlurListener;

            /**
             * Calls a callback by checking its existing. The argument list
             * is variable and thus this function is relying on the arguments
             * object.
             * @throws {Error} If the callback isn't a function
             */
            var executeUserCallback = function () {

                /**
                 * The callback function grabbed from the array-like arguments
                 * object. The first argument should always be the callback.
                 *
                 * @see [arguments]{@link https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Functions_and_function_scope/arguments}
                 * @type {*}
                 */
                var callback = arguments[0];

                /**
                 * Arguments to be passed to the callback. These are taken
                 * from the array-like arguments object. The first argument
                 * is stripped because that should be the callback function.
                 *
                 * @see [arguments]{@link https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Functions_and_function_scope/arguments}
                 * @type {Array}
                 */
                var args = Array.prototype.slice.call(arguments, 1);

                if (angular.isDefined(callback)) {
                    // removed evalAsync, seems to be too shady
                    //scope.$evalAsync(function () {
                    if (angular.isFunction(callback)) {
                        callback(args);
                    } else {
                        throw new Error('ui-ace use a function as callback.');
                    }
                    //});
                }
            };



            /**
             * Listener factory. Until now only change listeners can be created.
             * @type object
             */
            var listenerFactory = {
                /**
                 * Creates a change listener which propagates the change event
                 * and the editor session to the callback from the user option
                 * onChange. It might be exchanged during runtime, if this
                 * happens the old listener will be unbound.
                 *
                 * @param callback callback function defined in the user options
                 * @see onChangeListener
                 */
                onChange: function (callback) {
                    return function (e) {
                        var newValue = session.getValue();

                        if (ngModel && newValue !== ngModel.$viewValue &&
                            // HACK make sure to only trigger the apply outside of the
                            // digest loop 'cause ACE is actually using this callback
                            // for any text transformation !
                            !scope.$$phase && !scope.$root.$$phase) {
                            // removed evalasync, seems to be very shady
                            //scope.$evalAsync(function () {
                            ngModel.$setViewValue(newValue);
                            //});
                        }

                        executeUserCallback(callback, e, acee);
                    };
                },
                /**
                 * Creates a blur listener which propagates the editor session
                 * to the callback from the user option onBlur. It might be
                 * exchanged during runtime, if this happens the old listener
                 * will be unbound.
                 *
                 * @param callback callback function defined in the user options
                 * @see onBlurListener
                 */
                onBlur: function (callback) {
                    return function () {
                        executeUserCallback(callback, acee);
                    };
                }
            };

            attr.$observe('readonly', function (value) {
                acee.setReadOnly(!!value || value === '');
            });

            // Value Blind
            if (ngModel) {
                ngModel.$formatters.push(function (value) {
                    if (angular.isUndefined(value) || value === null) {
                        return '';
                    } else if (angular.isObject(value) || angular.isArray(value)) {
                        throw new Error('ui-ace cannot use an object or an array as a model');
                    }
                    return value;
                });

                ngModel.$render = function () {
                    session.setValue(ngModel.$viewValue);
                };
            }

            // Listen for option updates
            var updateOptions = function (current, previous) {
                if (current === previous) {
                    return;
                }
                opts = angular.extend({}, options, scope.$eval(attr.umbAceEditor));

                opts.callbacks = [opts.onLoad];
                if (opts.onLoad !== options.onLoad) {
                    // also call the global onLoad handler
                    opts.callbacks.unshift(options.onLoad);
                }

                // EVENTS

                // unbind old change listener
                session.removeListener('change', onChangeListener);

                // bind new change listener
                onChangeListener = listenerFactory.onChange(opts.onChange);
                session.on('change', onChangeListener);

                // unbind old blur listener
                //session.removeListener('blur', onBlurListener);
                acee.removeListener('blur', onBlurListener);

                // bind new blur listener
                onBlurListener = listenerFactory.onBlur(opts.onBlur);
                acee.on('blur', onBlurListener);

                setOptions(acee, session, opts);
            };

            scope.$watch(attr.umbAceEditor, updateOptions, /* deep watch */ true);

            // set the options here, even if we try to watch later, if this
            // line is missing things go wrong (and the tests will also fail)
            updateOptions(options);

            el.on('$destroy', function () {
                acee.session.$stopWorker();
                acee.destroy();
            });

            scope.$watch(function () {
                return [el[0].offsetWidth, el[0].offsetHeight];
            }, function () {
                acee.resize();
                acee.renderer.updateFull();
            }, true);
            scope.editor = acee;
        }

        function link(scope, el, attr, ngModel) {

            assetsService.load(['lib/ace-builds/src-min-noconflict/ace.js', 'lib/ace-builds/src-min-noconflict/ext-language_tools.js', '../Editor/Scripts/mode-forms.js']).then(function () {
                if (angular.isUndefined(window.ace)) {
                    throw new Error('ui-ace need ace to work... (o rly?)');
                } else {
                    linkStuff(scope, el, attr,ngModel);


                }
            });




        }

        var directive = {
            restrict: 'EA',
            require: '?ngModel',
            link: link
        };

        return directive;
    }

    angular.module('umbraco.directives')
        .constant('calcAceEditorConfig', {})
        .directive('calcAceEditor', ["calcAceEditorConfig", "assetsService", calcAceEditorDirective]);

})();

(function () {

    var ACE_NAMESPACE = "";

    var global = (function () { return this; })();
    if (!global && typeof window != "undefined") global = window; // strict mode


    if (!ACE_NAMESPACE && typeof requirejs !== "undefined")
        return;


    var define = function (module, deps, payload) {
        if (typeof module !== "string") {
            if (define.original)
                define.original.apply(this, arguments);
            else {
                console.error("dropping module because define wasn\'t a string.");
                console.trace();
            }
            return;
        }
        if (arguments.length == 2)
            payload = deps;
        if (!define.modules[module]) {
            define.payloads[module] = payload;
            define.modules[module] = null;
        }
    };

    define.modules = {};
    define.payloads = {};

    /**
     * Get at functionality define()ed using the function above
     */
    var _require = function (parentId, module, callback) {
        if (typeof module === "string") {
            var payload = lookup(parentId, module);
            if (payload != undefined) {
                callback && callback();
                return payload;
            }
        } else if (Object.prototype.toString.call(module) === "[object Array]") {
            var params = [];
            for (var i = 0, l = module.length; i < l; ++i) {
                var dep = lookup(parentId, module[i]);
                if (dep == undefined && require.original)
                    return;
                params.push(dep);
            }
            return callback && callback.apply(null, params) || true;
        }
    };

    var require = function (module, callback) {
        var packagedModule = _require("", module, callback);
        if (packagedModule == undefined && require.original)
            return require.original.apply(this, arguments);
        return packagedModule;
    };

    var normalizeModule = function (parentId, moduleName) {
        // normalize plugin requires
        if (moduleName.indexOf("!") !== -1) {
            var chunks = moduleName.split("!");
            return normalizeModule(parentId, chunks[0]) + "!" + normalizeModule(parentId, chunks[1]);
        }
        // normalize relative requires
        if (moduleName.charAt(0) == ".") {
            var base = parentId.split("/").slice(0, -1).join("/");
            moduleName = base + "/" + moduleName;

            while (moduleName.indexOf(".") !== -1 && previous != moduleName) {
                var previous = moduleName;
                moduleName = moduleName.replace(/\/\.\//, "/").replace(/[^\/]+\/\.\.\//, "");
            }
        }
        return moduleName;
    };

    /**
     * Internal function to lookup moduleNames and resolve them by calling the
     * definition function if needed.
     */
    var lookup = function (parentId, moduleName) {
        moduleName = normalizeModule(parentId, moduleName);

        var module = define.modules[moduleName];
        if (!module) {
            module = define.payloads[moduleName];
            if (typeof module === 'function') {
                var exports = {};
                var mod = {
                    id: moduleName,
                    uri: '',
                    exports: exports,
                    packaged: true
                };

                var req = function (module, callback) {
                    return _require(moduleName, module, callback);
                };

                var returnValue = module(req, exports, mod);
                exports = returnValue || mod.exports;
                define.modules[moduleName] = exports;
                delete define.payloads[moduleName];
            }
            module = define.modules[moduleName] = exports || module;
        }
        return module;
    };

    function exportAce(ns) {
        var root = global;
        if (ns) {
            if (!global[ns])
                global[ns] = {};
            root = global[ns];
        }

        if (!root.define || !root.define.packaged) {
            define.original = root.define;
            root.define = define;
            root.define.packaged = true;
        }

        if (!root.require || !root.require.packaged) {
            require.original = root.require;
            root.require = require;
            root.require.packaged = true;
        }
    }

    exportAce(ACE_NAMESPACE);

})();