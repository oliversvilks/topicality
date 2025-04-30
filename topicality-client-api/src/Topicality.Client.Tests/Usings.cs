global using Xunit;
global using Moq;
global using FluentAssertions;

// Temporary disable due to one file example
[assembly: CollectionBehavior(MaxParallelThreads = 1)]