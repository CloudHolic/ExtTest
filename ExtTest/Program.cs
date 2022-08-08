using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using ExtTest;

static IO<Env, string> readAllText<Env>(string path) where Env : FileIO =>
    env => env.ReadAllText(path);

static IO<Env, Unit> writeAllText<Env>(string path, string text) where Env : FileIO =>
    env => env.WriteAllText(path, text);

// A pure function for turning all lower-case characters into upper-case characters
static string Capitalize(string text) =>
    new(text.Map(x => char.IsLower(x) ? char.ToUpper(x) : x).ToArray());

static Eff<RT, Unit> CopyFile<RT>(string source, string dest) where RT : struct, HasFile<RT> =>
    from text in default(RT).FileEff.Map(rt => rt.ReadAllText(source))
    from _ in default(RT).FileEff.Map(rt => rt.WriteAllText(dest, text))
    select unit;

var appEnv = new LiveEnv();
var inpath = @"D:\Downloads\filetypesman-x64\readme.txt";
var outpath = @"D:\Downloads\readme.txt";

// Use it in the IO context
var computation = from text in readAllText<LiveEnv>(inpath).Map(Capitalize)
                  from _ in writeAllText<LiveEnv>(outpath, text)
                  select unit;

Either<Error, Unit> result = computation.Run(appEnv);