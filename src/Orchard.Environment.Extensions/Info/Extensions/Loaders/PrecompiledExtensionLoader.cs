﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orchard.Environment.Extensions.Folders;
using System.Linq;

namespace Orchard.Environment.Extensions.Info.Extensions.Loaders
{
    public class PrecompiledExtensionLoader : IExtensionLoader
    {
        private readonly string[] ExtensionsSearchPaths;

        private readonly IExtensionLibraryService _extensionLibraryService;
        private readonly ILogger _logger;

        public PrecompiledExtensionLoader(
            IOptions<ExtensionHarvestingOptions> optionsAccessor,
            IExtensionLibraryService extensionLibraryService,
            ILogger<PrecompiledExtensionLoader> logger)
        {
            ExtensionsSearchPaths = optionsAccessor.Value.ExtensionLocationExpanders.SelectMany(x => x.SearchPaths).ToArray();
            _extensionLibraryService = extensionLibraryService;
            _logger = logger;
        }

        public string Name => GetType().Name;

        public int Order => 30;

        public ExtensionEntry Load(IExtensionInfo extensionInfo)
        {
            if (!ExtensionsSearchPaths.Contains(extensionInfo.ExtensionFileInfo.PhysicalPath))
            {
                return null;
            }

            try
            {
                var assembly = _extensionLibraryService.LoadPrecompiledExtension(extensionInfo);
            
                if (assembly == null)
                {
                    return null;
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Loaded referenced precompiled extension \"{0}\": assembly name=\"{1}\"", extensionInfo.Id, assembly.FullName);
                }

                return new ExtensionEntry
                {
                    ExtensionInfo = extensionInfo,
                    Assembly = assembly,
                    ExportedTypes = assembly.ExportedTypes
                };
            }
            catch
            {
                return null;
            }
       }
    }
}