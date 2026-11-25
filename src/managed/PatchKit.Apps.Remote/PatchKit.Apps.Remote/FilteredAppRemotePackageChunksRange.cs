using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct FilteredAppRemotePackageChunksRange : IValidatedStruct
{
	public long StartChunkIndex { get; }

	public long EndChunkIndex { get; }

	public bool IsValid { get; }

	public static string Validate(long startChunkIndex, long endChunkIndex)
	{
		if (startChunkIndex < 0)
		{
			return "startChunkIndex cannot be less than zero.";
		}
		if (endChunkIndex < 0)
		{
			return "startChunkIndex cannot be less than zero.";
		}
		if (endChunkIndex < startChunkIndex)
		{
			return "endChunkIndex cannot be less than startChunkIndex.";
		}
		return null;
	}

	public FilteredAppRemotePackageChunksRange(long startChunkIndex, long endChunkIndex)
	{
		this = default;
		string text = Validate(startChunkIndex, endChunkIndex);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		StartChunkIndex = startChunkIndex;
		EndChunkIndex = endChunkIndex;
		IsValid = true;
	}

	public FilteredAppRemotePackageChunksRange ModifyStartChunkIndex(long newStartChunkIndex)
	{
		return new FilteredAppRemotePackageChunksRange(newStartChunkIndex, EndChunkIndex);
	}

	public FilteredAppRemotePackageChunksRange ModifyEndChunkIndex(long newEndChunkIndex)
	{
		return new FilteredAppRemotePackageChunksRange(StartChunkIndex, newEndChunkIndex);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"StartChunkIndex\" = {StartChunkIndex}, \"EndChunkIndex\" = {EndChunkIndex}";
		return text + "}";
	}
}
