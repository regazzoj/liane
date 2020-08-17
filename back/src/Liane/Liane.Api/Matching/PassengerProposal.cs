using System.Collections.Immutable;
using Liane.Api.Routing;

namespace Liane.Api.Matching
{
    public sealed class PassengerProposal
    {
        public PassengerProposal(string passengerId, ImmutableList<LatLng> latLngs)
        {
            PassengerId = passengerId;
            LatLngs = latLngs;
        }
        public string PassengerId { get; }
        public ImmutableList<LatLng> LatLngs { get; }
    }
}