// Usage Notes:
// - Generic factory methods can usually infer type, e.g.:
//     Maybe.Pure(3f) == Maybe.Pure<float>(3)
// - Maybe.Nothing<T>() returns a singleton for each type,
//     which evaluates to Value.Nothing. Because it has no
//     arguments, type must be specified.
// - Maybe.Just(Value) takes a Value, and returns a Maybe
//     which evaluates to that Value.
// - Maybe.Pure(value) takes an argument of any type, and
//     returns a Maybe which evaluates to Value.Pure(value),
//     unless the type is Nothing, in which case it returns
//     Maybe.Nothing.
// - Maybe.From(func) takes a function with no arguments
//     any any return type, and returns a Maybe which
//     evaluates to Value.From(value), unless the type is
//     Nothing, in which case it returns Maybe.Nothing.
// - Maybe.WeakPure(target) returns a Maybe which contains a
//     weak reference to the target. It behaves as Maybe.Pure
//     while the target is alive, Maybe.Nothing otherwise.
// - Maybe.WeakFrom(func) returns a Maybe which contains a
//     weak reference to a function. It behaves as Maybe.From
//     while the target is alive, Maybe.Nothing otherwise.
// - NOTE: Weak references do not expire until the target is
//     garbage collected! Having no strong references does
//     not guarantee a dead weak reference.

using System;

namespace Settworks.LiftedValues {

  /// <summary>
  /// Static methods for basic Maybe construction.
  /// </summary>
  public static class Maybe {

    /// <summary>
    /// Returns the Maybe Nothing singleton for a given type.
    /// </summary>
    public static Maybe<T> Nothing<T>() {
      return MaybeNothing<T>.value;
    }

    /// <summary>
    /// Returns a Maybe which evaluates to the supplied MValue.
    /// </summary>
    public static Maybe<T> Just<T>(MValue<T> value) {
      return (value is Just)
             ? new MaybeJust<T>(value)
             : Nothing<T>();  // Use singletons for Nothing.
    }

    /// <summary>
    /// Alias for Maybe.Just(Value.Pure(value)), or Maybe.Nothing() if
    /// value is Nothing.
    /// </summary>
    public static Maybe<T> Pure<T>(T value) {
      return (value is Nothing)
             ? Nothing<T>()
             : Just(Value.Pure<T>(value));
    }

    /// <summary>
    /// Alias for Maybe.Just(Value.From(func)), or Maybe.Nothing() if
    /// func returns Nothing.
    /// </summary>
    public static Maybe<T> From<T>(Func<T> func) {
      return (typeof(T).IsSubclassOf(typeof(Nothing)))
             ? Nothing<T>()
             : Just(Value.From<T>(func));
    }

    /// <summary>
    /// Encapsulates a weak reference in a Maybe, which evaluates to
    /// strongly-referenced Value.Pure(target) if target is alive and
    /// not Nothing; otherwise evaluates to Nothing.
    /// </summary>
    public static Maybe<T> WeakPure<T>(T target) {
      return (target is Nothing)
             ? Nothing<T>()
             : new MaybeWeakPure<T>(target);
    }

    /// <summary>
    /// Encapsulates a weak delegate in a Maybe, which evaluates to
    /// strongly-referenced Value.From(func) if target is alive;
    /// otherwise evaluates to Nothing.
    /// </summary>
    public static Maybe<T> WeakFrom<T>(Func<T> func) {
      return (typeof(T).IsSubclassOf(typeof(Nothing)))
             ? Nothing<T>()
             : (func.Target == null)
               ? Just(Value.From<T>(func)) // static method
               : new MaybeWeakFrom<T>(func);
    }

    // Private members below this point.

    sealed class MaybeNothing<T> : Maybe<T> {
      // MaybeNothing singletons wrap the corresponding Value.Nothing singletons.
      public static readonly MaybeNothing<T> value = new MaybeNothing<T>();
      public Func<MValue<T>> Eval {
        get { return () => Value.Nothing<T>(); }
      }
    }
    
    sealed class MaybeJust<T> : Maybe<T> {
      readonly MValue<T> just;
      public Func<MValue<T>> Eval {
        get { return () => just; }
      }
      public MaybeJust(MValue<T> just) {
        // Known to be Just (i.e. a Value); but the Maybe interface
        // uses MValue, so casting before Eval would be wasted.
        this.just = just;
      }
    }
    
    sealed class MaybeWeakPure<T> : Maybe<T> {
      readonly WeakReference target;
      public Func<MValue<T>> Eval {
        get {
          object reference = target.Target;
          if (reference == null) {
            return () => Value.Nothing<T>();
          } else {
            return () => Value.Pure((T)(reference));
          }
        }
      }
      public MaybeWeakPure(T target) {
        this.target = new WeakReference(target);
      }
    }
    
    sealed class MaybeWeakFrom<T> : Maybe<T> {
      readonly WeakReference target;
      readonly System.Reflection.MethodInfo method;
      public Func<MValue<T>> Eval {
        get {
          object reference = target.Target;
          if (reference == null) {
            return () => Value.Nothing<T>();
          } else {
            return () => Value.From
              ( (Func<T>)
                Delegate.CreateDelegate(typeof(Func<T>),reference,method)
              );
          }
        }
      }
      public MaybeWeakFrom(Func<T> func) {
        target = new WeakReference(func.Target);
        method = func.Method;
      }
    }

  }

}