# CodingChallenge

A small .NET 8 solution implementing an LRU (Least Recently Used) cache and unit tests demonstrating correct behavior and thread-safety.

## Repository structure

- CodingChallenge/ - main library project
  - ICache.cs - cache interface
  - LRUCache.cs - LRU cache implementation
- CodingChallenge.Tests/ - MSTest unit tests covering correctness, edge cases and concurrency

## Overview

This solution provides a simple, performant LRU cache implementation (LRUCache) that implements ICache and IDisposable. It is designed to be:
- Fixed-capacity (evicts least recently used item when full)
- Thread-safe (uses ReaderWriterLockSlim internally)
- Small and well-tested with unit tests covering basic usage, edge cases, and concurrency

Target framework: .NET 8

## Public API

ICache (interface)
- string Get(int key)
  - Retrieve the value for the given key. Implementations may return null or empty string when the key does not exist.
- void Put(int key, string value)
  - Insert or update the value for a given key.
- void Clear()
  - Remove all entries from the cache.

LRUCache (class, implements ICache, IDisposable)
- Constructor: LRUCache(int capacity)
  - capacity must be >= 1, otherwise ArgumentException is thrown.
- string Get(int key)
  - Returns the stored value for the key. The implementation moves the accessed item to the most-recently-used position.
  - Note: the current implementation returns string.Empty when a key does not exist (tests assert this behavior).
- void Put(int key, string value)
  - Insert or update a key; updates also move the entry to most-recently-used. When capacity is reached, the least-recently-used entry is evicted.
- void Clear()
  - Clears the cache.
- void Dispose()
  - Releases internal synchronization primitives. After Dispose, calling Get/Put should throw ObjectDisposedException (tests validate this behavior).

Thread-safety: LRUCache uses a ReaderWriterLockSlim to allow concurrent reads/upgrades and safe writes for Put/eviction.

## Example

Typical usage:

```csharp
var cache = new LRUCache(2);
cache.Put(1, "one");
cache.Put(2, "two");
var v1 = cache.Get(1); // "one"
cache.Put(3, "three"); // evicts key 2
```

## Build and test

Requirements: .NET 8 SDK

From the repository root you can use the CLI:

- Build the solution:
  dotnet build
- Run unit tests:
  dotnet test

Or open the solution in Visual Studio 2022/2026 and run the tests from Test Explorer.

## Notes and implementation details

- The ICache interface documentation indicates Get may return null for missing keys, but the current LRUCache returns string.Empty when a key is missing; tests rely on string.Empty for non-existent keys in many cases. Storing explicit null values is supported (tests exercise null storage).
- The cache keys are int and values are string in this implementation; the design is minimal for the coding-challenge scope and can be generalized if needed.

## Contributing

Feel free to open issues or PRs. If you intend to generalize the cache (generic types, size-based eviction, async-friendly APIs), include unit tests and update README accordingly.

## License

This repository does not include a license file. Add a LICENSE file if you want to publish under an open-source license.
