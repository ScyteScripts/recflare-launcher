using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public interface ITempDir : IBaseDisposable
{
	Path Path { get; }
}
