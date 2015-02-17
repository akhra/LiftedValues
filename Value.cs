// Usage Notes:
// - Generic factory methods can usually infer type, e.g.:
//     Value.Pure(3f) == Value.Pure<float>(3)
// - Value.Nothing<T>() returns a singleton for each type.
//     Use this only when returning from custom Maybe types.
//     Because it has no arguments, type must be specified.
// - Value.Pure(value) takes an argument of any type except
//     MValue or Nothing, and returns a Value which evaluates
//     to the supplied argument. Passing an MValue or Nothing
//     causes a runtime exception.
// - Value.From(func) takes a function with no arguments and
//     a return type other than MValue or Nothing, and returns
//     a Value which evaluates by calling that function.
//     Passing a function which returns MValue or Nothing
//     causes a runtime exception.

using System;

namespace Settworks.LiftedValues {

  /// <summary>
  /// Static methods for basic Just Value construction.
  /// </summary>
  public static class Value {
    
    /// <summary>
    /// Returns the Nothing singleton for a given type.
    /// </summary>
    public static MValue<T> Nothing<T>() {
      return ValueNothing<T>.value;
    }
    
    /// <summary>
    /// Returns a Value which evaluates to the supplied concrete
    /// value.
    /// </summary>
    public static Value<T> Pure<T>(T value) {
      if ( value is MValue
        && !( value is Just
           || value is IMaybe ))
      {
        throw new ArgumentOutOfRangeException
          ("value", "Only Maybe may evaluate to MValue or Nothing.");
      } else {
        return new ValuePure<T>(value);
      }
    }
    
    /// <summary>
    /// Returns a Value which evaluates as the supplied function.
    /// </summary>
    public static Value<T> From<T>(Func<T> func) {
      Type t = typeof(T);
      if ( t.IsSubclassOf(typeof(MValue))
        && !( t.IsSubclassOf(typeof(Just))
           || t.IsSubclassOf(typeof(IMaybe)) ))
      {
        throw new ArgumentOutOfRangeException
          ("func", "Only Maybe may evaluate to MValue or Nothing.");
      } else {
        return new ValueFrom<T>(func);
      }
    }

    // Private members below this point.
    
    sealed class ValueNothing<T> : MValue<T>, Nothing {
      public static readonly ValueNothing<T> value = new ValueNothing<T>();
      public Func<T> Eval {
        get { throw new NotSupportedException ("Cannot evaluate Nothing."); }
      }
    }
    
    sealed class ValuePure<T> : Value<T> {
      readonly T value;
      public Func<T> Eval {
        get { return () => value; }
      }
      public ValuePure(T value) {
        this.value = value;
      }
    }

    sealed class ValueFrom<T> : Value<T> {
      readonly Func<T> function;
      public Func<T> Eval {
        get { return function; }
      }
      public ValueFrom(Func<T> func) {
        this.function = func;
      }
    }

  }

}