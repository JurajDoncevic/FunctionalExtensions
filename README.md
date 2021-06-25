# FunctionalExtensions
Functional extensions for C# .NET Core and EF Core
Currently built for: **.NET Core 3.1**

![Build](https://github.com/JurajDoncevic/FunctionalExtensions/actions/workflows/build.yml/badge.svg)

![Test](https://github.com/JurajDoncevic/FunctionalExtensions/actions/workflows/test.yml/badge.svg)

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
### Using
### TryCatch

## Results
### Result
### DataResult\<T>

## GenericProvider over EF Core
### BaseModel\<TKey>
### BaseProvider<TKey, TModel, TDbContext>
