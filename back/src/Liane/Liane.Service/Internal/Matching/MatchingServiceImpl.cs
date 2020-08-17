using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Liane.Api.Matching;
using Liane.Api.Routing;
using Liane.Service.Internal.Osrm;

namespace Liane.Service.Internal.Matching
{
    public class MatchingServiceImpl : IMatchingService
    {
        private readonly IOsrmService osrmService;
        private readonly IUserService userService;

        public MatchingServiceImpl(IOsrmService osrmService, IUserService userService)
        {
            this.osrmService = osrmService;
            this.userService = userService;
        }

        public async Task<ImmutableList<PassengerProposal>> SearchPassengers(string userId)
        {
            var result = ImmutableList<PassengerProposal>.Empty;
            var driver = await userService.GetDriver(userId);
            var passengers = await userService.GetAllPassengers();


            if (driver.NbOfSeat == 0)
            {
                return result;
            }

            var route = await osrmService.Route(driver.LatLngs, overview: "false");
            foreach (var passenger in passengers)
            {
                if (passenger.HasMatch)
                {
                    continue;
                }

                var bestSequence = ImmutableList<LatLng>.Empty;
                Osrm.Response.Routing routeWithWaypoint;
                if (driver.Passengers.IsEmpty)
                {
                    if (passenger.End.Equals(driver.End))
                    {
                        bestSequence = ImmutableList.Create(driver.Start, passenger.Start, driver.End);
                    }
                    else
                    {
                        bestSequence = ImmutableList.Create(driver.Start, passenger.Start, passenger.End, driver.End);
                    }

                    routeWithWaypoint = await osrmService.Route(bestSequence, overview: "false");

                    var delta = routeWithWaypoint.Routes[0].Duration - route.Routes[0].Duration;
                    if (delta > driver.MaxDelta || delta < 0)
                    {
                        continue;
                    }

                    var endIndicePassenger = bestSequence.FindIndex(p => p.Equals(passenger.End));
                    var duration = routeWithWaypoint.Routes[0].Legs.Where((l, i) => i < endIndicePassenger).Sum(l => l.Duration);

                    var currentArrivalTime = driver.DepartureTime.AddSeconds(duration);
                    if (currentArrivalTime > passenger.MaxArrivalTime)
                    {
                        continue;
                    }
                }
                else
                {
                    var sequences = GenerateAllPossibleSequences(driver, passenger);

                    float minDelta = -1;
                    float delta;
                    var routeWithWaypoints = ImmutableList<Osrm.Response.Routing>.Empty;
                    foreach (var latLngs in sequences)
                    {
                        routeWithWaypoint = await osrmService.Route(latLngs, overview: "false");
                        routeWithWaypoints.Add(routeWithWaypoint);

                        delta = routeWithWaypoint.Routes[0].Duration - route.Routes[0].Duration;
                        if (delta > driver.MaxDelta || delta < 0)
                        {
                            continue;
                        }

                        // Checking arrival time of every passenger if this new one is taken
                        var someoneArriveTooLate = false;
                        var finalPassengersList = driver.Passengers.Add(passenger);
                        foreach (var p in finalPassengersList)
                        {
                            var endIndicePassenger = latLngs.FindIndex(pa => pa.Equals(p.End));
                            var duration = routeWithWaypoint.Routes[0].Legs.Where((l, i) => i < endIndicePassenger).Sum(l => l.Duration);

                            var currentArrivalTime = driver.DepartureTime.AddSeconds(duration);
                            if (currentArrivalTime > p.MaxArrivalTime)
                            {
                                someoneArriveTooLate = true;
                                break;
                            }
                        }

                        if (someoneArriveTooLate)
                        {
                            continue;
                        }

                        if (minDelta < 0 || minDelta > delta)
                        {
                            minDelta = delta;
                            bestSequence = latLngs;
                        }
                    }

                    if (minDelta < 0)
                    {
                        continue;
                    }
                }


                var id = await userService.GetId(passenger);
                result = result.Add(new PassengerProposal(id ?? "noId found", bestSequence));
            }

            return result;
        }

        private ImmutableList<ImmutableList<LatLng>> GenerateAllPossibleSequences(Driver driver, Passenger passenger)
        {
            var results = ImmutableList<ImmutableList<LatLng>>.Empty;

            var nbOfWaypoints = driver.LatLngs.Count;

            for (var i = driver.Passengers.Count + 1; i < nbOfWaypoints; i++)
            {
                for (var j = i; j < nbOfWaypoints; j++)
                {
                    var sequence = driver.LatLngs.GetRange(0, i);
                    sequence = sequence.Add(passenger.Start);
                    sequence = sequence.AddRange(driver.LatLngs.GetRange(i, j - i));
                    sequence = sequence.Add(passenger.End);
                    sequence = sequence.AddRange(driver.LatLngs.GetRange(j, nbOfWaypoints - j));

                    var makeARoundTrip = false;
                    for (var index = 0; index < sequence.Count; index++)
                    {
                        var value = sequence[index];
                        var maxIndexDiff = sequence.Count(p => value.Equals(p)) - 1;
                        var indexFirstOccurence = sequence.FindIndex(p => value.Equals(p));
                        var indexLastOccurence = sequence.LastIndexOf(sequence[index]);
                        
                        if ( indexLastOccurence - index > maxIndexDiff ||  indexFirstOccurence - index > maxIndexDiff)
                        {
                            makeARoundTrip = true;
                            break;
                        }
                    }

                    if (makeARoundTrip)
                    {
                        continue;
                    }


                    if (!results.Contains(sequence))
                    {
                        sequence = sequence.Distinct().ToImmutableList();
                        results = results.Add(sequence);
                    }
                }
            }

            return results;
        }


        public Task<ImmutableList<DriverProposal>> SearchDrivers(string userId)
        {
            return Task.FromResult(ImmutableList<DriverProposal>.Empty);
        }
    }
}