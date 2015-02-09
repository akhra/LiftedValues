# LiftedValues
*Lazy functional value wrappers for C#*

This package exists to allow transparency between values and zero-argument functions, which enables some convenient functional programming techniques.

By way of explanation, here's a simple example: let's say you have a pair of dice, a fixed value which modifies the die roll, and another modifier that requires an external lookup. With LiftedValues they can all be kept in the same list of `Value<int>`, turning this into a simple `foreach` enumeration where none of the required computations (die rolls, lookup) are performed until they're actually needed!

In addition to `Value<T>`, LiftedValues provides `Maybe<T>` which is roughly the nullable equivalent: it evaluates to either a good `Value<T>`, or to `Nothing`, which can be distinguished using C#'s type-level `is` operator, yielding very nice code:

    var maybeValue = someMaybe.Eval();
    if (maybeValue is Nothing) {
      HandleNothing();
    } else {
      HandleValue(maybeValue.Eval());
    }

Wrapping simple values and functions is covered via factory methods on the static `Value` and `Maybe` classes. There are also `Maybe` wrappers for `WeakReference`, including weak delegates to instance methods. (Compatible with Unity3D!)

To streamline more complex use, `Value<T>` and `Maybe<T>` are actually interfaces, allowing them to be inherited along with another class. Yes, this violates conventional naming. Too bad. ;)

*Despite names borrowed from Haskell, the FP-wary can rest easy: there are no monads here.*
