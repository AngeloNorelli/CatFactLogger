using CatFactLogger.Models;
using CatFactLogger.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CatFactLogger.Tests;

public sealed class FactFileWriterTests : IDisposable {
	private readonly string _tempDirectory;

	public FactFileWriterTests() {
		_tempDirectory = Directory.CreateTempSubdirectory("CatFactLoggerTests_").FullName;
	}

	public void Dispose() {
		if (Directory.Exists(_tempDirectory)) {
			Directory.Delete(_tempDirectory, recursive: true);
		}
	}

	private FactFileWriter CreateWriter(string fileName = "cat_facts.txt") {
		var fullPath = Path.Combine(_tempDirectory, fileName);
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string?> {
				["OutputFile:Path"] = fullPath
			})
			.Build();

		return new FactFileWriter(configuration);
	}

	[Theory]
	[InlineData("cat_facts.txt")]
	[InlineData("facts.txt")]
	[InlineData("temp.txt")]
	public void Constructor_CreateFile_WhenItDoesNotExist(string fileName) {
		var writer = CreateWriter(fileName);
		Assert.True(File.Exists(writer.FilePath));
	}

	[Fact]
	public async Task AppendFactAsync_WritesFactAndLength() {
		var writer = CreateWriter();
		var fact = new CatFact { Fact = "Cats have 32 muscles in each ear.", Length = 32 };

		await writer.AppendFactAsync(fact);

		var lines = await writer.ReadAllLinesAsync();
		Assert.Single(lines);
		Assert.Contains("length= 32", lines[0]);
		Assert.Contains("Cats have 32 muscles in each ear.", lines[0]);
	}

	[Fact]
	public async Task AppendFactAsync_AppendNewLine_OnEachCall_WithoutOverwritingPrevious() 
	{
		var writer = CreateWriter();
		
		await writer.AppendFactAsync(new CatFact { Fact = "First fact.", Length = 11 });
		await writer.AppendFactAsync(new CatFact { Fact = "Second fact.", Length = 12 });

		var lines = await writer.ReadAllLinesAsync();

		Assert.Equal(2, lines.Count);
		Assert.Contains("First fact.", lines[0]);
		Assert.Contains("Second fact.", lines[1]);
	}

	[Fact]
	public async Task ReadAllLinesAsync_ReturnsEmpty_WhenFileWasJustCreated() 
	{
		var writer = CreateWriter();
		
		var lines = await writer.ReadAllLinesAsync();

		Assert.Empty(lines);
	}

	[Fact]
	public void Contruct_Throws_WhenConfiguredDirectoryDoesNotExist() 
	{
		var missingDirPath = Path.Combine(_tempDirectory, "nonexistent", "cat_facts.txt");

		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string?> 
			{
				["OutputFile:Path"] = missingDirPath
			})
			.Build();

		Assert.Throws<InvalidOperationException>(() => new FactFileWriter(configuration));
	}
}