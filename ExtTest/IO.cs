using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace ExtTest;

public delegate Either<Error, A> IO<Env, A>(Env env);

public static class IO
{
    // Allows us to lift pure values into the IO domain
    public static IO<Env, A> Pure<A, Env>(A value) =>
        (Env env) => value;

    // Wrap up the error handling
    public static Either<Error, A> Run<Env, A>(this IO<Env, A> ma, Env env)
    {
        try
        {
            return ma(env);
        }
        catch(Exception e)
        {
            return Error.New("IO error", e);
        }
    }

    // Functor map
    public static IO<Env, B> Select<Env, A, B>(this IO<Env, A> ma, Func<A, B> f) => env =>
        ma(env).Match(
            Right: x => f(x),
            Left: Left<Error, B>);

    // Functor map
    public static IO<Env, B> Map<Env, A, B>(this IO<Env, A> ma, Func<A, B> f) =>
        Select(ma, f);

    // Mondaic bind
    public static IO<Env, B> SelectMany<Env, A, B>(this IO<Env, A> ma, Func<A, IO<Env, B>> f) => env =>
        ma(env).Match(
            Right: x => f(x)(env),
            Left: Left<Error, B>);

    // Monadic bind
    public static IO<Env, B> Bind<Env, A, B>(this IO<Env, A> ma, Func<A, IO<Env, B>> f) =>
        SelectMany(ma, f);

    // Monadic bind + projection
    public static IO<Env, C> SelectMany<Env, A, B, C>(this IO<Env, A> ma, Func<A, IO<Env, B>> bind, Func<A, B, C> project) =>
        ma.SelectMany(a => bind(a).Select(b => project(a, b)));
}