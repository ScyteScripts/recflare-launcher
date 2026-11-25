using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Strategy;

public struct UpdateStrategy : IValidatedStruct
{
	public ImmutableArray<AppPath> DirsToCreate { get; }

	public ImmutableArray<FileUpdateStrategy> FileStrategies { get; }

	public ImmutableArray<AppPath> EntriesToDelete { get; }

	public bool IsValid { get; }

	public static string Validate(ImmutableArray<AppPath> dirsToCreate, ImmutableArray<FileUpdateStrategy> fileStrategies, ImmutableArray<AppPath> entriesToDelete)
	{
		return null;
	}

	public UpdateStrategy(ImmutableArray<AppPath> dirsToCreate, ImmutableArray<FileUpdateStrategy> fileStrategies, ImmutableArray<AppPath> entriesToDelete)
	{
		this = default;
		Arg.IsValid(dirsToCreate, "dirsToCreate");
		Arg.IsValid(fileStrategies, "fileStrategies");
		Arg.IsValid(entriesToDelete, "entriesToDelete");
		string text = Validate(dirsToCreate, fileStrategies, entriesToDelete);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		DirsToCreate = dirsToCreate;
		FileStrategies = fileStrategies;
		EntriesToDelete = entriesToDelete;
		IsValid = true;
	}

	public UpdateStrategy ModifyDirsToCreate(ImmutableArray<AppPath> newDirsToCreate)
	{
		return new UpdateStrategy(newDirsToCreate, FileStrategies, EntriesToDelete);
	}

	public UpdateStrategy ModifyFileStrategies(ImmutableArray<FileUpdateStrategy> newFileStrategies)
	{
		return new UpdateStrategy(DirsToCreate, newFileStrategies, EntriesToDelete);
	}

	public UpdateStrategy ModifyEntriesToDelete(ImmutableArray<AppPath> newEntriesToDelete)
	{
		return new UpdateStrategy(DirsToCreate, FileStrategies, newEntriesToDelete);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"DirsToCreate\" = {DirsToCreate}, \"FileStrategies\" = {FileStrategies}, \"EntriesToDelete\" = {EntriesToDelete}";
		return text + "}";
	}
}
