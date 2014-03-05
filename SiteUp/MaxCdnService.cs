using System;
using System.Collections.Generic;
using System.Configuration;
using SiteUp.NetDna;

namespace SiteUp
{
    public class MaxCdnService
    {
        private IMaxCdnConfiguration _config;
        private readonly IMaxCdnApi _api;

        public MaxCdnService(IMaxCdnApi apiApi, IMaxCdnConfiguration configuration)
        {
            _api = apiApi;
            _config = configuration;
        }

        public void PurgeFiles(List<string> keys)
        {
            foreach (var key in keys)
            {
                var result = _api.PurgeFile(key);
                Console.WriteLine(result ? "Purged: {0}": "Unable to purge: {0}", key);
            }
        }


        public void PurgeCache()
        {
            var pullZones = _api.Get("/zones/pull.json");
            foreach (var pullZone in pullZones.data.pullzones)
            {
                _api.Delete("/zones/pull.json/" + pullZone.id + "/cache");
            }
        }
    }
}