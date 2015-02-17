// Usage Notes:
// - Only generic Value<T> and Maybe<T> should be inherited.
// - Non-generic interfaces are for type-level tests, e.g.:
//     if (value is Just) ...
// - Only evaluating a Maybe should return Nothing, and MValue
//     should only be used as the evaluation type of a Maybe.

using System;

namespace Settworks.LiftedValues {

  /// <summary>
  /// An MValue (Maybe Value) is either a Just Value, or Nothing.
  /// </summary>
  public interface MValue { }
  /// <summary>
  /// An MValue (Maybe Value) is either a Just Value, or Nothing.
  /// </summary>
  public interface MValue<T> : MValue {
    Func<T> Eval { get; }
  }
  
  /// <summary>
  /// Nothing represents the absence of value. Attempting to
  /// evaluate Nothing causes a runtime exception!
  /// </summary>
  public interface Nothing { }

  /// <summary>
  /// A Just Value evaluates to a concrete value of its type.
  /// </summary>
  public interface Just { }
  /// <summary>
  /// A Just Value evaluates to a concrete value of its type.
  /// </summary>
  public interface Value<T> : MValue<T>, Just { }
  
  /// <summary>
  /// A Maybe evaluates to either a Just Value or Nothing.
  /// </summary>
  public interface IMaybe { }
  /// <summary>
  /// A Maybe evaluates to either a Just Value or Nothing.
  /// </summary>
  public interface Maybe<T> : Value<MValue<T>>, IMaybe { }
  
}