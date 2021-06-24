# FunctionalExtensions
Functional extensions for C# .NET Core and EF Core
Currently built for: **.NET Core 3.1**

![Build](https://github.com/JurajDoncevic/FunctionalExtensions/actions/workflows/build.yml/badge.svg)

![Test](https://github.com/JurajDoncevic/FunctionalExtensions/actions/workflows/test.yml/badge.svg)

## About
FunctionalExtensions is a library for .NET Core providing functional-styled helper methods and types for C#. It allows production of safer and cleaner code.

The **Base** library provides high-order-functions and types for:
- the `Unit` terminal object
- conversion of `Action<...>` into `Func<..., Unit>`
- partial application via `Apply`
- aggregation (folding)
- mapping over objects (functor), including specialization for `IEnumerable` and `Task`
- binding over `IEnumerable` and `Task` (as monads)
- synchrounous and asynchronous specialized functions and types for `try-catch` and `using` handling
- the `Result` and `DataResult` monads, with synchrounous and asynchronous functions
- parallel execution and finalization of smaller functions (forking)

The **GenericProvider** library extension provides an extendable generic data access provider (repository) over a Entity Framework-operated store built using the Base library. 

## Base library functionalities
### Unit
### Apply
### Aggregation
### Fork
### Validation
### Pass
### Map
### Bind
### Using
### TryCatch

## Results
### Result
### DataResult\<T>

## GenericProvider over EF Core
### BaseModel\<TKey>
### BaseProvider<TKey, TModel, TDbContext>
