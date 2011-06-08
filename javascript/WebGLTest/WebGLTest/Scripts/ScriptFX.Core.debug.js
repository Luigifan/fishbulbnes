//! Script# Framework
//! More information at http://projects.nikhilk.net/ScriptSharp
//!

(function() {
function executeScript() {

Type.registerNamespace('ssfx');

////////////////////////////////////////////////////////////////////////////////
// ssfx.IServiceContainer

ssfx.IServiceContainer = function() { };
ssfx.IServiceContainer.prototype = {
    registerService : null,
    unregisterService : null
}
ssfx.IServiceContainer.registerInterface('ssfx.IServiceContainer');


////////////////////////////////////////////////////////////////////////////////
// ssfx.IServiceProvider

ssfx.IServiceProvider = function() { };
ssfx.IServiceProvider.prototype = {
    getService : null
}
ssfx.IServiceProvider.registerInterface('ssfx.IServiceProvider');


////////////////////////////////////////////////////////////////////////////////
// ssfx._registeredEvent

ssfx.$create__registeredEvent = function ssfx__registeredEvent(eventType, sender, eventArgs, eventCookie) {
    var $o = { };
    $o.eventType = eventType;
    $o.sender = sender;
    $o.eventArgs = eventArgs;
    $o.eventCookie = eventCookie;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.IEventManager

ssfx.IEventManager = function() { };
ssfx.IEventManager.prototype = {
    raiseEvent : null,
    registerEvent : null,
    registerEventHandler : null,
    unregisterEvent : null,
    unregisterEventHandler : null
}
ssfx.IEventManager.registerInterface('ssfx.IEventManager');


////////////////////////////////////////////////////////////////////////////////
// ssfx.ISupportInitialize

ssfx.ISupportInitialize = function() { };
ssfx.ISupportInitialize.prototype = {
    beginInitialize : null,
    endInitialize : null
}
ssfx.ISupportInitialize.registerInterface('ssfx.ISupportInitialize');


////////////////////////////////////////////////////////////////////////////////
// ssfx.INotifyDisposing

ssfx.INotifyDisposing = function() { };
ssfx.INotifyDisposing.prototype = {
    add_disposing : null,
    remove_disposing : null
}
ssfx.INotifyDisposing.registerInterface('ssfx.INotifyDisposing');


////////////////////////////////////////////////////////////////////////////////
// ssfx.HostName

ssfx.HostName = function() { };
ssfx.HostName.prototype = {
    other: 0, 
    IE: 1, 
    mozilla: 2, 
    safari: 3, 
    opera: 4
}
ssfx.HostName.registerEnum('ssfx.HostName', false);


////////////////////////////////////////////////////////////////////////////////
// ssfx.ITask

ssfx.ITask = function() { };
ssfx.ITask.prototype = {
    execute : null
}
ssfx.ITask.registerInterface('ssfx.ITask');


////////////////////////////////////////////////////////////////////////////////
// ssfx.IObjectWithOwner

ssfx.IObjectWithOwner = function() { };
ssfx.IObjectWithOwner.prototype = {
    get_owner : null,
    setOwner : null
}
ssfx.IObjectWithOwner.registerInterface('ssfx.IObjectWithOwner');


////////////////////////////////////////////////////////////////////////////////
// ssfx.Application

ssfx.Application = function ssfx_Application() {
    this._disposableObjects = [];
    this._idleFrequency = 100;
    this._windowUnloadHandler = ss.Delegate.create(this, this._onWindowUnload);
    window.attachEvent('onunload', this._windowUnloadHandler);
    this._windowUnloadingHandler = ss.Delegate.create(this, this._onWindowUnloading);
    window.attachEvent('onbeforeunload', this._windowUnloadingHandler);
    this._windowErrorHandler = ss.Delegate.create(this, this._onWindowError);
    window.attachEvent('onerror', this._windowErrorHandler);
    ss.onDomReady(ss.Delegate.create(this, function() {
        var rootElement = document.documentElement;
        var className = rootElement.className;
        if (className.startsWith('$')) {
            var hostInfo = this.get_host();
            className = className.replaceAll('$browser', ssfx.HostName.toString(hostInfo.get_name()));
            className = className.replaceAll('$majorver', hostInfo.get_majorVersion().toString());
            className = className.replaceAll('$minorver', hostInfo.get_minorVersion().toString());
            rootElement.className = className;
        }
        var sessionElement = document.getElementById('__session');
        if (sessionElement != null) {
            var value = sessionElement.value;
            if (String.isNullOrEmpty(value)) {
                this._firstLoad = true;
                this._sessionState = {};
            }
            else {
                this._sessionState = ssfx.JSON.deserialize(value);
                if (ss.isUndefined(this._sessionState['__appLoaded'])) {
                    this._firstLoad = true;
                }
            }
            this._sessionState['__appLoaded'] = true;
        }
        else {
            this._firstLoad = true;
        }
    }));
    ss.onReady(ss.Delegate.create(this, this._onReady));
}
ssfx.Application.prototype = {
    _host: null,
    _isIE: 0,
    _loaded: false,
    _disposing: false,
    _firstLoad: false,
    _sessionState: null,
    _history: null,
    _events: null,
    _disposableObjects: null,
    _idleFrequency: 0,
    _idleTimer: 0,
    _taskQueue: null,
    _taskTimer: 0,
    _registeredEventHandlers: null,
    _registeredEventTypes: null,
    _registeredEvents: null,
    _services: null,
    _windowUnloadHandler: null,
    _windowUnloadingHandler: null,
    _windowErrorHandler: null,
    _idleTimerTickHandler: null,
    _taskTimerTickHandler: null,
    
    get_domain: function ssfx_Application$get_domain() {
        return window.document.domain;
    },
    set_domain: function ssfx_Application$set_domain(value) {
        window.document.domain = value;
        return value;
    },
    
    get__events: function ssfx_Application$get__events() {
        if (this._events == null) {
            this._events = new ssfx.EventList();
        }
        return this._events;
    },
    
    get_history: function ssfx_Application$get_history() {
        ss.Debug.assert(this._history != null, 'History has not been enabled.');
        return this._history;
    },
    
    get_host: function ssfx_Application$get_host() {
        if (this._host == null) {
            this._host = new ssfx.HostInfo();
        }
        return this._host;
    },
    
    get_idleFrequency: function ssfx_Application$get_idleFrequency() {
        return this._idleFrequency;
    },
    set_idleFrequency: function ssfx_Application$set_idleFrequency(value) {
        ss.Debug.assert(value >= 100, 'IdleFrequency must be atleast 100ms');
        this._idleFrequency = value;
        return value;
    },
    
    get_isFirstLoad: function ssfx_Application$get_isFirstLoad() {
        return this._firstLoad;
    },
    
    get_isIE: function ssfx_Application$get_isIE() {
        if (this._isIE === 0) {
            this._isIE = (this.get_host().get_name() === ssfx.HostName.IE) ? 1 : -1;
        }
        return (this._isIE === 1) ? true : false;
    },
    
    get_sessionState: function ssfx_Application$get_sessionState() {
        ss.Debug.assert(this._loaded, 'You must wait until the load event before accessing session.');
        ss.Debug.assert(this._sessionState != null, 'In order to use session, you must add an <input type=\"hidden\" id=\"__session\" /> within a <form>.');
        return this._sessionState;
    },
    
    add_error: function ssfx_Application$add_error(value) {
        this.get__events().addHandler('error', value);
    },
    remove_error: function ssfx_Application$remove_error(value) {
        this.get__events().removeHandler('error', value);
    },
    
    add_idle: function ssfx_Application$add_idle(value) {
        this.get__events().addHandler('idle', value);
        if (this._idleTimer === 0) {
            if (this._idleTimerTickHandler == null) {
                this._idleTimerTickHandler = ss.Delegate.create(this, this._onIdleTimerTick);
            }
            this._idleTimer = window.setTimeout(this._idleTimerTickHandler, this._idleFrequency);
        }
    },
    remove_idle: function ssfx_Application$remove_idle(value) {
        var isActive = this.get__events().removeHandler('idle', value);
        if ((!isActive) && (this._idleTimer !== 0)) {
            window.clearTimeout(this._idleTimer);
            this._idleTimer = 0;
        }
    },
    
    add_load: function ssfx_Application$add_load(value) {
        if (this._loaded) {
            value.invoke(this, ss.EventArgs.Empty);
        }
        else {
            this.get__events().addHandler('load', value);
        }
    },
    remove_load: function ssfx_Application$remove_load(value) {
        this.get__events().removeHandler('load', value);
    },
    
    add_unload: function ssfx_Application$add_unload(value) {
        this.get__events().addHandler('unload', value);
    },
    remove_unload: function ssfx_Application$remove_unload(value) {
        this.get__events().removeHandler('unload', value);
    },
    
    add_unloading: function ssfx_Application$add_unloading(value) {
        this.get__events().addHandler('unloading', value);
    },
    remove_unloading: function ssfx_Application$remove_unloading(value) {
        this.get__events().removeHandler('unloading', value);
    },
    
    addTask: function ssfx_Application$addTask(task) {
        if (this._taskQueue == null) {
            this._taskQueue = [];
        }
        this._taskQueue.enqueue(task);
        if (this._taskTimer === 0) {
            if (this._taskTimerTickHandler == null) {
                this._taskTimerTickHandler = ss.Delegate.create(this, this._onTaskTimerTick);
            }
            this._taskTimer = window.setTimeout(this._taskTimerTickHandler, 0);
        }
    },
    
    enableHistory: function ssfx_Application$enableHistory() {
        if (this._history != null) {
            return;
        }
        this._history = ssfx.HistoryManager._createHistory();
    },
    
    getService: function ssfx_Application$getService(serviceType) {
        ss.Debug.assert(serviceType != null);
        if ((serviceType === ssfx.IServiceContainer) || (serviceType === ssfx.IEventManager)) {
            return this;
        }
        if (this._services != null) {
            var name = serviceType.get_fullName().replaceAll('.', '$');
            return this._services[name];
        }
        return null;
    },
    
    _onIdleTimerTick: function ssfx_Application$_onIdleTimerTick() {
        this._idleTimer = 0;
        var handler = this.get__events().getHandler('idle');
        if (handler != null) {
            handler.invoke(this, ss.EventArgs.Empty);
            this._idleTimer = window.setTimeout(this._idleTimerTickHandler, this._idleFrequency);
        }
    },
    
    _onReady: function ssfx_Application$_onReady() {
        window.setTimeout(ss.Delegate.create(this, function() {
            this._loaded = true;
            var handler = this.get__events().getHandler('load');
            if (handler != null) {
                handler.invoke(this, ss.EventArgs.Empty);
            }
            if (this._history != null) {
                this._history._initialize();
            }
        }), 0);
    },
    
    _onTaskTimerTick: function ssfx_Application$_onTaskTimerTick() {
        this._taskTimer = 0;
        if (this._taskQueue.length !== 0) {
            var task = this._taskQueue.dequeue();
            if (!task.execute()) {
                this._taskQueue.enqueue(task);
            }
            else {
                if (Type.canCast(task, ss.IDisposable)) {
                    (task).dispose();
                }
            }
            if (this._taskQueue.length !== 0) {
                this._taskTimer = window.setTimeout(this._taskTimerTickHandler, 0);
            }
        }
    },
    
    _onWindowError: function ssfx_Application$_onWindowError() {
        var handler = this.get__events().getHandler('error');
        if (handler != null) {
            var ce = new ss.CancelEventArgs();
            ce.set_cancel(true);
            handler.invoke(this, ce);
            if (ce.get_cancel()) {
                window.event.returnValue = false;
            }
        }
    },
    
    _onWindowUnload: function ssfx_Application$_onWindowUnload() {
        this._disposing = true;
        if (this._taskTimer !== 0) {
            window.clearTimeout(this._taskTimer);
        }
        if (this._idleTimer !== 0) {
            window.clearTimeout(this._idleTimer);
        }
        var handler = this.get__events().getHandler('unload');
        if (handler != null) {
            handler.invoke(this, ss.EventArgs.Empty);
        }
        if (this._taskQueue != null) {
            while (this._taskQueue.length !== 0) {
                var task = this._taskQueue.dequeue();
                if (Type.canCast(task, ss.IDisposable)) {
                    (task).dispose();
                }
            }
        }
        if (this._disposableObjects.length !== 0) {
            var $enum1 = ss.IEnumerator.getEnumerator(this._disposableObjects);
            while ($enum1.moveNext()) {
                var disposable = $enum1.get_current();
                disposable.dispose();
            }
            this._disposableObjects.clear();
        }
        if (this._history != null) {
            this._history.dispose();
            this._history = null;
        }
        window.detachEvent('onunload', this._windowUnloadHandler);
        window.detachEvent('onbeforeunload', this._windowUnloadingHandler);
        window.detachEvent('onerror', this._windowErrorHandler);
        this._windowUnloadingHandler = null;
        this._windowErrorHandler = null;
        this._taskTimerTickHandler = null;
        this._idleTimerTickHandler = null;
    },
    
    _onWindowUnloading: function ssfx_Application$_onWindowUnloading() {
        window.event.avoidReturn = true;
        var handler = this.get__events().getHandler('unloading');
        if (handler != null) {
            var e = new ssfx.ApplicationUnloadingEventArgs();
            handler.invoke(this, e);
        }
        if (this._sessionState != null) {
            var sessionElement = document.getElementById('__session');
            sessionElement.value = ssfx.JSON.serialize(this._sessionState);
        }
    },
    
    raiseEvent: function ssfx_Application$raiseEvent(eventType, sender, e) {
        ss.Debug.assert(String.isNullOrEmpty(eventType));
        ss.Debug.assert(sender != null);
        ss.Debug.assert(e != null);
        if (this._registeredEventHandlers != null) {
            var handler = this._registeredEventHandlers[eventType];
            if (handler != null) {
                handler.invoke(sender, e);
            }
        }
    },
    
    registerDisposableObject: function ssfx_Application$registerDisposableObject(disposableObject) {
        if (!this._disposing) {
            this._disposableObjects.add(disposableObject);
        }
    },
    
    registerEvent: function ssfx_Application$registerEvent(eventType, sender, e) {
        ss.Debug.assert(String.isNullOrEmpty(eventType));
        ss.Debug.assert(sender != null);
        ss.Debug.assert(e != null);
        if (this._registeredEventHandlers != null) {
            var handler = this._registeredEventHandlers[eventType];
            if (handler != null) {
                handler.invoke(sender, e);
            }
        }
        if (this._registeredEvents == null) {
            this._registeredEvents = [];
        }
        if (this._registeredEventTypes == null) {
            this._registeredEventTypes = {};
            this._registeredEventTypes[eventType] = 1;
        }
        else {
            var eventCount = this._registeredEventTypes[eventType];
            if (ss.isUndefined(eventCount)) {
                this._registeredEventTypes[eventType] = 1;
            }
            else {
                this._registeredEventTypes[eventType] = 1 + eventCount;
            }
        }
        var eventInfo = ssfx.$create__registeredEvent(eventType, sender, e, this._registeredEvents.length);
        this._registeredEvents.add(eventInfo);
        return eventInfo.eventCookie;
    },
    
    registerEventHandler: function ssfx_Application$registerEventHandler(eventType, handler) {
        ss.Debug.assert(!String.isNullOrEmpty(eventType));
        ss.Debug.assert(handler != null);
        var existingHandler = null;
        if (this._registeredEventHandlers == null) {
            this._registeredEventHandlers = {};
        }
        else {
            existingHandler = this._registeredEventHandlers[eventType];
        }
        this._registeredEventHandlers[eventType] = ss.Delegate.combine(existingHandler, handler);
        if (!ss.isNullOrUndefined(this._registeredEventTypes[eventType])) {
            var $enum1 = ss.IEnumerator.getEnumerator(this._registeredEvents);
            while ($enum1.moveNext()) {
                var eventInfo = $enum1.get_current();
                if (eventInfo == null) {
                    continue;
                }
                if (eventInfo.eventType === eventType) {
                    handler.invoke(eventInfo.sender, eventInfo.eventArgs);
                }
            }
        }
    },
    
    registerService: function ssfx_Application$registerService(serviceType, service) {
        ss.Debug.assert(serviceType != null);
        ss.Debug.assert(service != null);
        if (this._services == null) {
            this._services = {};
        }
        var name = serviceType.get_fullName().replaceAll('.', '$');
        ss.Debug.assert(this._services[name] == null);
        this._services[name] = service;
    },
    
    unregisterDisposableObject: function ssfx_Application$unregisterDisposableObject(disposableObject) {
        ss.Debug.assert(disposableObject != null);
        if (!this._disposing) {
            this._disposableObjects.remove(disposableObject);
        }
    },
    
    unregisterEvent: function ssfx_Application$unregisterEvent(eventCookie) {
        ss.Debug.assert(eventCookie != null);
        ss.Debug.assert(Type.canCast(eventCookie, Number));
        ss.Debug.assert(this._registeredEvents != null);
        ss.Debug.assert(this._registeredEventTypes != null);
        var eventInfo = this._registeredEvents[eventCookie];
        ss.Debug.assert(eventInfo != null);
        var eventCount = this._registeredEventTypes[eventInfo.eventType];
        ss.Debug.assert(eventCount >= 1);
        if (eventCount === 1) {
            delete this._registeredEventTypes[eventInfo.eventType];
        }
        else {
            this._registeredEventTypes[eventInfo.eventType] = eventCount - 1;
        }
        this._registeredEvents[eventCookie] = null;
    },
    
    unregisterEventHandler: function ssfx_Application$unregisterEventHandler(eventType, handler) {
        ss.Debug.assert(!String.isNullOrEmpty(eventType));
        ss.Debug.assert(handler != null);
        if (this._registeredEventHandlers != null) {
            var existingHandler = this._registeredEventHandlers[eventType];
            if (existingHandler != null) {
                existingHandler = ss.Delegate.remove(existingHandler, handler);
                if (existingHandler == null) {
                    delete this._registeredEventHandlers[eventType];
                }
                else {
                    this._registeredEventHandlers[eventType] = existingHandler;
                }
            }
        }
    },
    
    unregisterService: function ssfx_Application$unregisterService(serviceType) {
        ss.Debug.assert(serviceType != null);
        if (this._services != null) {
            var name = serviceType.get_fullName().replaceAll('.', '$');
            delete this._services[name];
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.ApplicationUnloadingEventArgs

ssfx.ApplicationUnloadingEventArgs = function ssfx_ApplicationUnloadingEventArgs() {
    ssfx.ApplicationUnloadingEventArgs.initializeBase(this);
}
ssfx.ApplicationUnloadingEventArgs.prototype = {
    
    setUnloadPrompt: function ssfx_ApplicationUnloadingEventArgs$setUnloadPrompt(prompt) {
        window.event.returnValue = prompt;
        window.event.avoidReturn = false;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.HistoryManager

ssfx.HistoryManager = function ssfx_HistoryManager(enabled, iframe) {
    this._enabled = enabled;
    this._iframe = iframe;
}
ssfx.HistoryManager._createHistory = function ssfx_HistoryManager$_createHistory() {
    var hostName = ssfx.Application.current.get_host().get_name();
    if ((hostName !== ssfx.HostName.IE) && (hostName !== ssfx.HostName.mozilla)) {
        return new ssfx.HistoryManager(false, null);
    }
    var iframe = null;
    if (hostName === ssfx.HostName.IE) {
        iframe = document.getElementById('_historyFrame');
        ss.Debug.assert(iframe != null, 'You must have an <iframe id=\"_historyFrame\" src=\"Empty.htm\" /> tag on your page.');
    }
    return new ssfx.HistoryManager(true, iframe);
}
ssfx.HistoryManager.prototype = {
    _enabled: false,
    _iframe: null,
    _emptyPageURL: null,
    _iframeLoadHandler: null,
    _ignoreTimer: false,
    _ignoreIFrame: false,
    _currentEntry: null,
    
    get_isEnabled: function ssfx_HistoryManager$get_isEnabled() {
        return this._enabled;
    },
    
    add_navigated: function ssfx_HistoryManager$add_navigated(value) {
        this.__navigated = ss.Delegate.combine(this.__navigated, value);
    },
    remove_navigated: function ssfx_HistoryManager$remove_navigated(value) {
        this.__navigated = ss.Delegate.remove(this.__navigated, value);
    },
    
    __navigated: null,
    
    addEntry: function ssfx_HistoryManager$addEntry(entryName) {
        ss.Debug.assert(!String.isNullOrEmpty(entryName));
        ss.Debug.assert(document.getElementById(entryName) == null, 'The entry identifier should not be the same as an element ID.');
        if (!this._enabled) {
            return;
        }
        this._ignoreTimer = true;
        if (this._iframe != null) {
            this._ignoreIFrame = true;
            this._iframe.src = this._emptyPageURL + entryName;
        }
        else {
            this._setCurrentEntry(entryName);
        }
    },
    
    dispose: function ssfx_HistoryManager$dispose() {
        if (this._iframe != null) {
            this._iframe.detachEvent('onload', this._iframeLoadHandler);
            this._iframe = null;
        }
    },
    
    _getCurrentEntry: function ssfx_HistoryManager$_getCurrentEntry() {
        var entryName = window.location.hash;
        if ((entryName.length !== 0) && (entryName.charAt(0) === '#')) {
            entryName = entryName.substr(1);
        }
        return entryName;
    },
    
    goBack: function ssfx_HistoryManager$goBack() {
        window.history.back();
    },
    
    goForward: function ssfx_HistoryManager$goForward() {
        window.history.forward();
    },
    
    _initialize: function ssfx_HistoryManager$_initialize() {
        if (!this._enabled) {
            return;
        }
        ssfx.Application.current.add_idle(ss.Delegate.create(this, this._onAppIdle));
        if (this._iframe != null) {
            ss.Debug.assert(this._iframe.src.length !== 0, 'You must set the Src attribute of the history iframe element to an empty page.');
            this._emptyPageURL = this._iframe.src + '?';
            this._iframeLoadHandler = ss.Delegate.create(this, this._onIFrameLoad);
            this._iframe.attachEvent('onload', this._iframeLoadHandler);
        }
        this._currentEntry = this._getCurrentEntry();
        this._onNavigated(this._currentEntry);
    },
    
    _onAppIdle: function ssfx_HistoryManager$_onAppIdle(sender, e) {
        var entryName = this._getCurrentEntry();
        if (entryName !== this._currentEntry) {
            if (this._ignoreTimer) {
                return;
            }
            this._currentEntry = entryName;
            this._onNavigated(entryName);
        }
        else {
            this._ignoreTimer = false;
        }
    },
    
    _onIFrameLoad: function ssfx_HistoryManager$_onIFrameLoad() {
        var entryName = this._iframe.contentWindow.location.search;
        if ((entryName.length !== 0) && (entryName.charAt(0) === '?')) {
            entryName = entryName.substr(1);
        }
        this._setCurrentEntry(entryName);
        if (this._ignoreIFrame) {
            this._ignoreIFrame = false;
            return;
        }
        this._onNavigated(entryName);
    },
    
    _onNavigated: function ssfx_HistoryManager$_onNavigated(entryName) {
        if (this.__navigated != null) {
            this.__navigated.invoke(this, new ssfx.HistoryEventArgs(entryName));
        }
    },
    
    _setCurrentEntry: function ssfx_HistoryManager$_setCurrentEntry(entryName) {
        this._currentEntry = entryName;
        window.location.hash = entryName;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.HistoryEventArgs

ssfx.HistoryEventArgs = function ssfx_HistoryEventArgs(entryName) {
    ssfx.HistoryEventArgs.initializeBase(this);
    this._entryName$1 = entryName;
}
ssfx.HistoryEventArgs.prototype = {
    _entryName$1: null,
    
    get_entryName: function ssfx_HistoryEventArgs$get_entryName() {
        return this._entryName$1;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.HostInfo

ssfx.HostInfo = function ssfx_HostInfo() {
    var userAgent = window.navigator.userAgent.toLowerCase();
    var version = null;
    var index;
    if ((index = userAgent.indexOf('opera')) >= 0) {
        this._name = ssfx.HostName.opera;
        version = userAgent.substr(index + 6);
    }
    else if ((index = userAgent.indexOf('msie')) >= 0) {
        this._name = ssfx.HostName.IE;
        version = userAgent.substr(index + 5);
    }
    else if ((index = userAgent.indexOf('safari')) >= 0) {
        this._name = ssfx.HostName.safari;
        version = userAgent.substr(index + 7);
    }
    else if ((index = userAgent.indexOf('firefox')) >= 0) {
        this._name = ssfx.HostName.mozilla;
        version = userAgent.substr(index + 8);
    }
    else if (userAgent.indexOf('gecko') >= 0) {
        this._name = ssfx.HostName.mozilla;
        version = window.navigator.appVersion;
    }
    if (version != null) {
        this._version = parseFloat(version);
        this._majorVersion = parseInt(this._version);
        if ((index = version.indexOf('.')) >= 0) {
            this._minorVersion = parseInt(version.substr(index + 1));
        }
    }
}
ssfx.HostInfo.prototype = {
    _name: 0,
    _version: 0,
    _majorVersion: 0,
    _minorVersion: 0,
    
    get_majorVersion: function ssfx_HostInfo$get_majorVersion() {
        return this._majorVersion;
    },
    
    get_minorVersion: function ssfx_HostInfo$get_minorVersion() {
        return this._minorVersion;
    },
    
    get_name: function ssfx_HostInfo$get_name() {
        return this._name;
    },
    
    get_version: function ssfx_HostInfo$get_version() {
        return this._version;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.EventList

ssfx.EventList = function ssfx_EventList() {
}
ssfx.EventList.prototype = {
    _events: null,
    
    addHandler: function ssfx_EventList$addHandler(key, handler) {
        ss.Debug.assert(!String.isNullOrEmpty(key));
        ss.Debug.assert(handler != null);
        if (this._events == null) {
            this._events = {};
        }
        this._events[key] = ss.Delegate.combine(this._events[key], handler);
    },
    
    getHandler: function ssfx_EventList$getHandler(key) {
        ss.Debug.assert(!String.isNullOrEmpty(key));
        if (this._events != null) {
            return this._events[key];
        }
        return null;
    },
    
    removeHandler: function ssfx_EventList$removeHandler(key, handler) {
        ss.Debug.assert(!String.isNullOrEmpty(key));
        ss.Debug.assert(handler != null);
        if (this._events != null) {
            var sourceHandler = this._events[key];
            if (sourceHandler != null) {
                var newHandler = ss.Delegate.remove(sourceHandler, handler);
                this._events[key] = newHandler;
                return (newHandler != null);
            }
        }
        return false;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.JSON

ssfx.JSON = function ssfx_JSON() {
}
ssfx.JSON.deserialize = function ssfx_JSON$deserialize(s) {
    if (String.isNullOrEmpty(s)) {
        return null;
    }
    if (ssfx.JSON._dateRegex == null) {
        ssfx.JSON._dateRegex = new RegExp('(\'|\")\\\\@(-?[0-9]+)@(\'|\")', 'gm');
    }
    s = s.replace(ssfx.JSON._dateRegex, 'new Date($2)');
    return eval('(' + s + ')');
}
ssfx.JSON.serialize = function ssfx_JSON$serialize(o) {
    if (ss.isNullOrUndefined(o)) {
        return String.Empty;
    }
    var sb = new ss.StringBuilder();
    ssfx.JSON._serializeCore(sb, o);
    return sb.toString();
}
ssfx.JSON._serializeCore = function ssfx_JSON$_serializeCore(sb, o) {
    if (ss.isNullOrUndefined(o)) {
        sb.append('null');
        return;
    }
    var scriptType = typeof(o);
    switch (scriptType) {
        case 'boolean':
            sb.append(o.toString());
            return;
        case 'number':
            sb.append((isFinite(o)) ? o.toString() : 'null');
            return;
        case 'string':
            sb.append((o).quote());
            return;
        case 'object':
            if (Array.isInstanceOfType(o)) {
                sb.append('[');
                var a = o;
                var length = a.length;
                var first = true;
                for (var i = 0; i < length; i++) {
                    if (first) {
                        first = false;
                    }
                    else {
                        sb.append(',');
                    }
                    ssfx.JSON._serializeCore(sb, a[i]);
                }
                sb.append(']');
            }
            else if (Date.isInstanceOfType(o)) {
                var d = o;
                var utcValue = Date.UTC(d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate(), d.getUTCHours(), d.getUTCMinutes(), d.getUTCSeconds(), d.getUTCMilliseconds());
                sb.append('\"\\@');
                sb.append(utcValue.toString());
                sb.append('@\"');
            }
            else if (RegExp.isInstanceOfType(o)) {
                sb.append(o.toString());
            }
            else {
                sb.append('{');
                var first = true;
                var $dict1 = o;
                for (var $key2 in $dict1) {
                    var entry = { key: $key2, value: $dict1[$key2] };
                    if ((entry.key).startsWith('$') || Function.isInstanceOfType(entry.value)) {
                        continue;
                    }
                    if (first) {
                        first = false;
                    }
                    else {
                        sb.append(',');
                    }
                    sb.append(entry.key);
                    sb.append(':');
                    ssfx.JSON._serializeCore(sb, entry.value);
                }
                sb.append('}');
            }
            return;
        default:
            ss.Debug.fail(scriptType + ' is not supported for JSON serialization.');
            sb.append('null');
            return;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.ObservableCollection

ssfx.ObservableCollection = function ssfx_ObservableCollection(owner, disposableItems) {
    this._owner = owner;
    this._items = [];
    this._disposableItems = disposableItems;
}
ssfx.ObservableCollection.prototype = {
    _owner: null,
    _items: null,
    _disposableItems: false,
    _handler: null,
    
    get_count: function ssfx_ObservableCollection$get_count() {
        return this._items.length;
    },
    
    add_collectionChanged: function ssfx_ObservableCollection$add_collectionChanged(value) {
        this._handler = ss.Delegate.combine(this._handler, value);
    },
    remove_collectionChanged: function ssfx_ObservableCollection$remove_collectionChanged(value) {
        this._handler = ss.Delegate.remove(this._handler, value);
    },
    
    add: function ssfx_ObservableCollection$add(item) {
        (item).setOwner(this._owner);
        this._items.add(item);
        if (this._handler != null) {
            this._handler.invoke(this, new ss.CollectionChangedEventArgs(0, item));
        }
    },
    
    clear: function ssfx_ObservableCollection$clear() {
        if (this._items.length !== 0) {
            var $enum1 = ss.IEnumerator.getEnumerator(this._items);
            while ($enum1.moveNext()) {
                var item = $enum1.get_current();
                item.setOwner(null);
            }
            this._items.clear();
            if (this._handler != null) {
                this._handler.invoke(this, new ss.CollectionChangedEventArgs(2, null));
            }
        }
    },
    
    contains: function ssfx_ObservableCollection$contains(item) {
        return this._items.contains(item);
    },
    
    dispose: function ssfx_ObservableCollection$dispose() {
        if (this._disposableItems) {
            var $enum1 = ss.IEnumerator.getEnumerator(this._items);
            while ($enum1.moveNext()) {
                var item = $enum1.get_current();
                item.dispose();
            }
        }
        this._items = null;
        this._owner = null;
        this._handler = null;
    },
    
    getEnumerator: function ssfx_ObservableCollection$getEnumerator() {
        return this._items.getEnumerator();
    },
    
    getItems: function ssfx_ObservableCollection$getItems() {
        return this._items;
    },
    
    remove: function ssfx_ObservableCollection$remove(item) {
        if (this._items.contains(item)) {
            (item).setOwner(null);
            this._items.remove(item);
            if (this._handler != null) {
                this._handler.invoke(this, new ss.CollectionChangedEventArgs(1, item));
            }
        }
    },
    get_item: function ssfx_ObservableCollection$get_item(index) {
        return this._items[index];
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.HttpStatusCode

ssfx.HttpStatusCode = function() { };
ssfx.HttpStatusCode.prototype = {
    canContinue: 100, 
    switchingProtocols: 101, 
    OK: 200, 
    created: 201, 
    partialContent: 206, 
    accepted: 202, 
    nonAuthoritativeInformation: 203, 
    noContent: 204, 
    resetContent: 205, 
    ambiguous: 300, 
    moved: 301, 
    redirect: 302, 
    redirectMethod: 303, 
    notModified: 304, 
    useProxy: 305, 
    temporaryRedirect: 307, 
    badRequest: 400, 
    methodNotAllowed: 400, 
    unauthorized: 401, 
    paymentRequired: 402, 
    forbidden: 403, 
    notFound: 404, 
    notAcceptable: 406, 
    proxyAuthenticationRequired: 407, 
    requestTimeout: 408, 
    conflict: 409, 
    gone: 410, 
    lengthRequired: 411, 
    preconditionFailed: 412, 
    requestEntityTooLarge: 413, 
    requestUriTooLong: 414, 
    unsupportedMediaType: 415, 
    requestedRangeNotSatisfiable: 416, 
    expectationFailed: 417, 
    internalServerError: 500, 
    notImplemented: 501, 
    badGateway: 502, 
    serviceUnavailable: 503, 
    gatewayTimeout: 504, 
    httpVersionNotSupported: 505
}
ssfx.HttpStatusCode.registerEnum('ssfx.HttpStatusCode', false);


////////////////////////////////////////////////////////////////////////////////
// ssfx.HttpRequestState

ssfx.HttpRequestState = function() { };
ssfx.HttpRequestState.prototype = {
    inactive: 0, 
    inProgress: 1, 
    completed: 2, 
    aborted: 3, 
    timedOut: 4
}
ssfx.HttpRequestState.registerEnum('ssfx.HttpRequestState', false);


////////////////////////////////////////////////////////////////////////////////
// ssfx.HttpVerb

ssfx.HttpVerb = function() { };
ssfx.HttpVerb.prototype = {
    GET: 0, 
    POST: 1, 
    PUT: 2, 
    DELETE: 3
}
ssfx.HttpVerb.registerEnum('ssfx.HttpVerb', false);


////////////////////////////////////////////////////////////////////////////////
// ssfx.IHttpResponse

ssfx.IHttpResponse = function() { };
ssfx.IHttpResponse.prototype = {
    get_contentLength : null,
    get_contentType : null,
    get_headers : null,
    get_request : null,
    get_statusCode : null,
    get_statusText : null,
    get_timeStamp : null,
    getHeader : null,
    getObject : null,
    getText : null,
    getXml : null
}
ssfx.IHttpResponse.registerInterface('ssfx.IHttpResponse');


////////////////////////////////////////////////////////////////////////////////
// ssfx.HttpRequest

ssfx.HttpRequest = function ssfx_HttpRequest() {
}
ssfx.HttpRequest.createRequest = function ssfx_HttpRequest$createRequest(uri, verb) {
    ss.Debug.assert(!String.isNullOrEmpty(uri));
    var request = new ssfx.HttpRequest();
    if (!uri.startsWith('{')) {
        request._uri = uri;
    }
    else {
        var uriData = ssfx.JSON.deserialize(uri);
        request._uri = uriData['__uri'];
        ss.Debug.assert(!String.isNullOrEmpty(request._uri));
        if (uriData['__nullParams']) {
            request._transportType = uriData['__transportType'];
        }
        else {
            request._transportType = Type.getType(uriData['__transportType']);
            delete uriData.__uri;
            delete uriData.__transportType;
            request._transportParameters = uriData;
        }
        ss.Debug.assert((request._transportType != null) && ssfx.HttpTransport.isAssignableFrom(request._transportType));
    }
    request._verb = verb;
    return request;
}
ssfx.HttpRequest.createUri = function ssfx_HttpRequest$createUri(uri, parameters) {
    var sb = new ss.StringBuilder(uri);
    if (uri.indexOf('?') < 0) {
        sb.append('?');
    }
    var parameterIndex = 0;
    var $dict1 = parameters;
    for (var $key2 in $dict1) {
        var entry = { key: $key2, value: $dict1[$key2] };
        if (parameterIndex !== 0) {
            sb.append('&');
        }
        sb.append(entry.key);
        sb.append('=');
        sb.append(encodeURIComponent(entry.value.toString()));
        parameterIndex++;
    }
    return sb.toString();
}
ssfx.HttpRequest.prototype = {
    _uri: null,
    _verb: 0,
    _content: null,
    _headers: null,
    _userName: null,
    _password: null,
    _transportType: null,
    _transportParameters: null,
    _timeout: 0,
    _callback: null,
    _context: null,
    _state: 0,
    _transport: null,
    _response: null,
    _timeStamp: null,
    
    get_content: function ssfx_HttpRequest$get_content() {
        return this._content;
    },
    set_content: function ssfx_HttpRequest$set_content(value) {
        ss.Debug.assert(this.get_verb() === ssfx.HttpVerb.POST);
        ss.Debug.assert(this._state === ssfx.HttpRequestState.inactive);
        this._content = value;
        return value;
    },
    
    get_hasCredentials: function ssfx_HttpRequest$get_hasCredentials() {
        return (!String.isNullOrEmpty(this._userName));
    },
    
    get_hasHeaders: function ssfx_HttpRequest$get_hasHeaders() {
        return (this._headers != null);
    },
    
    get_headers: function ssfx_HttpRequest$get_headers() {
        if (this._headers == null) {
            this._headers = {};
        }
        return this._headers;
    },
    
    get_password: function ssfx_HttpRequest$get_password() {
        return this._password;
    },
    
    get_response: function ssfx_HttpRequest$get_response() {
        ss.Debug.assert(this._state === ssfx.HttpRequestState.completed);
        return this._response;
    },
    
    get_state: function ssfx_HttpRequest$get_state() {
        return this._state;
    },
    
    get_timeout: function ssfx_HttpRequest$get_timeout() {
        return this._timeout;
    },
    set_timeout: function ssfx_HttpRequest$set_timeout(value) {
        this._timeout = value;
        return value;
    },
    
    get_timeStamp: function ssfx_HttpRequest$get_timeStamp() {
        return this._timeStamp;
    },
    
    get__transport: function ssfx_HttpRequest$get__transport() {
        return this._transport;
    },
    
    get__transportParameters: function ssfx_HttpRequest$get__transportParameters() {
        return this._transportParameters;
    },
    
    get_transportType: function ssfx_HttpRequest$get_transportType() {
        return this._transportType;
    },
    
    get_URI: function ssfx_HttpRequest$get_URI() {
        return this._uri;
    },
    
    get_userName: function ssfx_HttpRequest$get_userName() {
        return this._userName;
    },
    
    get_verb: function ssfx_HttpRequest$get_verb() {
        return this._verb;
    },
    
    abort: function ssfx_HttpRequest$abort() {
        if (this._state === ssfx.HttpRequestState.inProgress) {
            ssfx.HttpRequestManager._abort(this, false);
        }
    },
    
    dispose: function ssfx_HttpRequest$dispose() {
        if (this._transport != null) {
            this.abort();
        }
    },
    
    invoke: function ssfx_HttpRequest$invoke(callback, context) {
        ss.Debug.assert(this._state === ssfx.HttpRequestState.inactive);
        this._callback = callback;
        this._context = context;
        ssfx.Application.current.registerDisposableObject(this);
        ssfx.HttpRequestManager._beginInvoke(this);
    },
    
    _invokeCallback: function ssfx_HttpRequest$_invokeCallback() {
        ssfx.Application.current.unregisterDisposableObject(this);
        if (this._transport != null) {
            this._transport.dispose();
            this._transport = null;
        }
        if (this._callback != null) {
            this._callback.invoke(this, this._context);
            this._callback = null;
            this._context = null;
        }
    },
    
    _onAbort: function ssfx_HttpRequest$_onAbort() {
        this._state = ssfx.HttpRequestState.aborted;
        this._invokeCallback();
    },
    
    _onActivate: function ssfx_HttpRequest$_onActivate(transport) {
        this._transport = transport;
        this._state = ssfx.HttpRequestState.inProgress;
        this._timeStamp = new Date();
    },
    
    _onCompleted: function ssfx_HttpRequest$_onCompleted(response) {
        this._response = response;
        this._state = ssfx.HttpRequestState.completed;
        this._invokeCallback();
    },
    
    _onTimeout: function ssfx_HttpRequest$_onTimeout() {
        this._state = ssfx.HttpRequestState.timedOut;
        this._invokeCallback();
    },
    
    setContentAsForm: function ssfx_HttpRequest$setContentAsForm(data) {
        ss.Debug.assert(data != null);
        this.get_headers()['Content-Type'] = 'application/x-www-form-urlencoded';
        var sb = new ss.StringBuilder();
        var firstValue = true;
        var $dict1 = data;
        for (var $key2 in $dict1) {
            var e = { key: $key2, value: $dict1[$key2] };
            if (!firstValue) {
                sb.append('&');
            }
            sb.append(e.key);
            sb.append('=');
            sb.append(encodeURIComponent(e.value.toString()));
            firstValue = false;
        }
        this.set_content(sb.toString());
    },
    
    setContentAsJSON: function ssfx_HttpRequest$setContentAsJSON(data) {
        ss.Debug.assert(data != null);
        this.get_headers()['Content-Type'] = 'text/json';
        this.set_content(ssfx.JSON.serialize(data));
    },
    
    setCredentials: function ssfx_HttpRequest$setCredentials(userName, password) {
        ss.Debug.assert(!String.isNullOrEmpty(userName));
        ss.Debug.assert(!String.isNullOrEmpty(password));
        this._userName = userName;
        this._password = password;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.HttpRequestManager

ssfx.HttpRequestManager = function ssfx_HttpRequestManager() {
}
ssfx.HttpRequestManager.add_requestInvoking = function ssfx_HttpRequestManager$add_requestInvoking(value) {
    ssfx.HttpRequestManager.__requestInvoking = ss.Delegate.combine(ssfx.HttpRequestManager.__requestInvoking, value);
}
ssfx.HttpRequestManager.remove_requestInvoking = function ssfx_HttpRequestManager$remove_requestInvoking(value) {
    ssfx.HttpRequestManager.__requestInvoking = ss.Delegate.remove(ssfx.HttpRequestManager.__requestInvoking, value);
}
ssfx.HttpRequestManager.add_requestInvoked = function ssfx_HttpRequestManager$add_requestInvoked(value) {
    ssfx.HttpRequestManager.__requestInvoked = ss.Delegate.combine(ssfx.HttpRequestManager.__requestInvoked, value);
}
ssfx.HttpRequestManager.remove_requestInvoked = function ssfx_HttpRequestManager$remove_requestInvoked(value) {
    ssfx.HttpRequestManager.__requestInvoked = ss.Delegate.remove(ssfx.HttpRequestManager.__requestInvoked, value);
}
ssfx.HttpRequestManager.get_online = function ssfx_HttpRequestManager$get_online() {
    return window.navigator.onLine;
}
ssfx.HttpRequestManager.get_timeoutInterval = function ssfx_HttpRequestManager$get_timeoutInterval() {
    return ssfx.HttpRequestManager._timeoutInterval;
}
ssfx.HttpRequestManager.set_timeoutInterval = function ssfx_HttpRequestManager$set_timeoutInterval(value) {
    ssfx.HttpRequestManager._timeoutInterval = value;
    return value;
}
ssfx.HttpRequestManager._abort = function ssfx_HttpRequestManager$_abort(request, timedOut) {
    var transport = request.get__transport();
    if (transport != null) {
        transport.abort();
        ssfx.HttpRequestManager._endInvoke(request, null, timedOut);
    }
}
ssfx.HttpRequestManager.abortAll = function ssfx_HttpRequestManager$abortAll() {
    var requests = ssfx.HttpRequestManager._activeRequests;
    ssfx.HttpRequestManager._activeRequests = [];
    var $enum1 = ss.IEnumerator.getEnumerator(requests);
    while ($enum1.moveNext()) {
        var request = $enum1.get_current();
        ssfx.HttpRequestManager._abort(request, false);
    }
}
ssfx.HttpRequestManager._beginInvoke = function ssfx_HttpRequestManager$_beginInvoke(request) {
    if (ssfx.HttpRequestManager.__requestInvoking != null) {
        var e = new ssfx.PreHttpRequestEventArgs(request);
        ssfx.HttpRequestManager.__requestInvoking.invoke(null, e);
        if (e.get_isSuppressed()) {
            request._onCompleted(e.get_response());
            return;
        }
    }
    var transportType = request.get_transportType();
    if (transportType == null) {
        transportType = ssfx._xmlHttpTransport;
    }
    var transport = new transportType(request);
    request._onActivate(transport);
    ssfx.HttpRequestManager._activeRequests.add(request);
    transport.invoke();
    if (((ssfx.HttpRequestManager._timeoutInterval !== 0) || (request.get_timeout() !== 0)) && (ssfx.HttpRequestManager._appIdleHandler == null)) {
        ssfx.HttpRequestManager._appIdleHandler = ssfx.HttpRequestManager._onApplicationIdle;
        ssfx.Application.current.add_idle(ssfx.HttpRequestManager._appIdleHandler);
    }
}
ssfx.HttpRequestManager._endInvoke = function ssfx_HttpRequestManager$_endInvoke(request, response, timedOut) {
    ssfx.HttpRequestManager._activeRequests.remove(request);
    if (response != null) {
        request._onCompleted(response);
    }
    else if (timedOut) {
        request._onTimeout();
    }
    else {
        request._onAbort();
    }
    if (ssfx.HttpRequestManager.__requestInvoked != null) {
        var e = new ssfx.PostHttpRequestEventArgs(request, response);
        ssfx.HttpRequestManager.__requestInvoked.invoke(null, e);
    }
    if ((ssfx.HttpRequestManager._activeRequests.length === 0) && (ssfx.HttpRequestManager._appIdleHandler != null)) {
        ssfx.Application.current.remove_idle(ssfx.HttpRequestManager._appIdleHandler);
        ssfx.HttpRequestManager._appIdleHandler = null;
    }
}
ssfx.HttpRequestManager._onApplicationIdle = function ssfx_HttpRequestManager$_onApplicationIdle(sender, e) {
    if (ssfx.HttpRequestManager._activeRequests.length === 0) {
        return;
    }
    var timedOutRequests = null;
    var currentTimeValue = new Date().getTime();
    var $enum1 = ss.IEnumerator.getEnumerator(ssfx.HttpRequestManager._activeRequests);
    while ($enum1.moveNext()) {
        var request = $enum1.get_current();
        var timeStampValue = request.get_timeStamp().getTime();
        var interval = request.get_timeout();
        if (interval === 0) {
            interval = ssfx.HttpRequestManager._timeoutInterval;
            if (interval === 0) {
                continue;
            }
        }
        if ((currentTimeValue - timeStampValue) > interval) {
            if (timedOutRequests == null) {
                timedOutRequests = [];
            }
            timedOutRequests.add(request);
        }
    }
    if (timedOutRequests != null) {
        var $enum2 = ss.IEnumerator.getEnumerator(timedOutRequests);
        while ($enum2.moveNext()) {
            var request = $enum2.get_current();
            ssfx.HttpRequestManager._abort(request, true);
        }
    }
}
ssfx.HttpRequestManager._onCompleted = function ssfx_HttpRequestManager$_onCompleted(request, response) {
    ssfx.HttpRequestManager._endInvoke(request, response, false);
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.HttpTransport

ssfx.HttpTransport = function ssfx_HttpTransport(request) {
    this._request = request;
}
ssfx.HttpTransport.createUri = function ssfx_HttpTransport$createUri(uri, transportType, parameters) {
    ss.Debug.assert(!String.isNullOrEmpty(uri));
    ss.Debug.assert((transportType != null) && ssfx.HttpTransport.isAssignableFrom(transportType));
    if (parameters == null) {
        return '{__nullParams: true, __uri:\'' + uri + '\', __transportType: ' + transportType.get_fullName() + '}';
    }
    else {
        parameters['__uri'] = uri;
        parameters['__transportType'] = transportType.get_fullName();
        return ssfx.JSON.serialize(parameters);
    }
}
ssfx.HttpTransport.prototype = {
    _request: null,
    
    get_parameters: function ssfx_HttpTransport$get_parameters() {
        return this._request.get__transportParameters();
    },
    
    get_request: function ssfx_HttpTransport$get_request() {
        return this._request;
    },
    
    getMethod: function ssfx_HttpTransport$getMethod() {
        return ssfx.HttpVerb.toString(this._request.get_verb());
    },
    
    onCompleted: function ssfx_HttpTransport$onCompleted(response) {
        ssfx.HttpRequestManager._onCompleted(this._request, response);
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.PostHttpRequestEventArgs

ssfx.PostHttpRequestEventArgs = function ssfx_PostHttpRequestEventArgs(request, response) {
    ssfx.PostHttpRequestEventArgs.initializeBase(this);
    this._request$1 = request;
    this._response$1 = response;
}
ssfx.PostHttpRequestEventArgs.prototype = {
    _request$1: null,
    _response$1: null,
    
    get_request: function ssfx_PostHttpRequestEventArgs$get_request() {
        return this._request$1;
    },
    
    get_response: function ssfx_PostHttpRequestEventArgs$get_response() {
        return this._response$1;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.PreHttpRequestEventArgs

ssfx.PreHttpRequestEventArgs = function ssfx_PreHttpRequestEventArgs(request) {
    ssfx.PreHttpRequestEventArgs.initializeBase(this);
    this._request$1 = request;
}
ssfx.PreHttpRequestEventArgs.prototype = {
    _request$1: null,
    _response$1: null,
    _suppressed$1: false,
    
    get_isSuppressed: function ssfx_PreHttpRequestEventArgs$get_isSuppressed() {
        return this._suppressed$1;
    },
    
    get_request: function ssfx_PreHttpRequestEventArgs$get_request() {
        return this._request$1;
    },
    
    get_response: function ssfx_PreHttpRequestEventArgs$get_response() {
        return this._response$1;
    },
    
    suppressRequest: function ssfx_PreHttpRequestEventArgs$suppressRequest(response) {
        this._suppressed$1 = true;
        this._response$1 = response;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx._xmlHttpResponse

ssfx._xmlHttpResponse = function ssfx__xmlHttpResponse(request, xmlHTTP) {
    this._timeStamp = new Date();
    this._request = request;
    this._xmlHTTP = xmlHTTP;
}
ssfx._xmlHttpResponse.prototype = {
    _request: null,
    _xmlHTTP: null,
    _headers: null,
    _timeStamp: null,
    _text: null,
    _object: null,
    _xml: null,
    
    get_contentLength: function ssfx__xmlHttpResponse$get_contentLength() {
        return this.getText().length;
    },
    
    get_contentType: function ssfx__xmlHttpResponse$get_contentType() {
        return this._xmlHTTP.getResponseHeader('Content-Type');
    },
    
    get_headers: function ssfx__xmlHttpResponse$get_headers() {
        if (this._headers == null) {
            var headers = this._xmlHTTP.getAllResponseHeaders();
            var parts = headers.split('\n');
            this._headers = {};
            var $enum1 = ss.IEnumerator.getEnumerator(parts);
            while ($enum1.moveNext()) {
                var part = $enum1.get_current();
                var colonIndex = part.indexOf(':');
                this._headers[part.substr(0, colonIndex)] = part.substr(colonIndex + 1).trim();
            }
        }
        return this._headers;
    },
    
    get_request: function ssfx__xmlHttpResponse$get_request() {
        return this._request;
    },
    
    get_statusCode: function ssfx__xmlHttpResponse$get_statusCode() {
        return this._xmlHTTP.status;
    },
    
    get_statusText: function ssfx__xmlHttpResponse$get_statusText() {
        return this._xmlHTTP.statusText;
    },
    
    get_timeStamp: function ssfx__xmlHttpResponse$get_timeStamp() {
        return this._timeStamp;
    },
    
    getHeader: function ssfx__xmlHttpResponse$getHeader(name) {
        return this._xmlHTTP.getResponseHeader(name);
    },
    
    getObject: function ssfx__xmlHttpResponse$getObject() {
        if (this._object == null) {
            this._object = ssfx.JSON.deserialize(this.getText());
        }
        return this._object;
    },
    
    getText: function ssfx__xmlHttpResponse$getText() {
        if (this._text == null) {
            this._text = this._xmlHTTP.responseText;
        }
        return this._text;
    },
    
    getXml: function ssfx__xmlHttpResponse$getXml() {
        if (this._xml == null) {
            var xml = this._xmlHTTP.responseXML;
            if ((xml == null) || (xml.documentElement == null)) {
                try {
                    xml = ss.XmlDocumentParser.parse(this._xmlHTTP.responseText);
                    if ((xml != null) && (xml.documentElement != null)) {
                        this._xml = xml;
                    }
                }
                catch ($e1) {
                }
            }
            else {
                this._xml = xml;
                if (ssfx.Application.current.get_isIE()) {
                    xml.setProperty('SelectionLanguage', 'XPath');
                }
            }
        }
        return this._xml;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx._xmlHttpTransport

ssfx._xmlHttpTransport = function ssfx__xmlHttpTransport(request) {
    ssfx._xmlHttpTransport.initializeBase(this, [ request ]);
}
ssfx._xmlHttpTransport.prototype = {
    _xmlHTTP$1: null,
    
    abort: function ssfx__xmlHttpTransport$abort() {
        if (this._xmlHTTP$1 != null) {
            this._xmlHTTP$1.onreadystatechange = ss.Delegate.Null;
            this._xmlHTTP$1.abort();
            this._xmlHTTP$1 = null;
        }
    },
    
    dispose: function ssfx__xmlHttpTransport$dispose() {
        this.abort();
    },
    
    invoke: function ssfx__xmlHttpTransport$invoke() {
        var request = this.get_request();
        this._xmlHTTP$1 = new XMLHttpRequest();
        this._xmlHTTP$1.onreadystatechange = ss.Delegate.create(this, this._onReadyStateChange$1);
        if (!this.get_request().get_hasCredentials()) {
            this._xmlHTTP$1.open(this.getMethod(), request.get_URI(), true);
        }
        else {
            this._xmlHTTP$1.open(this.getMethod(), request.get_URI(), true, request.get_userName(), request.get_password());
        }
        var headers = (request.get_hasHeaders()) ? request.get_headers() : null;
        if (headers != null) {
            var $dict1 = headers;
            for (var $key2 in $dict1) {
                var entry = { key: $key2, value: $dict1[$key2] };
                this._xmlHTTP$1.setRequestHeader(entry.key, entry.value);
            }
        }
        var body = request.get_content();
        if ((body != null) && ((headers == null) || (headers['Content-Type'] == null))) {
            this._xmlHTTP$1.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        }
        this._xmlHTTP$1.send(body);
    },
    
    _onReadyStateChange$1: function ssfx__xmlHttpTransport$_onReadyStateChange$1() {
        if (this._xmlHTTP$1.readyState === 4) {
            var response = new ssfx._xmlHttpResponse(this.get_request(), this._xmlHTTP$1);
            this._xmlHTTP$1.onreadystatechange = ss.Delegate.Null;
            this._xmlHTTP$1 = null;
            this.onCompleted(response);
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.AnimationStopState

ssfx.AnimationStopState = function() { };
ssfx.AnimationStopState.prototype = {
    complete: 0, 
    abort: 1, 
    revert: 2
}
ssfx.AnimationStopState.registerEnum('ssfx.AnimationStopState', false);


////////////////////////////////////////////////////////////////////////////////
// ssfx.Bounds

ssfx.$create_Bounds = function ssfx_Bounds(left, top, width, height) {
    var $o = { };
    $o.left = left;
    $o.top = top;
    $o.width = width;
    $o.height = height;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.DragDropData

ssfx.$create_DragDropData = function ssfx_DragDropData(mode, dataType, data) {
    var $o = { };
    $o.mode = mode;
    $o.dataType = dataType;
    $o.data = data;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.DragMode

ssfx.DragMode = function() { };
ssfx.DragMode.prototype = {
    move: 0, 
    copy: 1
}
ssfx.DragMode.registerEnum('ssfx.DragMode', false);


////////////////////////////////////////////////////////////////////////////////
// ssfx.IAction

ssfx.IAction = function() { };
ssfx.IAction.prototype = {
    get_actionArgument : null,
    get_actionName : null,
    add_action : null,
    remove_action : null
}
ssfx.IAction.registerInterface('ssfx.IAction');


////////////////////////////////////////////////////////////////////////////////
// ssfx.IDragDrop

ssfx.IDragDrop = function() { };
ssfx.IDragDrop.prototype = {
    get_supportsDataTransfer : null,
    dragDrop : null
}
ssfx.IDragDrop.registerInterface('ssfx.IDragDrop');


////////////////////////////////////////////////////////////////////////////////
// ssfx.IDragSource

ssfx.IDragSource = function() { };
ssfx.IDragSource.prototype = {
    get_element : null,
    onDragStart : null,
    onDrag : null,
    onDragEnd : null
}
ssfx.IDragSource.registerInterface('ssfx.IDragSource');


////////////////////////////////////////////////////////////////////////////////
// ssfx.IDropTarget

ssfx.IDropTarget = function() { };
ssfx.IDropTarget.prototype = {
    get_element : null,
    supportsDataObject : null,
    drop : null,
    onDragEnter : null,
    onDragLeave : null,
    onDragOver : null
}
ssfx.IDropTarget.registerInterface('ssfx.IDropTarget');


////////////////////////////////////////////////////////////////////////////////
// ssfx.IEditableText

ssfx.IEditableText = function() { };
ssfx.IEditableText.prototype = {
    get_text : null,
    set_text : null,
    add_textChanged : null,
    remove_textChanged : null
}
ssfx.IEditableText.registerInterface('ssfx.IEditableText');


////////////////////////////////////////////////////////////////////////////////
// ssfx.IStaticText

ssfx.IStaticText = function() { };
ssfx.IStaticText.prototype = {
    get_text : null
}
ssfx.IStaticText.registerInterface('ssfx.IStaticText');


////////////////////////////////////////////////////////////////////////////////
// ssfx.IToggle

ssfx.IToggle = function() { };
ssfx.IToggle.prototype = {
    get_checked : null,
    add_checkedChanged : null,
    remove_checkedChanged : null
}
ssfx.IToggle.registerInterface('ssfx.IToggle');


////////////////////////////////////////////////////////////////////////////////
// ssfx.IValidator

ssfx.IValidator = function() { };
ssfx.IValidator.prototype = {
    get_isValid : null,
    get_validationGroup : null
}
ssfx.IValidator.registerInterface('ssfx.IValidator');


////////////////////////////////////////////////////////////////////////////////
// ssfx.Key

ssfx.Key = function() { };
ssfx.Key.prototype = {
    backspace: 8, 
    tab: 9, 
    enter: 13, 
    escape: 27, 
    space: 32, 
    pageUp: 33, 
    pageDown: 34, 
    end: 35, 
    home: 36, 
    left: 37, 
    up: 38, 
    right: 39, 
    down: 40, 
    del: 127
}
ssfx.Key.registerEnum('ssfx.Key', false);


////////////////////////////////////////////////////////////////////////////////
// ssfx.Location

ssfx.$create_Location = function ssfx_Location(left, top) {
    var $o = { };
    $o.left = left;
    $o.top = top;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.OverlayOptions

ssfx.$create_OverlayOptions = function ssfx_OverlayOptions(cssClass) {
    var $o = { };
    $o.cssClass = cssClass;
    $o.fadeInOutInterval = 250;
    $o.opacity = 0.75;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.PopupMode

ssfx.PopupMode = function() { };
ssfx.PopupMode.prototype = {
    center: 0, 
    anchorTopLeft: 1, 
    anchorTopRight: 2, 
    anchorBottomRight: 3, 
    anchorBottomLeft: 4, 
    alignTopLeft: 5, 
    alignTopRight: 6, 
    alignBottomRight: 7, 
    alignBottomLeft: 8
}
ssfx.PopupMode.registerEnum('ssfx.PopupMode', false);


////////////////////////////////////////////////////////////////////////////////
// ssfx.PopupOptions

ssfx.$create_PopupOptions = function ssfx_PopupOptions(referenceElement, mode) {
    var $o = { };
    $o.referenceElement = referenceElement;
    $o.mode = mode;
    $o.id = null;
    $o.xOffset = 0;
    $o.yOffset = 0;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.Size

ssfx.$create_Size = function ssfx_Size(width, height) {
    var $o = { };
    $o.width = width;
    $o.height = height;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.Animation

ssfx.Animation = function ssfx_Animation(element) {
    if (element == null) {
        element = document.documentElement;
    }
    this._element = element;
    this._repeatCount = 1;
    ssfx.Application.current.registerDisposableObject(this);
}
ssfx.Animation.prototype = {
    _element: null,
    _repeatCount: 0,
    _autoReverse: false,
    _repeatDelay: 0,
    _completed: false,
    _isPlaying: false,
    _isRepeating: false,
    _repetitions: 0,
    _repeatTimeStamp: 0,
    _reversed: false,
    
    add_repeating: function ssfx_Animation$add_repeating(value) {
        this.__repeating = ss.Delegate.combine(this.__repeating, value);
    },
    remove_repeating: function ssfx_Animation$remove_repeating(value) {
        this.__repeating = ss.Delegate.remove(this.__repeating, value);
    },
    
    __repeating: null,
    
    add_starting: function ssfx_Animation$add_starting(value) {
        this.__starting = ss.Delegate.combine(this.__starting, value);
    },
    remove_starting: function ssfx_Animation$remove_starting(value) {
        this.__starting = ss.Delegate.remove(this.__starting, value);
    },
    
    __starting: null,
    
    add_stopped: function ssfx_Animation$add_stopped(value) {
        this.__stopped = ss.Delegate.combine(this.__stopped, value);
    },
    remove_stopped: function ssfx_Animation$remove_stopped(value) {
        this.__stopped = ss.Delegate.remove(this.__stopped, value);
    },
    
    __stopped: null,
    
    get_autoReverse: function ssfx_Animation$get_autoReverse() {
        return this._autoReverse;
    },
    set_autoReverse: function ssfx_Animation$set_autoReverse(value) {
        ss.Debug.assert(!this.get_isPlaying());
        this._autoReverse = value;
        return value;
    },
    
    get_completed: function ssfx_Animation$get_completed() {
        return this._completed;
    },
    
    get_element: function ssfx_Animation$get_element() {
        return this._element;
    },
    
    get_isPlaying: function ssfx_Animation$get_isPlaying() {
        return this._isPlaying;
    },
    
    get_isReversed: function ssfx_Animation$get_isReversed() {
        return this._reversed;
    },
    
    get_repeatCount: function ssfx_Animation$get_repeatCount() {
        return this._repeatCount;
    },
    set_repeatCount: function ssfx_Animation$set_repeatCount(value) {
        ss.Debug.assert(!this.get_isPlaying());
        ss.Debug.assert(value >= 0);
        this._repeatCount = value;
        return value;
    },
    
    get_repeatDelay: function ssfx_Animation$get_repeatDelay() {
        return this._repeatDelay;
    },
    set_repeatDelay: function ssfx_Animation$set_repeatDelay(value) {
        ss.Debug.assert(!this.get_isPlaying());
        ss.Debug.assert(value >= 0);
        this._repeatDelay = value;
        return value;
    },
    
    get_repetitions: function ssfx_Animation$get_repetitions() {
        return this._repetitions;
    },
    
    dispose: function ssfx_Animation$dispose() {
        if (this._isPlaying) {
            this.stop(ssfx.AnimationStopState.abort);
        }
        if (this._element != null) {
            this._element = null;
            ssfx.Application.current.unregisterDisposableObject(this);
        }
    },
    
    _onPlay: function ssfx_Animation$_onPlay(reversed) {
        if (this.__starting != null) {
            this.__starting.invoke(this, ss.EventArgs.Empty);
        }
        this.performSetup();
        this._isPlaying = true;
        this._repetitions = 1;
        this._reversed = reversed;
        this.playCore();
    },
    
    _onStop: function ssfx_Animation$_onStop(completed, stopState) {
        this.stopCore(completed, stopState);
        this._completed = completed;
        this._isPlaying = false;
        this.performCleanup();
        if (this.__stopped != null) {
            this.__stopped.invoke(this, ss.EventArgs.Empty);
        }
    },
    
    _onProgress: function ssfx_Animation$_onProgress(timeStamp) {
        if (this._isRepeating) {
            if ((this._repeatDelay !== 0) && ((this._repeatTimeStamp + this._repeatDelay) > timeStamp)) {
                return false;
            }
        }
        var completed = this.progressCore(this._isRepeating, timeStamp);
        this._isRepeating = false;
        if (completed && ((this._repeatCount === 0) || (this._repeatCount > this._repetitions))) {
            completed = false;
            this._repetitions++;
            if (this.__repeating != null) {
                var ce = new ss.CancelEventArgs();
                this.__repeating.invoke(this, ce);
                completed = ce.get_cancel();
            }
            if (!completed) {
                this._isRepeating = true;
                if (this._autoReverse) {
                    this._reversed = !this._reversed;
                }
                this._repeatTimeStamp = timeStamp;
                this.performRepetition(this._reversed);
            }
        }
        return completed;
    },
    
    performCleanup: function ssfx_Animation$performCleanup() {
    },
    
    performRepetition: function ssfx_Animation$performRepetition(reversed) {
    },
    
    performSetup: function ssfx_Animation$performSetup() {
    },
    
    play: function ssfx_Animation$play() {
        ss.Debug.assert(!this.get_isPlaying());
        this._completed = false;
        ssfx.AnimationManager._play(this);
    },
    
    stop: function ssfx_Animation$stop(stopState) {
        ss.Debug.assert(this.get_isPlaying());
        ssfx.AnimationManager._stop(this, stopState);
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.AnimationManager

ssfx.AnimationManager = function ssfx_AnimationManager() {
}
ssfx.AnimationManager.get_FPS = function ssfx_AnimationManager$get_FPS() {
    return ssfx.AnimationManager._fps;
}
ssfx.AnimationManager.set_FPS = function ssfx_AnimationManager$set_FPS(value) {
    ss.Debug.assert((value > 0) && (value <= 100));
    ssfx.AnimationManager._fps = value;
    return value;
}
ssfx.AnimationManager._onTick = function ssfx_AnimationManager$_onTick() {
    ssfx.AnimationManager._timerCookie = 0;
    if (ssfx.AnimationManager._activeAnimations.length === 0) {
        return;
    }
    var timeStamp = new Date().getTime();
    var currentAnimations = ssfx.AnimationManager._activeAnimations;
    var newAnimations = [];
    ssfx.AnimationManager._activeAnimations = null;
    var $enum1 = ss.IEnumerator.getEnumerator(currentAnimations);
    while ($enum1.moveNext()) {
        var animation = $enum1.get_current();
        var completed = animation._onProgress(timeStamp);
        if (completed) {
            animation._onStop(true, ssfx.AnimationStopState.complete);
        }
        else {
            newAnimations.add(animation);
        }
    }
    if (newAnimations.length !== 0) {
        ssfx.AnimationManager._activeAnimations = newAnimations;
        if (ssfx.AnimationManager._timerCookie === 0) {
            ssfx.AnimationManager._timerCookie = window.setTimeout(ssfx.AnimationManager._onTick, 1000 / ssfx.AnimationManager._fps);
        }
    }
}
ssfx.AnimationManager._play = function ssfx_AnimationManager$_play(animation) {
    if (ssfx.AnimationManager._activeAnimations == null) {
        ssfx.AnimationManager._activeAnimations = [];
    }
    ssfx.AnimationManager._activeAnimations.add(animation);
    animation._onPlay(false);
    if (ssfx.AnimationManager._timerCookie === 0) {
        ssfx.AnimationManager._timerCookie = window.setTimeout(ssfx.AnimationManager._onTick, 1000 / ssfx.AnimationManager._fps);
    }
}
ssfx.AnimationManager._stop = function ssfx_AnimationManager$_stop(animation, stopState) {
    ss.Debug.assert(ssfx.AnimationManager._activeAnimations != null);
    animation._onStop(false, stopState);
    ssfx.AnimationManager._activeAnimations.remove(animation);
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.AnimationSequence

ssfx.AnimationSequence = function ssfx_AnimationSequence(animations) {
    ssfx.AnimationSequence.initializeBase(this, [ null ]);
    ss.Debug.assert((animations != null) && (animations.length > 1));
    this._animations$1 = animations;
    this._current$1 = -1;
}
ssfx.AnimationSequence.prototype = {
    _animations$1: null,
    _successionDelay$1: 0,
    _current$1: 0,
    _nextAnimation$1: false,
    _successionTimeStamp$1: 0,
    
    get_successionDelay: function ssfx_AnimationSequence$get_successionDelay() {
        return this._successionDelay$1;
    },
    set_successionDelay: function ssfx_AnimationSequence$set_successionDelay(value) {
        ss.Debug.assert(!this.get_isPlaying());
        ss.Debug.assert(value >= 0);
        this._successionDelay$1 = value;
        return value;
    },
    
    playCore: function ssfx_AnimationSequence$playCore() {
        ss.Debug.assert(this._current$1 === -1);
        if (!this.get_isReversed()) {
            this._current$1 = 0;
        }
        else {
            this._current$1 = this._animations$1.length - 1;
        }
        this._animations$1[this._current$1]._onPlay(this.get_isReversed());
    },
    
    progressCore: function ssfx_AnimationSequence$progressCore(startRepetition, timeStamp) {
        if (startRepetition) {
            if (!this.get_isReversed()) {
                this._current$1 = 0;
            }
            else {
                this._current$1 = this._animations$1.length - 1;
            }
            this._nextAnimation$1 = true;
        }
        var animation = this._animations$1[this._current$1];
        if (this._nextAnimation$1) {
            if ((this._successionDelay$1 !== 0) && ((this._successionTimeStamp$1 + this._successionDelay$1) > timeStamp)) {
                return false;
            }
            this._nextAnimation$1 = false;
            animation._onPlay(this.get_isReversed());
        }
        var completed = animation._onProgress(timeStamp);
        if (completed) {
            animation._onStop(true, ssfx.AnimationStopState.complete);
            if (!this.get_isReversed()) {
                this._current$1++;
            }
            else {
                this._current$1--;
            }
            this._nextAnimation$1 = true;
            this._successionTimeStamp$1 = timeStamp;
        }
        return completed && ((this._current$1 === this._animations$1.length) || (this._current$1 === -1));
    },
    
    stopCore: function ssfx_AnimationSequence$stopCore(completed, stopState) {
        if (!completed) {
            var animation = this._animations$1[this._current$1];
            animation._onStop(false, stopState);
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.Behavior

ssfx.Behavior = function ssfx_Behavior(element, id) {
    ss.Debug.assert(element != null);
    ssfx.Application.current.registerDisposableObject(this);
    this._element = element;
    this._id = id;
    if (!String.isNullOrEmpty(id)) {
        if (id === 'control') {
            var existingControl = element[id];
            if ((existingControl != null) && (Type.getInstanceType(existingControl) === ssfx._genericControl)) {
                delete element.control;
                ssfx.Application.current.unregisterDisposableObject(existingControl);
                this._events = existingControl.get__eventsInternal();
            }
        }
        ss.Debug.assert(ss.isUndefined(element[id]));
        element[id] = this;
    }
    if (id !== 'control') {
        var existingControl = element.control;
        if (existingControl == null) {
            existingControl = new ssfx._genericControl(element);
        }
    }
    var behaviors = element._behaviors;
    if (behaviors == null) {
        behaviors = [];
        element._behaviors = behaviors;
    }
    behaviors.add(this);
}
ssfx.Behavior.getBehavior = function ssfx_Behavior$getBehavior(element, type) {
    ss.Debug.assert(element != null);
    ss.Debug.assert(type != null);
    var behaviors = element._behaviors;
    if (behaviors != null) {
        var $enum1 = ss.IEnumerator.getEnumerator(behaviors);
        while ($enum1.moveNext()) {
            var behavior = $enum1.get_current();
            if (type.isAssignableFrom(Type.getInstanceType(behavior))) {
                return behavior;
            }
        }
    }
    return null;
}
ssfx.Behavior.getBehaviors = function ssfx_Behavior$getBehaviors(element, type) {
    ss.Debug.assert(element != null);
    var behaviors = element._behaviors;
    if (ss.isNullOrUndefined(behaviors) || (behaviors.length === 0)) {
        return null;
    }
    if (type == null) {
        return behaviors.clone();
    }
    return behaviors.filter(function(behavior) {
        return type.isAssignableFrom(Type.getInstanceType(behavior));
    });
}
ssfx.Behavior.getNamedBehavior = function ssfx_Behavior$getNamedBehavior(element, id) {
    ss.Debug.assert(element != null);
    ss.Debug.assert(!String.isNullOrEmpty(id));
    return element[id];
}
ssfx.Behavior.prototype = {
    _element: null,
    _id: null,
    _elementEvents: null,
    _events: null,
    _initializing: false,
    
    get_element: function ssfx_Behavior$get_element() {
        return this._element;
    },
    
    get_elementEvents: function ssfx_Behavior$get_elementEvents() {
        if (this._elementEvents == null) {
            this._elementEvents = new ssfx.ElementEventList(this._element);
        }
        return this._elementEvents;
    },
    
    get_events: function ssfx_Behavior$get_events() {
        if (this._events == null) {
            this._events = new ssfx.EventList();
        }
        return this._events;
    },
    
    get__eventsInternal: function ssfx_Behavior$get__eventsInternal() {
        return this._events;
    },
    
    get_isDisposed: function ssfx_Behavior$get_isDisposed() {
        return (this._element == null);
    },
    
    get_isInitializing: function ssfx_Behavior$get_isInitializing() {
        return this._initializing;
    },
    
    add_propertyChanged: function ssfx_Behavior$add_propertyChanged(value) {
        this.get_events().addHandler('PropertyChanged', value);
    },
    remove_propertyChanged: function ssfx_Behavior$remove_propertyChanged(value) {
        this.get_events().removeHandler('PropertyChanged', value);
    },
    
    beginInitialize: function ssfx_Behavior$beginInitialize() {
        this._initializing = true;
    },
    
    dispose: function ssfx_Behavior$dispose() {
        if (this._elementEvents != null) {
            this._elementEvents.dispose();
        }
        if (this._element != null) {
            if (this._id != null) {
                if (ssfx.Application.current.get_isIE()) {
                    this._element.removeAttribute(this._id);
                }
                else {
                    delete this._element[this._id];
                }
            }
            var behaviors = this._element._behaviors;
            ss.Debug.assert(behaviors != null);
            behaviors.remove(this);
            this._element = null;
            ssfx.Application.current.unregisterDisposableObject(this);
        }
    },
    
    endInitialize: function ssfx_Behavior$endInitialize() {
        this._initializing = false;
    },
    
    raisePropertyChanged: function ssfx_Behavior$raisePropertyChanged(propertyName) {
        var propChangedHandler = this.get_events().getHandler('PropertyChanged');
        if (propChangedHandler != null) {
            propChangedHandler.invoke(this, new ss.PropertyChangedEventArgs(propertyName));
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.Color

ssfx.Color = function ssfx_Color(red, green, blue) {
    ss.Debug.assert(red >= 0 && red <= 255);
    ss.Debug.assert(green >= 0 && green <= 255);
    ss.Debug.assert(blue >= 0 && blue <= 255);
    this._red = red;
    this._green = green;
    this._blue = blue;
}
ssfx.Color.format = function ssfx_Color$format(red, green, blue) {
    return String.format('#{0:X2}{1:X2}{2:X2}', red, green, blue);
}
ssfx.Color.parse = function ssfx_Color$parse(s) {
    if (String.isNullOrEmpty(s)) {
        return null;
    }
    if ((s.length === 7) && s.startsWith('#')) {
        var red = parseInt(s.substr(1, 2), 16);
        var green = parseInt(s.substr(3, 2), 16);
        var blue = parseInt(s.substr(5, 2), 16);
        return new ssfx.Color(red, green, blue);
    }
    else if (s.startsWith('rgb(') && s.endsWith(')')) {
        var parts = s.substring(4, s.length - 1).split(',');
        if (parts.length === 3) {
            return new ssfx.Color(parseInt(parts[0].trim()), parseInt(parts[1].trim()), parseInt(parts[2].trim()));
        }
    }
    return null;
}
ssfx.Color.prototype = {
    _red: 0,
    _green: 0,
    _blue: 0,
    
    get_blue: function ssfx_Color$get_blue() {
        return this._blue;
    },
    
    get_green: function ssfx_Color$get_green() {
        return this._green;
    },
    
    get_red: function ssfx_Color$get_red() {
        return this._red;
    },
    
    toString: function ssfx_Color$toString() {
        return ssfx.Color.format(this._red, this._green, this._blue);
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.Control

ssfx.Control = function ssfx_Control(element) {
    ssfx.Control.initializeBase(this, [ element, 'control' ]);
}
ssfx.Control.getControl = function ssfx_Control$getControl(element) {
    return ssfx.Behavior.getNamedBehavior(element, 'control');
}
ssfx.Control.prototype = {
    
    add_disposing: function ssfx_Control$add_disposing(value) {
        this.get_events().addHandler('disposing', value);
    },
    remove_disposing: function ssfx_Control$remove_disposing(value) {
        this.get_events().removeHandler('disposing', value);
    },
    
    dispose: function ssfx_Control$dispose() {
        var element = this.get_element();
        if (element != null) {
            var disposingHandler = this.get_events().getHandler('disposing');
            if (disposingHandler != null) {
                disposingHandler.invoke(this, ss.EventArgs.Empty);
            }
            var behaviors = ssfx.Behavior.getBehaviors(element, null);
            ss.Debug.assert((behaviors != null) && (behaviors.length > 0));
            if (behaviors.length > 1) {
                var $enum1 = ss.IEnumerator.getEnumerator(behaviors);
                while ($enum1.moveNext()) {
                    var behavior = $enum1.get_current();
                    if (behavior !== this) {
                        behavior.dispose();
                    }
                }
            }
        }
        ssfx.Control.callBaseMethod(this, 'dispose');
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.ElementEventList

ssfx.ElementEventList = function ssfx_ElementEventList(element) {
    ss.Debug.assert(element != null);
    this._element = element;
    this._handlers = {};
}
ssfx.ElementEventList.prototype = {
    _element: null,
    _handlers: null,
    
    attach: function ssfx_ElementEventList$attach(eventName, handler) {
        ss.Debug.assert(this._element != null);
        ss.Debug.assert(!String.isNullOrEmpty(eventName));
        ss.Debug.assert(handler != null);
        ss.Debug.assert(!this.isAttached(eventName));
        this._element.attachEvent(eventName, handler);
        this._handlers[eventName] = handler;
    },
    
    detach: function ssfx_ElementEventList$detach(eventName) {
        ss.Debug.assert(this._element != null);
        ss.Debug.assert(!String.isNullOrEmpty(eventName));
        var handler = this._handlers[eventName];
        if (handler != null) {
            this._element.detachEvent(eventName, handler);
            delete this._handlers[eventName];
            return true;
        }
        return false;
    },
    
    dispose: function ssfx_ElementEventList$dispose() {
        if (this._element != null) {
            var $dict1 = this._handlers;
            for (var $key2 in $dict1) {
                var e = { key: $key2, value: $dict1[$key2] };
                this._element.detachEvent(e.key, e.value);
            }
            this._element = null;
            this._handlers = null;
        }
    },
    
    isAttached: function ssfx_ElementEventList$isAttached(eventName) {
        ss.Debug.assert(this._element != null);
        ss.Debug.assert(!String.isNullOrEmpty(eventName));
        return (this._handlers[eventName] != null) ? true : false;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.DragDropEventArgs

ssfx.DragDropEventArgs = function ssfx_DragDropEventArgs(dataObject) {
    ssfx.DragDropEventArgs.initializeBase(this);
    this._dataObject$1 = dataObject;
}
ssfx.DragDropEventArgs.prototype = {
    _dataObject$1: null,
    
    get_dataObject: function ssfx_DragDropEventArgs$get_dataObject() {
        return this._dataObject$1;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.DragDropManager

ssfx.DragDropManager = function ssfx_DragDropManager() {
}
ssfx.DragDropManager.get_canDragDrop = function ssfx_DragDropManager$get_canDragDrop() {
    return (ssfx.DragDropManager._dragDropImplementation != null);
}
ssfx.DragDropManager.get_supportsDataTransfer = function ssfx_DragDropManager$get_supportsDataTransfer() {
    ss.Debug.assert(ssfx.DragDropManager.get_canDragDrop());
    return ssfx.DragDropManager._dragDropImplementation.get_supportsDataTransfer();
}
ssfx.DragDropManager.add_dragDropEnding = function ssfx_DragDropManager$add_dragDropEnding(value) {
    ssfx.DragDropManager._dragEndingHandler = ss.Delegate.combine(ssfx.DragDropManager._dragEndingHandler, value);
}
ssfx.DragDropManager.remove_dragDropEnding = function ssfx_DragDropManager$remove_dragDropEnding(value) {
    ssfx.DragDropManager._dragEndingHandler = ss.Delegate.remove(ssfx.DragDropManager._dragEndingHandler, value);
}
ssfx.DragDropManager.add_dragDropStarting = function ssfx_DragDropManager$add_dragDropStarting(value) {
    ssfx.DragDropManager._dragStartingHandler = ss.Delegate.combine(ssfx.DragDropManager._dragStartingHandler, value);
}
ssfx.DragDropManager.remove_dragDropStarting = function ssfx_DragDropManager$remove_dragDropStarting(value) {
    ssfx.DragDropManager._dragStartingHandler = ss.Delegate.remove(ssfx.DragDropManager._dragStartingHandler, value);
}
ssfx.DragDropManager._endDragDrop = function ssfx_DragDropManager$_endDragDrop() {
    if (ssfx.DragDropManager._dragEndingHandler != null) {
        ssfx.DragDropManager._dragEndingHandler.invoke(null, new ssfx.DragDropEventArgs(ssfx.DragDropManager._currentDataObject));
    }
    ssfx.DragDropManager._currentDataObject = null;
}
ssfx.DragDropManager.registerDragDropImplementation = function ssfx_DragDropManager$registerDragDropImplementation(dragDrop) {
    ssfx.DragDropManager._dragDropImplementation = dragDrop;
}
ssfx.DragDropManager.registerDropTarget = function ssfx_DragDropManager$registerDropTarget(target) {
    ssfx.DragDropManager._dropTargets.add(target);
}
ssfx.DragDropManager.startDragDrop = function ssfx_DragDropManager$startDragDrop(data, dragVisual, dragOffset, source, context) {
    ss.Debug.assert(ssfx.DragDropManager.get_canDragDrop());
    if (ssfx.DragDropManager._currentDataObject != null) {
        return false;
    }
    var validDropTargets = [];
    var $enum1 = ss.IEnumerator.getEnumerator(ssfx.DragDropManager._dropTargets);
    while ($enum1.moveNext()) {
        var dropTarget = $enum1.get_current();
        if (dropTarget.supportsDataObject(data)) {
            validDropTargets.add(dropTarget);
        }
    }
    if (validDropTargets.length === 0) {
        return false;
    }
    ssfx.DragDropManager._currentDataObject = data;
    if (ssfx.DragDropManager._dragStartingHandler != null) {
        ssfx.DragDropManager._dragStartingHandler.invoke(null, new ssfx.DragDropEventArgs(data));
    }
    ssfx.DragDropManager._dragDropImplementation.dragDrop(new ssfx._dragDropTracker(source), context, validDropTargets, dragVisual, dragOffset, ssfx.DragDropManager._currentDataObject);
    return true;
}
ssfx.DragDropManager.unregisterDropTarget = function ssfx_DragDropManager$unregisterDropTarget(target) {
    ssfx.DragDropManager._dropTargets.remove(target);
}


////////////////////////////////////////////////////////////////////////////////
// ssfx._dragDropTracker

ssfx._dragDropTracker = function ssfx__dragDropTracker(actualSource) {
    this._actualSource = actualSource;
}
ssfx._dragDropTracker.prototype = {
    _actualSource: null,
    
    get_element: function ssfx__dragDropTracker$get_element() {
        return this._actualSource.get_element();
    },
    
    onDragStart: function ssfx__dragDropTracker$onDragStart(context) {
        if (this._actualSource != null) {
            this._actualSource.onDragStart(context);
        }
    },
    
    onDrag: function ssfx__dragDropTracker$onDrag(context) {
        if (this._actualSource != null) {
            this._actualSource.onDrag(context);
        }
    },
    
    onDragEnd: function ssfx__dragDropTracker$onDragEnd(canceled, context) {
        if (this._actualSource != null) {
            this._actualSource.onDragEnd(canceled, context);
        }
        ssfx.DragDropManager._endDragDrop();
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.ElementHelper

ssfx.ElementHelper = function ssfx_ElementHelper() {
}
ssfx.ElementHelper.addCSSClass = function ssfx_ElementHelper$addCSSClass(element, className) {
    var cssClass = element.className;
    if (cssClass.indexOf(className) < 0) {
        element.className = cssClass + ' ' + className;
    }
}
ssfx.ElementHelper.containsCSSClass = function ssfx_ElementHelper$containsCSSClass(element, className) {
    return element.className.split(' ').contains(className);
}
ssfx.ElementHelper.getBounds = function ssfx_ElementHelper$getBounds(element) {
    var location = ssfx.ElementHelper.getLocation(element);
    return ssfx.$create_Bounds(location.left, location.top, element.offsetWidth, element.offsetHeight);
}
ssfx.ElementHelper.getLocation = function ssfx_ElementHelper$getLocation(element) {
    var offsetX = 0;
    var offsetY = 0;
    for (var parentElement = element; parentElement != null; parentElement = parentElement.offsetParent) {
        offsetX += parentElement.offsetLeft;
        offsetY += parentElement.offsetTop;
    }
    return ssfx.$create_Location(offsetX, offsetY);
}
ssfx.ElementHelper.getSize = function ssfx_ElementHelper$getSize(element) {
    return ssfx.$create_Size(element.offsetWidth, element.offsetHeight);
}
ssfx.ElementHelper.removeCSSClass = function ssfx_ElementHelper$removeCSSClass(element, className) {
    var cssClass = ' ' + element.className + ' ';
    var index = cssClass.indexOf(' ' + className + ' ');
    if (index >= 0) {
        var newClass = cssClass.substr(0, index) + ' ' + cssClass.substr(index + className.length + 1);
        element.className = newClass;
    }
}
ssfx.ElementHelper.setLocation = function ssfx_ElementHelper$setLocation(element, location) {
    element.style.left = location.left + 'px';
    element.style.top = location.top + 'px';
}
ssfx.ElementHelper.setSize = function ssfx_ElementHelper$setSize(element, size) {
    element.style.width = size.width + 'px';
    element.style.height = size.height + 'px';
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.FadeEffect

ssfx.FadeEffect = function ssfx_FadeEffect(element, duration, opacity) {
    ssfx.FadeEffect.initializeBase(this, [ element, duration ]);
    this._opacity$2 = opacity;
}
ssfx.FadeEffect.prototype = {
    _fadingIn$2: false,
    _opacity$2: 0,
    
    get_isFadingIn: function ssfx_FadeEffect$get_isFadingIn() {
        return this._fadingIn$2;
    },
    
    fadeIn: function ssfx_FadeEffect$fadeIn() {
        if (this.get_isPlaying()) {
            this.stop(ssfx.AnimationStopState.complete);
        }
        this._fadingIn$2 = true;
        this.play();
    },
    
    fadeOut: function ssfx_FadeEffect$fadeOut() {
        if (this.get_isPlaying()) {
            this.stop(ssfx.AnimationStopState.complete);
        }
        this._fadingIn$2 = false;
        this.play();
    },
    
    performCleanup: function ssfx_FadeEffect$performCleanup() {
        ssfx.FadeEffect.callBaseMethod(this, 'performCleanup');
        if (!this._fadingIn$2) {
            this._setOpacity$2(0);
            this.get_element().style.display = 'none';
        }
    },
    
    performSetup: function ssfx_FadeEffect$performSetup() {
        ssfx.FadeEffect.callBaseMethod(this, 'performSetup');
        if (this._fadingIn$2) {
            this._setOpacity$2(0);
            this.get_element().style.display = '';
        }
    },
    
    performTweening: function ssfx_FadeEffect$performTweening(frame) {
        if (this._fadingIn$2) {
            this._setOpacity$2(this._opacity$2 * frame);
        }
        else {
            this._setOpacity$2(this._opacity$2 * (1 - frame));
        }
    },
    
    _setOpacity$2: function ssfx_FadeEffect$_setOpacity$2(opacity) {
        if (ssfx.Application.current.get_isIE()) {
            this.get_element().style.filter = 'alpha(opacity=' + (opacity * 100) + ')';
        }
        else {
            this.get_element().style.opacity = opacity.toString();
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx._genericControl

ssfx._genericControl = function ssfx__genericControl(element) {
    ssfx._genericControl.initializeBase(this, [ element ]);
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.OverlayBehavior

ssfx.OverlayBehavior = function ssfx_OverlayBehavior(element, options) {
    ssfx.OverlayBehavior.initializeBase(this, [ element, options.id ]);
    this._overlayElement$1 = document.createElement('div');
    this._overlayElement$1.className = options.cssClass;
    var overlayStyle = this._overlayElement$1.style;
    overlayStyle.display = 'none';
    overlayStyle.top = '0px';
    overlayStyle.left = '0px';
    overlayStyle.width = '100%';
    if (ssfx.Application.current.get_isIE() && (ssfx.Application.current.get_host().get_majorVersion() < 7)) {
        overlayStyle.position = 'absolute';
    }
    else {
        this._fixedOverlayElement$1 = true;
        overlayStyle.position = 'fixed';
        overlayStyle.height = '100%';
    }
    document.body.appendChild(this._overlayElement$1);
    if (options.fadeInOutInterval !== 0) {
        this._fade$1 = new ssfx.FadeEffect(this._overlayElement$1, options.fadeInOutInterval, options.opacity);
        this._fade$1.set_easingFunction(ssfx.TimedAnimation.easeInOut);
        this._fade$1.add_stopped(ss.Delegate.create(this, this._onAnimationStopped$1));
    }
}
ssfx.OverlayBehavior.prototype = {
    _overlayElement$1: null,
    _fixedOverlayElement$1: false,
    _fade$1: null,
    _resizeHandler$1: null,
    _visible$1: false,
    
    get_isVisible: function ssfx_OverlayBehavior$get_isVisible() {
        return this._visible$1;
    },
    
    add_visibilityChanged: function ssfx_OverlayBehavior$add_visibilityChanged(value) {
        this.get_events().addHandler(ssfx.OverlayBehavior._visibilityChangedEventKey$1, value);
    },
    remove_visibilityChanged: function ssfx_OverlayBehavior$remove_visibilityChanged(value) {
        this.get_events().removeHandler(ssfx.OverlayBehavior._visibilityChangedEventKey$1, value);
    },
    
    dispose: function ssfx_OverlayBehavior$dispose() {
        if (this._fade$1 != null) {
            this._fade$1.dispose();
            this._fade$1 = null;
        }
        if (this._resizeHandler$1 != null) {
            window.detachEvent('onresize', this._resizeHandler$1);
            this._resizeHandler$1 = null;
        }
        ssfx.OverlayBehavior.callBaseMethod(this, 'dispose');
    },
    
    hide: function ssfx_OverlayBehavior$hide() {
        if ((!this._visible$1) || this._fade$1.get_isPlaying()) {
            return;
        }
        if (this._resizeHandler$1 != null) {
            window.detachEvent('onresize', this._resizeHandler$1);
            this._resizeHandler$1 = null;
        }
        if (this._fade$1 != null) {
            this._fade$1.fadeOut();
        }
        else {
            this._overlayElement$1.style.display = 'none';
            this._visible$1 = false;
            var handler = this.get_events().getHandler(ssfx.OverlayBehavior._visibilityChangedEventKey$1);
            if (handler != null) {
                handler.invoke(this, ss.EventArgs.Empty);
            }
        }
    },
    
    _onAnimationStopped$1: function ssfx_OverlayBehavior$_onAnimationStopped$1(sender, e) {
        this._visible$1 = this._fade$1.get_isFadingIn();
        var handler = this.get_events().getHandler(ssfx.OverlayBehavior._visibilityChangedEventKey$1);
        if (handler != null) {
            handler.invoke(this, ss.EventArgs.Empty);
        }
    },
    
    _onWindowResize$1: function ssfx_OverlayBehavior$_onWindowResize$1() {
        this._overlayElement$1.style.height = document.documentElement.offsetHeight + 'px';
    },
    
    show: function ssfx_OverlayBehavior$show() {
        if (this._visible$1 || this._fade$1.get_isPlaying()) {
            return;
        }
        if (!this._fixedOverlayElement$1) {
            this._overlayElement$1.style.height = document.documentElement.offsetHeight + 'px';
            this._resizeHandler$1 = ss.Delegate.create(this, this._onWindowResize$1);
            window.attachEvent('onresize', this._resizeHandler$1);
        }
        if (this._fade$1 != null) {
            this._fade$1.fadeIn();
        }
        else {
            this._overlayElement$1.style.display = '';
            this._visible$1 = true;
            var handler = this.get_events().getHandler(ssfx.OverlayBehavior._visibilityChangedEventKey$1);
            if (handler != null) {
                handler.invoke(this, ss.EventArgs.Empty);
            }
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.PopupBehavior

ssfx.PopupBehavior = function ssfx_PopupBehavior(element, options) {
    ssfx.PopupBehavior.initializeBase(this, [ element, options.id ]);
    this._options$1 = options;
    element.style.position = 'absolute';
    element.style.display = 'none';
}
ssfx.PopupBehavior.prototype = {
    _options$1: null,
    _iframe$1: null,
    
    dispose: function ssfx_PopupBehavior$dispose() {
        if (this.get_element() != null) {
            this.hide();
        }
        ssfx.PopupBehavior.callBaseMethod(this, 'dispose');
    },
    
    hide: function ssfx_PopupBehavior$hide() {
        this.get_element().style.display = 'none';
        if (this._iframe$1 != null) {
            this._iframe$1.parentNode.removeChild(this._iframe$1);
            this._iframe$1 = null;
        }
    },
    
    show: function ssfx_PopupBehavior$show() {
        var parentElement = this.get_element().offsetParent;
        if (parentElement == null) {
            parentElement = document.documentElement;
        }
        this.get_element().style.display = 'block';
        var x = 0;
        var y = 0;
        var xOffsetDirection = 1;
        var yOffsetDirection = 1;
        var alignment = false;
        var parentBounds = ssfx.ElementHelper.getBounds(parentElement);
        var elementBounds = ssfx.ElementHelper.getBounds(this.get_element());
        var referenceBounds = ssfx.ElementHelper.getBounds(this._options$1.referenceElement);
        var xDelta = referenceBounds.left - parentBounds.left;
        var yDelta = referenceBounds.top - parentBounds.top;
        switch (this._options$1.mode) {
            case ssfx.PopupMode.center:
                x = Math.round(referenceBounds.width / 2 - elementBounds.width / 2);
                y = Math.round(referenceBounds.height / 2 - elementBounds.height / 2);
                break;
            case ssfx.PopupMode.anchorTopLeft:
                x = 0;
                y = -elementBounds.height;
                break;
            case ssfx.PopupMode.anchorTopRight:
                x = referenceBounds.width - elementBounds.width;
                y = -elementBounds.height;
                break;
            case ssfx.PopupMode.anchorBottomRight:
                x = referenceBounds.width - elementBounds.width;
                y = referenceBounds.height;
                break;
            case ssfx.PopupMode.anchorBottomLeft:
                x = 0;
                y = referenceBounds.height;
                break;
            case ssfx.PopupMode.alignTopLeft:
                x = referenceBounds.left;
                y = referenceBounds.top;
                alignment = true;
                break;
            case ssfx.PopupMode.alignTopRight:
                x = referenceBounds.left + referenceBounds.width - elementBounds.width;
                y = referenceBounds.top;
                xOffsetDirection = -1;
                alignment = true;
                break;
            case ssfx.PopupMode.alignBottomRight:
                x = referenceBounds.left + referenceBounds.width - elementBounds.width;
                y = referenceBounds.top + referenceBounds.height - elementBounds.height;
                xOffsetDirection = -1;
                yOffsetDirection = -1;
                alignment = true;
                break;
            case ssfx.PopupMode.alignBottomLeft:
                x = referenceBounds.left;
                y = referenceBounds.top + referenceBounds.height - elementBounds.height;
                yOffsetDirection = -1;
                alignment = true;
                break;
        }
        if (!alignment) {
            x += xDelta + this._options$1.xOffset;
            y += yDelta + this._options$1.yOffset;
        }
        else {
            x += xDelta + this._options$1.xOffset * xOffsetDirection;
            y += yDelta + this._options$1.yOffset * yOffsetDirection;
        }
        var docWidth = document.body.clientWidth;
        if (x + elementBounds.width > docWidth - 2) {
            x -= (x + elementBounds.width - docWidth + 2);
        }
        if (x < 0) {
            x = 2;
        }
        if (y < 0) {
            y = 2;
        }
        ssfx.ElementHelper.setLocation(this.get_element(), ssfx.$create_Location(x, y));
        var host = ssfx.Application.current.get_host();
        if ((host.get_name() === ssfx.HostName.IE) && (host.get_majorVersion() < 7)) {
            this._iframe$1 = document.createElement('IFRAME');
            this._iframe$1.src = 'javascript:false;';
            this._iframe$1.scrolling = 'no';
            this._iframe$1.style.position = 'absolute';
            this._iframe$1.style.display = 'block';
            this._iframe$1.style.border = 'none';
            this._iframe$1.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=0)';
            this._iframe$1.style.left = x + 'px';
            this._iframe$1.style.top = y + 'px';
            this._iframe$1.style.width = elementBounds.width + 'px';
            this._iframe$1.style.height = elementBounds.height + 'px';
            this._iframe$1.style.zIndex = 1;
            this.get_element().parentNode.insertBefore(this._iframe$1, this.get_element());
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ssfx.TimedAnimation

ssfx.TimedAnimation = function ssfx_TimedAnimation(element, duration) {
    ssfx.TimedAnimation.initializeBase(this, [ element ]);
    ss.Debug.assert(duration > 0);
    this._duration$1 = duration;
}
ssfx.TimedAnimation.easeIn = function ssfx_TimedAnimation$easeIn(t) {
    return t * t;
}
ssfx.TimedAnimation.easeInOut = function ssfx_TimedAnimation$easeInOut(t) {
    t = t * 2;
    if (t < 1) {
        return t * t / 2;
    }
    return -((--t) * (t - 2) - 1) / 2;
}
ssfx.TimedAnimation.easeOut = function ssfx_TimedAnimation$easeOut(t) {
    return -t * (t - 2);
}
ssfx.TimedAnimation.prototype = {
    _duration$1: 0,
    _easingFunction$1: null,
    _startTimeStamp$1: 0,
    
    get_duration: function ssfx_TimedAnimation$get_duration() {
        return this._duration$1;
    },
    set_duration: function ssfx_TimedAnimation$set_duration(value) {
        ss.Debug.assert(!this.get_isPlaying());
        ss.Debug.assert(this._duration$1 >= 0);
        this._duration$1 = value;
        return value;
    },
    
    get_easingFunction: function ssfx_TimedAnimation$get_easingFunction() {
        return this._easingFunction$1;
    },
    set_easingFunction: function ssfx_TimedAnimation$set_easingFunction(value) {
        ss.Debug.assert(!this.get_isPlaying());
        this._easingFunction$1 = value;
        return value;
    },
    
    playCore: function ssfx_TimedAnimation$playCore() {
        this._startTimeStamp$1 = new Date().getTime();
        this.progressCore(false, this._startTimeStamp$1);
    },
    
    progressCore: function ssfx_TimedAnimation$progressCore(startRepetition, timeStamp) {
        var frame = 0;
        var completed = false;
        if (!startRepetition) {
            frame = (timeStamp - this._startTimeStamp$1) / this._duration$1;
            if (!this.get_isReversed()) {
                completed = (frame >= 1);
                frame = Math.min(1, frame);
            }
            else {
                frame = 1 - frame;
                completed = (frame <= 0);
                frame = Math.max(0, frame);
            }
            if ((!completed) && (this._easingFunction$1 != null)) {
                frame = this._easingFunction$1.invoke(frame);
            }
        }
        else {
            this._startTimeStamp$1 = timeStamp;
            if (this.get_isReversed()) {
                frame = 1;
            }
        }
        this.performTweening(frame);
        return completed;
    },
    
    stopCore: function ssfx_TimedAnimation$stopCore(completed, stopState) {
        if (!completed) {
            if (stopState === ssfx.AnimationStopState.complete) {
                this.performTweening(1);
            }
            else if (stopState === ssfx.AnimationStopState.revert) {
                this.performTweening(0);
            }
        }
    }
}


ssfx.Application.registerClass('ssfx.Application', null, ssfx.IServiceProvider, ssfx.IServiceContainer, ssfx.IEventManager);
ssfx.ApplicationUnloadingEventArgs.registerClass('ssfx.ApplicationUnloadingEventArgs', ss.EventArgs);
ssfx.HistoryManager.registerClass('ssfx.HistoryManager', null, ss.IDisposable);
ssfx.HistoryEventArgs.registerClass('ssfx.HistoryEventArgs', ss.EventArgs);
ssfx.HostInfo.registerClass('ssfx.HostInfo');
ssfx.EventList.registerClass('ssfx.EventList');
ssfx.JSON.registerClass('ssfx.JSON');
ssfx.ObservableCollection.registerClass('ssfx.ObservableCollection', null, ss.IDisposable, ss.IEnumerable, ss.INotifyCollectionChanged);
ssfx.HttpRequest.registerClass('ssfx.HttpRequest', null, ss.IDisposable);
ssfx.HttpRequestManager.registerClass('ssfx.HttpRequestManager');
ssfx.HttpTransport.registerClass('ssfx.HttpTransport', null, ss.IDisposable);
ssfx.PostHttpRequestEventArgs.registerClass('ssfx.PostHttpRequestEventArgs', ss.EventArgs);
ssfx.PreHttpRequestEventArgs.registerClass('ssfx.PreHttpRequestEventArgs', ss.EventArgs);
ssfx._xmlHttpResponse.registerClass('ssfx._xmlHttpResponse', null, ssfx.IHttpResponse);
ssfx._xmlHttpTransport.registerClass('ssfx._xmlHttpTransport', ssfx.HttpTransport);
ssfx.Animation.registerClass('ssfx.Animation', null, ss.IDisposable);
ssfx.AnimationManager.registerClass('ssfx.AnimationManager');
ssfx.AnimationSequence.registerClass('ssfx.AnimationSequence', ssfx.Animation);
ssfx.Behavior.registerClass('ssfx.Behavior', null, ss.IDisposable, ssfx.ISupportInitialize, ss.INotifyPropertyChanged);
ssfx.Color.registerClass('ssfx.Color');
ssfx.Control.registerClass('ssfx.Control', ssfx.Behavior, ssfx.INotifyDisposing);
ssfx.ElementEventList.registerClass('ssfx.ElementEventList', null, ss.IDisposable);
ssfx.DragDropEventArgs.registerClass('ssfx.DragDropEventArgs', ss.EventArgs);
ssfx.DragDropManager.registerClass('ssfx.DragDropManager');
ssfx._dragDropTracker.registerClass('ssfx._dragDropTracker', null, ssfx.IDragSource);
ssfx.ElementHelper.registerClass('ssfx.ElementHelper');
ssfx.TimedAnimation.registerClass('ssfx.TimedAnimation', ssfx.Animation);
ssfx.FadeEffect.registerClass('ssfx.FadeEffect', ssfx.TimedAnimation);
ssfx._genericControl.registerClass('ssfx._genericControl', ssfx.Control);
ssfx.OverlayBehavior.registerClass('ssfx.OverlayBehavior', ssfx.Behavior);
ssfx.PopupBehavior.registerClass('ssfx.PopupBehavior', ssfx.Behavior);
ssfx.Application.current = null;
(function () {
    ssfx.Application.current = new ssfx.Application();
})();
ssfx.JSON._dateRegex = null;
ssfx.HttpRequestManager.__requestInvoking = null;
ssfx.HttpRequestManager.__requestInvoked = null;
ssfx.HttpRequestManager._timeoutInterval = 0;
ssfx.HttpRequestManager._activeRequests = [];
ssfx.HttpRequestManager._appIdleHandler = null;
ssfx.AnimationManager._fps = 100;
ssfx.AnimationManager._activeAnimations = null;
ssfx.AnimationManager._timerCookie = 0;
ssfx.DragDropManager._dragDropImplementation = null;
ssfx.DragDropManager._dropTargets = [];
ssfx.DragDropManager._dragStartingHandler = null;
ssfx.DragDropManager._dragEndingHandler = null;
ssfx.DragDropManager._currentDataObject = null;
ssfx.OverlayBehavior._visibilityChangedEventKey$1 = 'visibilityChanged';

// ---- Do not remove this footer ----
// This script was generated using Script# v0.6.0.0 (http://projects.nikhilk.net/ScriptSharp)
// -----------------------------------

}
ss.loader.registerScript('ScriptFX.Core', [], executeScript);
})();
