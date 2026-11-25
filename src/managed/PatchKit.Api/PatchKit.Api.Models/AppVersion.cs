using Newtonsoft.Json;
using PatchKit.Core.CSharp;

namespace PatchKit.Api.Models;

public struct AppVersion
{
	[JsonProperty("id")]
	public int Id { get; }

	[JsonProperty("label")]
	public string Label { get; }

	[JsonProperty("changelog")]
	public string Changelog { get; }

	[JsonProperty("publish_date")]
	public long PublishDate { get; }

	[JsonProperty("content_guid")]
	public string ContentGuid { get; }

	[JsonProperty("content_meta_guid")]
	public string ContentMetaGuid { get; }

	[JsonProperty("diff_guid")]
	public string DiffGuid { get; }

	[JsonProperty("diff_meta_guid")]
	public string DiffMetaGuid { get; }

	[JsonProperty("draft")]
	public bool Draft { get; }

	[JsonProperty("pending_publish")]
	public bool PendingPublish { get; }

	[JsonProperty("published")]
	public bool Published { get; }

	[JsonProperty("publish_progress")]
	public float PublishProgress { get; }

	[JsonProperty("main_executable")]
	public string MainExecutable { get; }

	[JsonProperty("main_executable_args")]
	public string MainExecutableArgs { get; }

	[JsonProperty("ignored_files")]
	[JsonConverter(typeof(ImmutableArrayJsonConverter<string>))]
	public ImmutableArray<string> IgnoredFiles { get; }

	[JsonProperty("publish_when_processed")]
	public bool PublishWhenProcessed { get; }

	[JsonProperty("processing_started_at")]
	public string ProcessingStartedAt { get; }

	[JsonProperty("processing_finished_at")]
	public string ProcessingFinishedAt { get; }

	[JsonProperty("can_be_imported")]
	public bool CanBeImported { get; }

	[JsonProperty("patch_links")]
	public AppVersionPatchLinks PatchLinks { get; }

	[JsonConstructor]
	public AppVersion(int id, string label, string changelog, long publishDate, string contentGuid, string contentMetaGuid, string diffGuid, string diffMetaGuid, bool draft, bool pendingPublish, bool published, float publishProgress, string mainExecutable, string mainExecutableArgs, ImmutableArray<string> ignoredFiles, bool publishWhenProcessed, string processingStartedAt, string processingFinishedAt, bool canBeImported, AppVersionPatchLinks patchLinks)
	{
		this = default;
		Id = id;
		Label = label;
		Changelog = changelog;
		PublishDate = publishDate;
		ContentGuid = contentGuid;
		ContentMetaGuid = contentMetaGuid;
		DiffGuid = diffGuid;
		DiffMetaGuid = diffMetaGuid;
		Draft = draft;
		PendingPublish = pendingPublish;
		Published = published;
		PublishProgress = publishProgress;
		MainExecutable = mainExecutable;
		MainExecutableArgs = mainExecutableArgs;
		IgnoredFiles = ignoredFiles;
		PublishWhenProcessed = publishWhenProcessed;
		ProcessingStartedAt = processingStartedAt;
		ProcessingFinishedAt = processingFinishedAt;
		CanBeImported = canBeImported;
		PatchLinks = patchLinks;
	}
}
