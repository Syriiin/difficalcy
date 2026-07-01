// This is a small tool to remove the unnecessary assets from the osu.Game.Resources.dll transitive dep before build time.
// It's a bit hacky and I don't love it but it saves over 100MB from the final image size, so it's worth it.

using Mono.Cecil;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: StripResources <input-path> <output-path>");
    return 1;
}

string inputPath = args[0];
string outputPath = args[1];

var parameters = new ReaderParameters { AssemblyResolver = new DefaultAssemblyResolver() };
using var assembly = AssemblyDefinition.ReadAssembly(inputPath, parameters);

if (assembly.MainModule.Resources.Count == 0)
{
    Console.WriteLine("No embedded resources found.");
    return 0;
}

int count = assembly.MainModule.Resources.Count;
long totalSize = 0;
foreach (var resource in assembly.MainModule.Resources.OfType<EmbeddedResource>())
{
    totalSize += resource.GetResourceData().Length;
}

assembly.MainModule.Resources.Clear();
assembly.Write(outputPath, new WriterParameters { WriteSymbols = false });

var input = new FileInfo(inputPath);
var output = new FileInfo(outputPath);
Console.WriteLine($"Stripped {count} resources ({totalSize / (1024 * 1024):F1} MB)");
Console.WriteLine($"Input:  {input.Length / (1024 * 1024):F1} MB");
Console.WriteLine($"Output: {output.Length / (1024 * 1024):F1} MB");

return 0;
