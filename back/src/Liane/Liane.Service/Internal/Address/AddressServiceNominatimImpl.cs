using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Liane.Api.Address;
using Liane.Api.Routing;
using Liane.Api.Util.Exception;
using Liane.Api.Util.Http;
using Liane.Service.Internal.Nominatim;
using Microsoft.Extensions.Logging;

namespace Liane.Service.Internal.Address
{
    public sealed class AddressServiceNominatimImpl : IAddressService
    {
        private readonly HttpClient client;
        private readonly ILogger<AddressServiceNominatimImpl> logger;

        public AddressServiceNominatimImpl(ILogger<AddressServiceNominatimImpl> logger)
        {
            client = new HttpClient();
            this.logger = logger;
        }

        public async Task<Api.Address.Address> GetDisplayName(LatLng coordinate)
        {
            const string uri = "http://liane.gjini.co:7070/reverse";

            var response = await client.GetAsyncAs<Response>(uri, new
            {
                lat = coordinate.Lat,
                lon = coordinate.Lng,
                format = "json",
                addressdetails = 1
            });

            return MapAddress(response);
        }

        public async Task<Api.Address.Address> GetCoordinate(string displayName)
        {
            var addresses = await Search(displayName);
            if (addresses.IsEmpty)
            {
                throw new ResourceNotFoundException($"Address '{displayName}' not found");
            }

            return addresses[0];
        }

        public async Task<ImmutableList<Api.Address.Address>> Search(string displayName)
        {
            const string uri = "http://liane.gjini.co:7070/search/fr";

            var responses = await client.GetAsyncAs<ImmutableList<Response>>(uri, new
            {
                q = displayName,
                format = "json",
                addressdetails = 1
            });

            logger.LogInformation("Call returns ", responses);

            return responses.Select(MapAddress)
                .ToImmutableList();
        }

        private static Api.Address.Address MapAddress(Response r)
        {
            return new Api.Address.Address(r.DisplayName, new LatLng(r.Lat, r.Lon), r.Address);
        }
    }
}