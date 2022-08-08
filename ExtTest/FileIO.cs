using LanguageExt;
using static LanguageExt.Prelude;

namespace ExtTest;

public interface FileIO
{
    string ReadAllText(string path);

    Unit WriteAllText(string path, string text);
}

public interface HasFile<RT> where RT : struct, HasFile<RT>
{
    Eff<RT, FileIO> FileEff { get; }
}

public struct LiveRuntime : HasFile<LiveRuntime>
{
    public Eff<LiveRuntime, FileIO> FileEff => SuccessEff(LiveFileIO.Default);
}

public class LiveFileIO : FileIO
{
    public static readonly FileIO Default = new LiveFileIO();

    public string ReadAllText(string path) => File.ReadAllText(path);

    public Unit WriteAllText(string path, string text)
    {
        File.WriteAllText(path, text);
        return unit;
    }
}