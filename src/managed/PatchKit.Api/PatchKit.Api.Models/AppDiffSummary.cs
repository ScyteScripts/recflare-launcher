using Newtonsoft.Json;
using PatchKit.Core.CSharp;

namespace PatchKit.Api.Models;

public struct AppDiffSummary
{
	[JsonProperty("version")]
	public string Version { get; }

	[JsonProperty("size")]
	public long Size { get; }

	[JsonProperty("uncompressed_size")]
	public long UncompressedSize { get; }

	[JsonProperty("encryption_method")]
	public string EncryptionMethod { get; }

	[JsonProperty("compression_method")]
	public string CompressionMethod { get; }

	[JsonProperty("added_files")]
	[JsonConverter(typeof(ImmutableArrayJsonConverter<string>))]
	public ImmutableArray<string> AddedFiles { get; }

	[JsonProperty("modified_files")]
	[JsonConverter(typeof(ImmutableArrayJsonConverter<string>))]
	public ImmutableArray<string> ModifiedFiles { get; }

	[JsonProperty("unchanged_files")]
	[JsonConverter(typeof(ImmutableArrayJsonConverter<string>))]
	public ImmutableArray<string> UnchangedFiles { get; }

	[JsonProperty("removed_files")]
	[JsonConverter(typeof(ImmutableArrayJsonConverter<string>))]
	public ImmutableArray<string> RemovedFiles { get; }

	[JsonProperty("hash_code")]
	public string HashCode { get; }

	[JsonProperty("chunks")]
	public Chunks Chunks { get; }

	[JsonConstructor]
	public AppDiffSummary(string version, long size, long uncompressedSize, string encryptionMethod, string compressionMethod, ImmutableArray<string> addedFiles, ImmutableArray<string> modifiedFiles, ImmutableArray<string> unchangedFiles, ImmutableArray<string> removedFiles, string hashCode, Chunks chunks)
	{
		this = default;
		Version = version;
		Size = size;
		UncompressedSize = uncompressedSize;
		EncryptionMethod = encryptionMethod;
		CompressionMethod = compressionMethod;
		AddedFiles = addedFiles;
		ModifiedFiles = modifiedFiles;
		UnchangedFiles = unchangedFiles;
		RemovedFiles = removedFiles;
		HashCode = hashCode;
		Chunks = chunks;
	}
}
