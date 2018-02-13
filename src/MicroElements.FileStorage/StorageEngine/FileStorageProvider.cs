﻿// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.CodeContracts;
using MicroElements.FileStorage.Utils;

namespace MicroElements.FileStorage.StorageEngine
{
    /// <summary>
    /// FileStorageProvider.
    /// <para>Data is stored in the file system.</para>
    /// </summary>
    public class FileStorageProvider : IStorageProvider
    {
        // todo: to interfaces
        private FileStorageConfiguration _configuration;
        private readonly string _basePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageProvider"/> class.
        /// </summary>
        /// <param name="configuration">FileStorageConfiguration.</param>
        public FileStorageProvider([NotNull] FileStorageConfiguration configuration)
        {
            Check.NotNull(configuration, nameof(configuration));
            Check.NotNull(configuration.BasePath, nameof(configuration.BasePath));

            _configuration = configuration;

            _basePath = configuration.BasePath.PathNormalize();
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }

        /// <inheritdoc />
        public async Task<FileContent> ReadFile(string subPath)
        {
            Check.NotNull(subPath, nameof(subPath));

            var fullPath = GetFullPath(subPath);
            string text = string.Empty;
            if (File.Exists(fullPath))
            {
                text = await FileAsync.ReadAllText(fullPath);
            }
            return new FileContent(fullPath, text);
        }

        /// <inheritdoc />
        public IEnumerable<Task<FileContent>> ReadDirectory(string subPath)
        {
            Check.NotNull(subPath, nameof(subPath));

            var fullPath = GetFullPath(subPath);
            if (Directory.Exists(fullPath))
            {
                foreach (var fullFileName in Directory.EnumerateFiles(fullPath))
                {
                    var fullFileNameRelative = fullFileName.RelativeTo(_basePath);
                    yield return ReadFile(fullFileNameRelative);
                }
            }
        }

        /// <inheritdoc />
        public async Task WriteFile(string subPath, FileContent content)
        {
            Check.NotNull(content, nameof(content));
            Check.NotNull(subPath, nameof(subPath));

            var fullPath = GetFullPath(subPath);
            var directoryName = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            await FileAsync.WriteAllText(fullPath, content.Content);
        }

        /// <inheritdoc />
        public Task DeleteFile(string subPath)
        {
            var fullPath = GetFullPath(subPath);
            // todo: error catching
            File.Delete(fullPath);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public FileContentMetadata GetFileMetadata(string subPath)
        {
            return new FileContentMetadata();
        }

        /// <inheritdoc />
        public StorageMetadata GetStorageMetadata()
        {
            return new StorageMetadata
            {
                IsReadOnly = _configuration.IsReadOnly
            };
        }

        private string GetFullPath(string subPath)
        {
            var location = Path.Combine(_basePath, subPath);
            return location;
        }
    }
}
