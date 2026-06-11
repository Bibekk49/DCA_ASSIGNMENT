# Assignment 6 – Feature Status

* [x] Command Dispatcher
  * [x] `ICommandDispatcher` interface
  * [x] `CommandDispatcher` implementation (resolves handlers via `IServiceProvider`)
  * [x] `LoggingDispatcherDecorator`
* [x] Interaction tests (ZOMBIES)
  * [x] Zero – no handler registered → exception
  * [x] One correct – UC2 handler called
  * [x] One correct – UC5 handler called
  * [x] One incorrect – wrong handler registered → exception
  * [x] Many – both registered, only correct one called (UC2)
  * [x] Many – both registered, only correct one called (UC5)
  * [x] Same – handler called exactly once
* [x] Decorator tests (`DispatcherLoggingDecoratorTests`)
* [x] Challenge: auto-register handlers via `IServiceCollection` extension method
  * [x] Scans assembly via reflection
  * [x] Registers all `ICommandHandler<T>` implementations automatically
  * [x] Auto-registration tests
