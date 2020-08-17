using System;
using System.Collections.Immutable;
using System.Linq;
using Liane.Api.Routing;

namespace Liane.Api.Matching
{
    public sealed class Driver : User
    {
        public Driver(LatLng start, LatLng end, float maxDelta, DateTime departureTime, ImmutableList<Passenger> passengers, ImmutableList<LatLng> latLngs)
        {
            Start = start;
            End = end;
            MaxDelta = maxDelta;
            DepartureTime = departureTime;
            Passengers = passengers;
            LatLngs = ImmutableList.Create(start).AddRange(latLngs).Add(end);
            LatLngs = LatLngs.Distinct().ToImmutableList();
            NbOfSeat = 1;
        }

        public LatLng Start { get; }
        public LatLng End { get; }
        public float MaxDelta { get; }
        public int NbOfSeat { get; }
        public DateTime DepartureTime { get; }
        public ImmutableList<Passenger> Passengers { get; }
        public ImmutableList<LatLng> LatLngs { get; }

        private bool Equals(Driver other)
        {
            return Start.Equals(other.Start) && End.Equals(other.End) && MaxDelta.Equals(other.MaxDelta) && NbOfSeat == other.NbOfSeat && DepartureTime.Equals(other.DepartureTime);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is Driver other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End, MaxDelta, NbOfSeat, DepartureTime);
        }
    }
}