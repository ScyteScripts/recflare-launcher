using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Strategy;

public struct FileUpdateStrategy : IValidatedStruct
{
	public AppPath Path
	{
		get
		{
			if (DataStrategy.HasValue)
			{
				return DataStrategy.Value.Path;
			}
			if (PatchesStrategy.HasValue)
			{
				return PatchesStrategy.Value.Path;
			}
			throw new UnpredictedCondition("Invalid values");
		}
	}

	public FileUpdateDataStrategy? DataStrategy { get; }

	public FileUpdatePatchesStrategy? PatchesStrategy { get; }

	public bool IsValid { get; }

	public static string Validate(FileUpdateDataStrategy? dataStrategy, FileUpdatePatchesStrategy? patchesStrategy)
	{
		return null;
	}

	public static implicit operator FileUpdateStrategy(FileUpdatePatchesStrategy value)
	{
		return new FileUpdateStrategy(null, value);
	}

	public static implicit operator FileUpdateStrategy(FileUpdateDataStrategy value)
	{
		return new FileUpdateStrategy(value, null);
	}

	public FileUpdateStrategy(FileUpdateDataStrategy? dataStrategy, FileUpdatePatchesStrategy? patchesStrategy)
	{
		this = default;
		Arg.IsValid(dataStrategy, "dataStrategy");
		Arg.IsValid(patchesStrategy, "patchesStrategy");
		string text = Validate(dataStrategy, patchesStrategy);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		DataStrategy = dataStrategy;
		PatchesStrategy = patchesStrategy;
		IsValid = true;
	}

	public FileUpdateStrategy ModifyDataStrategy(FileUpdateDataStrategy? newDataStrategy)
	{
		return new FileUpdateStrategy(newDataStrategy, PatchesStrategy);
	}

	public FileUpdateStrategy ModifyPatchesStrategy(FileUpdatePatchesStrategy? newPatchesStrategy)
	{
		return new FileUpdateStrategy(DataStrategy, newPatchesStrategy);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"DataStrategy\" = {DataStrategy}, \"PatchesStrategy\" = {PatchesStrategy}";
		return text + "}";
	}
}
