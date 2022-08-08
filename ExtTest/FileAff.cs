using LanguageExt;
using static LanguageExt.Prelude;

namespace ExtTest;

public interface FileAsyncIO
{
    ValueTask<string[]> ReadAllLinesAsync(string path);

    ValueTask<Unit> WriteAllLinesAsync(string path, string[] lines);
}

public static class FileAff
{
    static readonly FileAsyncIO injected;

    public static Aff<Seq<string>> readAllLines(string path) =>
        Aff(async () => (await injected.ReadAllLinesAsync(path)).ToSeq());

    public static Aff<Unit> writeAllLines(string path, Seq<string> lines) =>
        Aff(async () =>
        {
            await injected.WriteAllLinesAsync(path, lines.ToArray());
            return unit;
        });
}
