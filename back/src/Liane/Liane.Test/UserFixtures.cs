using System;
using System.Collections.Immutable;
using Liane.Api.Matching;
using Liane.Api.Routing;

namespace Liane.Test
{
    public enum OnePassengerAlreadyMatchingCase
    {
        Matching,
        OldPassengerTooLate,
        NewPassengerTooLate,
        TooLongTravel
    }

    public enum OnePassengerAlreadySameEnd
    {
        None,
        All,
        OldAndNewPassengers,
        NewPassengerAndDriver,
        OldPassengerAndDriver
    }

    public static class Drivers
    {
        public static readonly Driver Driver = new Driver(Fixtures.Mende, Fixtures.Florac, 1200, Fixtures.SeptAoutMatin, ImmutableList<Passenger>.Empty, ImmutableList<LatLng>.Empty);
    }

    public static class PassengersAnotherEnd
    {
        public static readonly Passenger Matching = new Passenger(Fixtures.LeCrouzet, Fixtures.Cocures, Fixtures.SeptAoutSoir);
        public static readonly Passenger TooLate = new Passenger(Fixtures.LeCrouzet, Fixtures.Cocures, Fixtures.SeptAoutMatin.AddMinutes(15));
        public static readonly Passenger TooLongDetour = new Passenger(Fixtures.LeCrouzet, Fixtures.Rampon, Fixtures.SeptAoutSoir);
    }

    public static class PassengersSameEnd
    {
        public static readonly Passenger TooLongDetour = new Passenger(Fixtures.GorgesDuTarnCausses, Fixtures.Florac, Fixtures.SeptAoutSoir);
        public static readonly Passenger Matching = new Passenger(Fixtures.LeCrouzet, Fixtures.Florac, Fixtures.SeptAoutSoir);
        public static readonly Passenger TooLate = new Passenger(Fixtures.LeCrouzet, Fixtures.Florac, Fixtures.SeptAoutMatin.AddMinutes(15));
    }

    public static class TakeNewPassengerDropOldPassenger
    {
        public static Passenger GetNewPassenger(LatLng end, Boolean isLate = false)
        {
            Passenger oldPassenger = new Passenger(
                Fixtures.LeCrouzet,
                end,
                isLate ? Fixtures.SeptAoutMatin.AddMinutes(30) : Fixtures.SeptAoutSoir
            );
            return oldPassenger;
        }

        public static Passenger GetOldPassenger(LatLng end, Boolean isLate = false)
        {
            Passenger oldPassenger = new Passenger(
                Fixtures.SaintEtienneDuValdonnez,
                end,
                isLate ? Fixtures.SeptAoutMatin.AddMinutes(30) : Fixtures.SeptAoutSoir,
                true
            );
            return oldPassenger;
        }

        public static Driver GetDriver(Passenger oldPassenger, Boolean tooLongTravel = false)
        {
            var driver = new Driver(
                Fixtures.Mende, Fixtures.Florac,
                tooLongTravel ? 500 : 1200, Fixtures.SeptAoutMatin,
                ImmutableList.Create(oldPassenger),
                ImmutableList.Create(oldPassenger.Start, oldPassenger.End));
            return driver;
        }

        public static ImmutableList<User> GetUsers(OnePassengerAlreadyMatchingCase scenario, OnePassengerAlreadySameEnd sameEnd)
        {
            LatLng oldPassengerEnd = Fixtures.Rampon;
            LatLng newPassengerEnd = Fixtures.Cocures;
            switch (sameEnd)
            {
                case OnePassengerAlreadySameEnd.All:
                    oldPassengerEnd = Fixtures.Florac;
                    newPassengerEnd = Fixtures.Florac;
                    break;
                case OnePassengerAlreadySameEnd.OldAndNewPassengers:
                    newPassengerEnd = oldPassengerEnd;
                    break;
                case OnePassengerAlreadySameEnd.NewPassengerAndDriver:
                    newPassengerEnd = Fixtures.Florac;
                    break;
                case OnePassengerAlreadySameEnd.OldPassengerAndDriver:
                    throw new Exception();
            }

            var oldPassenger = GetOldPassenger(oldPassengerEnd);
            var newPassenger = GetNewPassenger(newPassengerEnd);
            var driver = GetDriver(oldPassenger);

            switch (scenario)
            {
                case OnePassengerAlreadyMatchingCase.OldPassengerTooLate:
                    oldPassenger = GetOldPassenger(oldPassengerEnd, true);
                    driver = GetDriver(oldPassenger);
                    break;
                case OnePassengerAlreadyMatchingCase.NewPassengerTooLate:
                    newPassenger = GetNewPassenger(newPassengerEnd, true);
                    break;
                case OnePassengerAlreadyMatchingCase.TooLongTravel:
                    driver = GetDriver(oldPassenger, true);
                    break;
            }

            return ImmutableList.Create<User>(driver, oldPassenger, newPassenger);
        }
    }

    public static class DropOldPassengerTakeNewPassenger
    {
        public static Passenger GetNewPassenger(LatLng end, Boolean isLate = false)
        {
            Passenger newPassenger = new Passenger(
                Fixtures.Cocures,
                end,
                isLate ? Fixtures.SeptAoutMatin.AddMinutes(30) : Fixtures.SeptAoutSoir
            );
            return newPassenger;
        }

        public static Passenger GetOldPassenger(LatLng end, Boolean isLate = false)
        {
            Passenger oldPassenger = new Passenger(
                Fixtures.SaintEtienneDuValdonnez,
                end,
                isLate ? Fixtures.SeptAoutMatin.AddMinutes(30) : Fixtures.SeptAoutSoir,
                true
            );
            return oldPassenger;
        }

