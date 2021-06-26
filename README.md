# FunctionalExtensions
Functional extensions for C# .NET Core and EF Core
Currently built for: **.NET Core 3.1**

![Build](https://github.com/JurajDoncevic/FunctionalExtensions/actions/workflows/build.yml/badge.svg)

![Tests](https://github.com/JurajDoncevic/FunctionalExtensions/actions/workflows/test.yml/badge.svg)

## About
FunctionalExtensions is a library for .NET Core providing functional-styled helper methods and types for C#. It allows production of safer and cleaner code.

The **Base** library provides helper functions, high-order functions, and types for:
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
`Unit` is a terminal data type. This data type has only one form of instance, constructed by the `Unit()` function. 

It is used to signify that an operation (function) doesn't return anything - similar to a function returning `void`. Usually, such functions have side-effects!

`Unit` can also be constructed through the `Ignore` function. `Ignore` is used to ignore a returned result of an operation. `Ignore` in essence turns **any** data type to `Unit`.

```
Ignore<T>: T -> Unit
```
Example:
```csharp
Func<int, int, int> add = (x, y) => x + y;
var result = add(5, 2).Ignore(); //result == Unit
```

By switching void-returning to Unit-returning methods, the need for `Action<...>` types no longer exists. Every method can now be referenced as a kind of `Func<..., Unit>`.
### ToFunc
There isn't always a benefit of working over a functionally-styled (referentially transparent) codebase - `Action` types will appear.
So each type of the `Action<...>` kind can morphed into a `Func<..., Unit>` type using the `ToFunc()` function. 
```
ToFunc<T*>: Action<T*> -> Func<T*, Unit>
```
Example:
```csharp
Action<int, int> showSum = (x, y) => Console.WriteLine("The sum is: {0}", x + y);
Func<int, int, Unit> funcShowSum = showSum.ToFunc();
var result = funcShowSum(3, 4); // this is a Unit
```
`ToFunc` can be called over Actions of up to 10 parameters. Need more params? Go ask Uncle Bob what he thinks of that...
### Pass
The `Pass` function is used to execute a Unit-returning/side-effect function with the currenty piped data as the parameter. The data is passed onward without mutation down the pipe.
```
Pass: T -> (T -> Unit) -> T
```
Example:
```csharp
...
.GetSomeData() // returns SomeData
.Pass(_ => Console.Writeline("I got this: " + _.ToString())
.DoSomethingWithSomeData() // receives SomeData as param
...
```
### Apply
Partial application is an important mechanism of functional programming languages, as it enables abstraction. The layers of abstraction are lowered as each parameter of a function is applied to; and the function becomes more specialized. A strategy pattern can also be designed if a high-order function is partially applied to.

The `Apply` function is used for partial application. Each applied param is closed in closure.
```
Apply: Func<T+> -> T -> Func<T*>
```
Example:
```csharp
Func<int, int, int, int> f = (x, y, z) => x + y + z;
Func<int, int, int> g = f.Apply(4); // takes only the params y and z
Func<int, int> h = f.Apply(2).Apply(3); // takes only the param z

int resG = g(5,6); // 15
int resH = h(4); // 9
int resX = f.Apply(1)(2, 3) // 6
```
### Aggregation
LINQ already offers an aggregation operation `Aggregate`, but this library also has the `Fold` operation for `IEnumerable` and `Task<IEnumerable>`. 
```
Fold: IEnumerable<T> -> R -> (T -> R -> R) -> R
```
Example:
```csharp
const string testString = "12345";

List<int> testInts = new List<int>{ 1, 2, 3, 4, 5 };


string finalCsv = testInts.Fold("", (item, seed) => seed + item.ToString()); // 12345
```
This can also be useful for folding over an `IServiceCollection` to register services for dependency injection:
```csharp
System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                .Where(_ => _.Name.EndsWith("Service"))
                .ToList()
                .Fold(services, (provider, services) => services.AddTransient(provider)); // returns appended IServiceCollection
```
`Foldi` is also provided by this library, with the enumerable size limit of `LONG_MAX`. The `Foldi` folding function receives the index of the currently folded element 
```
Foldi: IEnumerable<T> -> R -> (long -> T -> R -> R) -> R
```
```csharp
List<string> names = new List<string> { "John", "Barry", "Steven", "Joe" };
// John is 1 Barry is 2 Steven is 3 Joe is 4
string result = names.Foldi("", (idx, item, seed) => item + " is " + (idx + 1).ToString() + " ");
```
### Fork
`Fork` is a helper function used to execute multiple "prong" functions in parallel and finalize their results into a final result.
```
Fork: T -> (IEnumerable<R> -> R) -> Array<(T -> R)> -> R 
```
Examples:
```csharp
int x = 5;
x.Fork(_ => _.Sum(), // finalization function
       _ => _ + 1, // 6
       _ => _ * 2, // 10
       _ => _ / 5, // 1
       _ => _ - 2 // 3
    ); // gives result of 20

// example services for getting prices of gas from different gas stations
GasService gasService1, gasService2, gasService3 = ...;
Fork(_ => _.Min(),
     () => gasService1.GetPrice(),
     () => gasService2.GetPrice(),
     () => gasService3.GetPrice(),
    ); // queries all the services and finds the minimum price
```
### Validation
`Validate` is used to check if the given value passes all the predicates.
```
T -> Array<(T -> bool)> -> bool
```
Example:
```csharp
Person person = new Person
{
    Name = "John",
    Surname = "Doe",
    Age = 20
};

bool isPersonValid = person.Validate(
    _ => !_.Name.IsNullOrEmpty(),
    _ => _.Surname.Length >= 3,
    _ => Age > 14
); // true
```
### Map
`Map` is the functor mapping function. `Map` is defined generically for all data types and specifically for `IEnumerable` and `Task`. For `IEnumerable` the `Mapi` function is also provided, allowing mapping with the elements' indices (the enumerable maximum size is `LONG_MAX`).
```
Map: T -> (T -> R) -> R
Map: IEnumerable<T> -> (T -> R) -> IEnumerable<R>
Map: Task<T> -> (T -> R) -> Task<R>
Mapi: IEnumerable<T> -> (long -> T -> R) -> IEnumerable<R>
```
Examples:
```csharp
2.Map(_ => _ + 3); // 5

List<string> someWords = new List<string> { "these", "are", "some", "words" };

// "THESE", "ARE", "SOME", "UPPER", "WORDS"
someWords.Map(_ => _.ToUpper()); 

// "1:these", "2:are", "3:some", "4:words"
someWords.Mapi((idx, _) => (idx + 1).ToString() + ":" + _);

// within an async method
await Task.Run(
    () => { System.Threading.Thread.Sleep(2000); return 1; }
    ).Map(_ => _ + 1); // 2
```
`Map` is also defined for Result types and kinds (see below).

### Bind
`Bind` is the monad binding function. `Bind` is defined for `IEnumerable` and `Task`.
```
Bind: M<T> -> (T -> M<R>) -> M<R>
Bind: Task<T> -> (T -> Task<R>) -> Task<R>
Bind: IEnumerable<T> -> (T -> IEnumerable<R>) -> IEnumerable<R>
```
Examples:
```csharp
List<List<int>> list = new List<List<int>>
{
    new List<int> { 1, 2, 3, 4 },
    new List<int> { 5, 6, 7, 8, 9 },
    new List<int> { 10, 11, 12 }
};
List<int> flatList = list.Bind(_ => _); // 1 2 3 4 5 6 7 8 9 10 11 12

// within an async method
Task<int> startTask = Task<int>.Run(() => { System.Threading.Thread.Sleep(1000);  return 1; });
string result = // "3"
    await startTask.Bind(_ => Task<int>.Run(() => _ + 1))
                   .Bind(_ => Task<int>.Run(() => _ + 1))
                   .Bind(_ => Task<string>.Run(() => _.ToString()));
```
`Bind` is also defined for Result types and kinds (see below).
### Using
`Using` is a high-order-function over the `using` block, so it can be seamlessly integrated into the functional pipeline. As parameters it accepts a *setup* function to setup the `IDisposable` and an *operate* function to operate over the `IDisposable`.
```
Using: () -> IDisposable -> (IDisposable -> T) -> T
Using: () -> IDisposable -> (IDisposable -> Task<T>) -> Task<T>

```
Example:
```csharp
Person result =
    Using(
        () => new PersonDbContext(), // setup IDisposable
        _ => _.Person.Find(id) // operate over IDisposable
        );
```
In the future it is planned to provide a `Using<T>` type, so you can imply the usage of `using` by design. This will be possible by setting `Using<T>` as the return type of a method (exp. via an interface-defined method).

### TryCatch
`TryCatch` is a high-order-function over the `try-catch` block, so it can be seasmlessly integrated into the functional pipeline. As parameters it accepts an *operate* function and an *catchOperate* function. The `TryCatch` function returns a `Try<T>` type (similar to Scala).

Due to the existence of the `Try<T>` type, the usage of a `try-catch` block is implied for a function returning `Try<T>` or its derivates (see Results).
```
Try: (() -> T) -> (Exception -> Exception) -> Try<T>
Try: (() -> Task<T>) -> (Exception -> Exception) -> Task<Try<T>>
```
Examples:
```csharp
// IsException = true, IsData = false
Try<int> exceptionTry = 
    TryCatch(
        ((Action)(() =>
        {
            throw new Exception(exceptionMessage);
            return 1;
        })).ToFunc(),
        (ex) => ex
        );

// IsException = true, IsData = false, ExpectedData = 1
Try<int> exceptionTry = 
    TryCatch(
        ((Action)(() =>
        {
            return 1;
        })).ToFunc(),
        (ex) => ex
        );

// If expected data is Unit, then IsData is marked false.
// IsException = false, IsData = false, ExpectedData = Unit
Try<Unit> unitTry =
    TryCatch(
        ((Action)(() => Console.WriteLine())).ToFunc(),
        (ex) => ex
        );

```

## Results
Result kinds are used to encapsulate results of complex operations that return data or a logical result signifying the operation's outcome.

All results can have the following ErrorType states:
- ExceptionThrown
- Failure
- NoData
- Unknown
- None

This is implemented in the `ErrorTypes` enum. 
### Result
`Result` is a monadic type that embellish an operation's outcome. `Result` can be constructed from the `Try<Unit>` or `Try<bool>` type (asnychronously or synchronously) using the `ToResult` function. The `Bind` function allows piping multiple operations.

```csharp
// IsSuccess = false, IsFailure = true, 
// ErrorMessage = Operation failed, ErrorType = Failure
Result result =
    TryCatch( // Try<bool>
        () => false,
        (ex) => ex
        ).ToResult();

// IsSuccess = true, IsFailure = false, 
// ErrorMessage = string.Empty, ErrorType = None
Result result =
    TryCatch( // Try<bool>
        () => true,
        (ex) => ex
        ).ToResult();

// IsSuccess = false, IsFailure = true, 
// ErrorMessage = "Exception message", ErrorType = ExceptionThrown
Result result =
    TryCatch( // Try<bool>
        () => { throw new Exception("Exception message"); return true},
        (ex) => ex
        ).ToResult();

// all is good unless an exception appears
// IsSuccess = true, IsFailure = false, 
// ErrorMessage = string.Empty, ErrorType = None
Result result =
    TryCatch( // Try<Unit>
        ((Action)(() => Console.WriteLine())).ToFunc(),
        (ex) => ex
        ).ToResult();


Result result =
    TryCatch( // Try<bool>
        () => true,
        (ex) => ex
        ).ToResult()
        .Bind(
            _ => TryCatch( // this simulates another operation
                    () => true,
                    (ex) => ex
                    ).ToResult()
            )
        .Bind(
            _ => TryCatch( // this simulates another operation
                    () => true,
                    (ex) => ex
                    ).ToResult()
            );
```

### DataResult
The `DataResult<T>` is a monadic kind used to embellish an operation's outcome and returning data. `DataResult` can be constructed from the `Try<T>` kind. The `Bind` function allows piping multiple operations.
```csharp
// IsSuccess = true, IsFailure = false, 
// ErrorMessage = string.Empty, ErrorType = None, 
// HasData = true, Data = { Name = "test" }
DataResult<a> dataResult =
    TryCatch(
        () => new { Name = "test" },
        (ex) => ex
        ).ToDataResult();

// IsSuccess = false, IsFailure = true, 
// ErrorMessage = <exceptionMessage>, ErrorType = ExceptionThrown, 
// HasData = false, Data = default
DataResult<a> dataResult =
    TryCatch(
        () => { throw new Exception(exceptionMessage); return new { Name = "test" }; },
        (ex) => ex
        ).ToDataResult();

DataResult<string> dataResult = // "1234"
    TryCatch(
        () => 1,
        (ex) => ex
        ).ToDataResult()
    .Bind(x => 
        TryCatch( // this mocks a complex data returning operation
            () => x.ToString() + "2",
            (ex) => ex
            ).ToDataResult())
    .Bind(x =>
        TryCatch(
            () => x.ToString() + "3",
            (ex) => ex
            ).ToDataResult())
    .Bind(x =>
        TryCatch(
            () => x.ToString() + "4",
            (ex) => ex
            ).ToDataResult()
    );

DataResult<string> dataResult = // ExceptionThrown, 3rd Bind not executed
    TryCatch(
        () => 1,
        (ex) => ex
        ).ToDataResult()
    .Bind(x =>
        TryCatch(
            () => x.ToString() + "2",
            (ex) => ex
            ).ToDataResult())
    .Bind(x =>
        TryCatch(
            () => { throw new Exception(exceptionMessage; return "3"; },
            (ex) => ex
            ).ToDataResult())
    .Bind(x =>
        TryCatch(
            () => x.ToString() + "4",
            (ex) => ex
            ).ToDataResult()
    );
```

In the future `Bind` will be extended so it can seamlessly operate over a pipeline containing both Results and DataResults

## GenericProvider over EF Core
A generic repository for Entity Framework Core has been implementing using the Base library.
### BaseModel
All models, scaffolded from or migrated to a database, must inherit the `BaseModel` class. This class contains the primary key identifier `Id` and its basic annotations. The data type of the key is determined by the generic parameter `TKey` 
```csharp
// this model has the Id of type Guid
public class Person : BaseModel<Guid>  
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public Place Place { get; set; }
}
``` 
### BaseProvider
On each model inheriting the `BaseModel` class, a provider can be implemented by inheriting the generic `BaseProvider` class. The following methods are generically and asynchronously implemented using EF Core:

- `Fetch` - fetches an instance with given id from the DB
- `FetchAll` - fetches all instances from the DB
- `FetchIncluding` - `Fetch` but additional elements from the aggregate can be loaded with multiple expressions
- `FetchAllIncluding` - `FetchAll` but additional elements from the aggregate can be loaded with multiple expressions
- `Insert` - inserts a new entity into the DB
- `Delete` - deletes an entity with given id from the DB

The `BaseProvider` returns embellished results using `Result` and `DataResult`.

The class `Person` (from the previous example) can have its provider implemented as follows:
```csharp
public class PersonProvider : BaseProvider<Guid, Person, PersonDbContext>
{
        public PersonProvider(PersonDbContext dbContext) : base(dbContext)
        {
        }

        // additional user-defined methods go here
}
```
With just this, the provider can be used:
```csharp
// dbContext and id are initialized somewhere above
...
PersonProvider personProvider = new PersonProvider(dbContext);

var dataResult = await personProvider.FetchIncluding(id, _ => Place, _ => Place.Country);
``` 
By extensions, the Bind function can also be used to chain Provider operations:
```csharp
var chainedResults =
    await _personProvider.FetchAll()
                         .Bind(_ => _placeProvider.Fetch(_.First().PlaceId.Value))
                         .Bind(_ => _countryProvider.Fetch(_.CountryId))
                         .Map(_ => _.Name);
```