        public static Driver GetDriver(Passenger oldPassenger, Boolean tooLongTravel = false)
        {
            var driver = new Driver(
                Fixtures.Mende, Fixtures.Florac,
                tooLongTravel ? 100 : 1200, Fixtures.SeptAoutMatin,
                ImmutableList.Create(oldPassenger),
                ImmutableList.Create(oldPassenger.Start, oldPassenger.End));
            return driver;
        }

        public static ImmutableList<User> GetUsers(OnePassengerAlreadyMatchingCase scenario, OnePassengerAlreadySameEnd sameEnd)
        {
            LatLng oldPassengerEnd = Fixtures.LeCrouzet;
            LatLng newPassengerEnd = Fixtures.Rampon;
            switch (sameEnd)
            {
                case OnePassengerAlreadySameEnd.All:
                    // oldPassengerEnd = Fixtures.Florac;
                    // newPassengerEnd = Fixtures.Florac;
                    // break;
                    throw new Exception($"SameEnd scenario: {sameEnd} not supported here");
                case OnePassengerAlreadySameEnd.OldAndNewPassengers:
                    throw new Exception($"SameEnd scenario: {sameEnd} not supported here");
                case OnePassengerAlreadySameEnd.NewPassengerAndDriver:
                    newPassengerEnd = Fixtures.Florac;
                    break;
                case OnePassengerAlreadySameEnd.OldPassengerAndDriver:
                    throw new Exception($"SameEnd scenario: {sameEnd} not supported here");
            }

            var oldPassenger = GetOldPassenger(oldPassengerEnd);
            var newPassenger = GetNewPassenger(newPassengerEnd);
            var driver = GetDriver(oldPassenger);

            switch (scenario)
            {
                case OnePassengerAlreadyMatchingCase.OldPassengerTooLate:
                    oldPassenger = GetOldPassenger(oldPassengerEnd, true);
                    driver = GetDriver(oldPassenger);
                    break;
                case OnePassengerAlreadyMatchingCase.NewPassengerTooLate:
                    newPassenger = GetNewPassenger(newPassengerEnd, true);
                    break;
                case OnePassengerAlreadyMatchingCase.TooLongTravel:
                    driver = GetDriver(oldPassenger, true);
                    break;
            }

            return ImmutableList.Create<User>(driver, oldPassenger, newPassenger);
        }
    }

    public static class TakeNewPassengerDropNewPassenger
    {
        public static Passenger GetNewPassenger(LatLng end, Boolean isLate = false)
        {
            Passenger newPassenger = new Passenger(
                Fixtures.LeCrouzet,
                end,
                isLate ? Fixtures.SeptAoutMatin.AddMinutes(30) : Fixtures.SeptAoutSoir
            );
            return newPassenger;
        }

        public static Passenger GetOldPassenger(LatLng end, Boolean isLate = false)
        {
            Passenger oldPassenger = new Passenger(
                Fixtures.SaintEtienneDuValdonnez,
                end,
                isLate ? Fixtures.SeptAoutMatin.AddMinutes(30) : Fixtures.SeptAoutSoir,
                true
            );
            return oldPassenger;
        }

        public static Driver GetDriver(Passenger oldPassenger, Boolean tooLongTravel = false)
        {
            var driver = new Driver(
                Fixtures.Mende, Fixtures.Florac,
                tooLongTravel ? 500 : 1200, Fixtures.SeptAoutMatin,
                ImmutableList.Create(oldPassenger),
                ImmutableList.Create(oldPassenger.Start, oldPassenger.End));
            return driver;
        }

        public static ImmutableList<User> GetUsers(OnePassengerAlreadyMatchingCase scenario, OnePassengerAlreadySameEnd sameEnd)
        {
            LatLng oldPassengerEnd = Fixtures.Rampon;
            LatLng newPassengerEnd = Fixtures.Cocures;
            switch (sameEnd)
            {
                case OnePassengerAlreadySameEnd.All:
                    oldPassengerEnd = Fixtures.Florac;
                    newPassengerEnd = Fixtures.Florac;
                    break;
                case OnePassengerAlreadySameEnd.OldAndNewPassengers:
                    newPassengerEnd = oldPassengerEnd;
                    break;
                case OnePassengerAlreadySameEnd.NewPassengerAndDriver:
                    throw new Exception($"SameEnd scenario: {sameEnd} not supported here");
                case OnePassengerAlreadySameEnd.OldPassengerAndDriver:
                    oldPassengerEnd = Fixtures.Florac;
                    break;
            }

            var oldPassenger = GetOldPassenger(oldPassengerEnd);
            var newPassenger = GetNewPassenger(newPassengerEnd);
            var driver = GetDriver(oldPassenger);

            switch (scenario)
            {
                case OnePassengerAlreadyMatchingCase.OldPassengerTooLate:
                    oldPassenger = GetOldPassenger(oldPassengerEnd, true);
                    driver = GetDriver(oldPassenger);
                    break;
                case OnePassengerAlreadyMatchingCase.NewPassengerTooLate:
                    newPassenger = GetNewPassenger(newPassengerEnd, true);
                    break;
                case OnePassengerAlreadyMatchingCase.TooLongTravel:
                    driver = GetDriver(oldPassenger, true);
                    break;
            }

            return ImmutableList.Create<User>(driver, oldPassenger, newPassenger);
        }
    }
}