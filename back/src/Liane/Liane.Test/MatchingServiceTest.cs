using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Liane.Api.Matching;
using Liane.Api.Routing;
using Liane.Service.Internal.Matching;
using Liane.Service.Internal.Osrm;
using Liane.Service.Internal.Osrm.Response;
using Liane.Test.Util;
using Moq;
using NUnit.Framework;

namespace Liane.Test
{
    [TestFixture]
    public sealed class MatchingServiceTest
    {
        private readonly MatchingServiceImpl tested;

        private readonly UserServiceImpl userServiceImpl;

        public MatchingServiceTest()
        {
            var mock = new Mock<IOsrmService>();

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.Florac),
                "mende-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.LeCrouzet, Fixtures.Florac),
                "mende-leCrouzet-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.GorgesDuTarnCausses, Fixtures.Florac),
                "mende-gorgesDuTarnCausses-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.GorgesDuTarnCausses, Fixtures.Prades, Fixtures.Florac),
                "mende-gorgesDuTarnCausses-prades-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.LeCrouzet, Fixtures.Cocures, Fixtures.Florac),
                "mende-leCrouzet-cocures-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.LeCrouzet, Fixtures.Rampon, Fixtures.Florac),
                "mende-leCrouzet-rampon-florac.json");


            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.Cocures, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-cocures-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.Cocures, Fixtures.LeCrouzet, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-cocures-leCrouzet-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.Cocures, Fixtures.LeCrouzet, Fixtures.Rampon, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-cocures-leCrouzet-rampon-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.Cocures, Fixtures.Rampon, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-cocures-rampon-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.Cocures, Fixtures.Rampon, Fixtures.LeCrouzet, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-cocures-rampon-leCrouzet-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-florac.json");
            
            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.LeCrouzet, Fixtures.Cocures, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-leCrouzet-cocures-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.LeCrouzet, Fixtures.Cocures, Fixtures.Rampon, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-leCrouzet-cocures-rampon-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.LeCrouzet, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-leCrouzet-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.LeCrouzet, Fixtures.Rampon, Fixtures.Cocures, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-leCrouzet-rampon-cocures-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.LeCrouzet, Fixtures.Rampon, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-leCrouzet-rampon-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.Rampon, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-rampon-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.Rampon, Fixtures.LeCrouzet, Fixtures.Cocures, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-rampon-leCrouzet-cocures-florac.json");

            SetupRouteMock(mock,
                ImmutableList.Create(Fixtures.Mende, Fixtures.SaintEtienneDuValdonnez, Fixtures.Rampon, Fixtures.LeCrouzet, Fixtures.Florac),
                "mende-saintEtienneDuValdonnez-rampon-leCrouzet-florac.json");
            

            userServiceImpl = new UserServiceImpl();
            tested = new MatchingServiceImpl(mock.Object, userServiceImpl);
        }

        private static void SetupRouteMock(Mock<IOsrmService> mock, ImmutableList<LatLng> input, string file)
        {
            mock.Setup(service =>
                    service.Route(
                        It.Is<ImmutableList<LatLng>>(l => input.SequenceEqual(l)),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .ReturnsAsync(() => AssertJson.ReadJson<Routing>(file));
        }

        [Test]
        public void ShouldMatchNoDriver()
        {
            tested.SearchDrivers("");
        }


        [Test]
        public async Task ShouldNotMatch_SameEndTooLongDetour()
        {
            userServiceImpl.EmptyUsersList();
            userServiceImpl.AddUser("Conducteur", Drivers.Driver);
            userServiceImpl.AddUser("Passager gorgesDuTarnCausses, trop long détour", PassengersSameEnd.TooLongDetour);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_SameEndArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            userServiceImpl.AddUser("Conducteur", Drivers.Driver);
            userServiceImpl.AddUser("Passager leCrouzet, en retard", PassengersSameEnd.TooLate);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldMatch_SameEnd()
        {
            userServiceImpl.EmptyUsersList();
            userServiceImpl.AddUser("Conducteur", Drivers.Driver);
            userServiceImpl.AddUser("Passager leCrouzet, à l'heure", PassengersSameEnd.Matching);

            var passengers = await tested.SearchPassengers("Conducteur");

            Assert.AreEqual(1, passengers.Count);
            Assert.AreEqual("Passager leCrouzet, à l'heure", passengers[0].PassengerId);
        }


        [Test]
        public async Task ShouldNotMatch_AnotherEndTooLongDetour()
        {
            userServiceImpl.EmptyUsersList();
            userServiceImpl.AddUser("Conducteur", Drivers.Driver);
            userServiceImpl.AddUser("Passager gorgesDuTarnCausses-prades, trop long détour", PassengersAnotherEnd.TooLongDetour);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_AnotherEndArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            userServiceImpl.AddUser("Conducteur", Drivers.Driver);
            userServiceImpl.AddUser("Passager leCrouzet-cocures, en retard", PassengersAnotherEnd.TooLate);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldMatch_AnotherEnd()
        {
            userServiceImpl.EmptyUsersList();
            userServiceImpl.AddUser("Conducteur", Drivers.Driver);
            userServiceImpl.AddUser("Passager leCrouzet-cocures, à l'heure", PassengersAnotherEnd.Matching);

            var passengers = await tested.SearchPassengers("Conducteur");

            Assert.AreEqual(1, passengers.Count);
            Assert.AreEqual("Passager leCrouzet-cocures, à l'heure", passengers[0].PassengerId);
        }


        [Test]
        public async Task ShouldMatch_TakeNewPassengerDropOldPassenger_AllSameEnd()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.Matching, OnePassengerAlreadySameEnd.All);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            Assert.AreEqual(1, passengers.Count);
            Assert.AreEqual("Passager 2, potentiel nouveau passager", passengers[0].PassengerId);
            
            var d = users[0] as Driver;
            var p1 = users[1] as Passenger;
            var p2 = users[2] as Passenger;
            CollectionAssert.AreEqual(ImmutableList.Create(d.Start,p1.Start,p2.Start,d.End),passengers[0].LatLngs);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropOldPassenger_AllSameEndOldArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.OldPassengerTooLate, OnePassengerAlreadySameEnd.All);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropOldPassenger_AllSameEndNewArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.NewPassengerTooLate, OnePassengerAlreadySameEnd.All);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropOldPassenger_AllSameEndTooLong()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.TooLongTravel, OnePassengerAlreadySameEnd.All);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }


        [Test]
        public async Task ShouldMatch_TakeNewPassengerDropOldPassenger_NewPassengerAndDriverSameEnd()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.Matching, OnePassengerAlreadySameEnd.NewPassengerAndDriver);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            Assert.AreEqual(1, passengers.Count);
            Assert.AreEqual("Passager 2, potentiel nouveau passager", passengers[0].PassengerId);
            
            var d = users[0] as Driver;
            var p1 = users[1] as Passenger;
            var p2 = users[2] as Passenger;
            CollectionAssert.AreEqual(ImmutableList.Create(d.Start,p1.Start,p2.Start,p1.End,d.End),passengers[0].LatLngs);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropOldPassenger_NewPassengerAndDriverSameEndOldArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.OldPassengerTooLate, OnePassengerAlreadySameEnd.NewPassengerAndDriver);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropOldPassenger_NewPassengerAndDriverSameEndNewArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.NewPassengerTooLate, OnePassengerAlreadySameEnd.NewPassengerAndDriver);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropOldPassenger_NewPassengerAndDriverSameEndTooLong()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.TooLongTravel, OnePassengerAlreadySameEnd.NewPassengerAndDriver);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }


        [Test]
        public async Task ShouldMatch_TakeNewPassengerDropOldPassenger_OldAndNewPassengersSameEnd()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.Matching, OnePassengerAlreadySameEnd.OldAndNewPassengers);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            Assert.AreEqual(1, passengers.Count);
            Assert.AreEqual("Passager 2, potentiel nouveau passager", passengers[0].PassengerId);
            
            var d = users[0] as Driver;
            var p1 = users[1] as Passenger;
            var p2 = users[2] as Passenger;
            CollectionAssert.AreEqual(ImmutableList.Create(d.Start,p1.Start,p2.Start,p1.End,d.End),passengers[0].LatLngs);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropOldPassenger_OldAndNewPassengersSameEndOldArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.OldPassengerTooLate, OnePassengerAlreadySameEnd.OldAndNewPassengers);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropOldPassenger_OldAndNewPassengersSameEndNewArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.NewPassengerTooLate, OnePassengerAlreadySameEnd.OldAndNewPassengers);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropOldPassenger_OldAndNewPassengersSameEndTooLong()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.TooLongTravel, OnePassengerAlreadySameEnd.OldAndNewPassengers);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }


        [Test]
        public async Task ShouldMatch_TakeNewPassengerDropOldPassenger_NoneSameEnd()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.Matching, OnePassengerAlreadySameEnd.None);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            Assert.AreEqual(1, passengers.Count);
            Assert.AreEqual("Passager 2, potentiel nouveau passager", passengers[0].PassengerId);
            
            var d = users[0] as Driver;
            var p1 = users[1] as Passenger;
            var p2 = users[2] as Passenger;
            CollectionAssert.AreEqual(ImmutableList.Create(d.Start,p1.Start,p2.Start,p1.End,p2.End,d.End),passengers[0].LatLngs);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropOldPassenger_NoneSameEndOldArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.OldPassengerTooLate, OnePassengerAlreadySameEnd.None);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropOldPassenger_NoneSameEndNewArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.NewPassengerTooLate, OnePassengerAlreadySameEnd.None);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropOldPassenger_NoneSameEndTooLong()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropOldPassenger.GetUsers(OnePassengerAlreadyMatchingCase.TooLongTravel, OnePassengerAlreadySameEnd.None);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }


        [Test]
        public async Task ShouldMatch_DropOldPassengerTakeNewPassenger_NewPassengerAndDriverSameEnd()
        {
            userServiceImpl.EmptyUsersList();
            var users = DropOldPassengerTakeNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.Matching, OnePassengerAlreadySameEnd.NewPassengerAndDriver);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            Assert.AreEqual(1, passengers.Count);
            Assert.AreEqual("Passager 2, potentiel nouveau passager", passengers[0].PassengerId);

            var d = users[0] as Driver;
            var p1 = users[1] as Passenger;
            var p2 = users[2] as Passenger;
            CollectionAssert.AreEqual(ImmutableList.Create(d.Start,p1.Start,p1.End,p2.Start,d.End),passengers[0].LatLngs);
        }

        [Test]
        public async Task ShouldNotMatch_DropOldPassengerTakeNewPassenger_NewPassengerAndDriverSameEndOldArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = DropOldPassengerTakeNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.OldPassengerTooLate, OnePassengerAlreadySameEnd.NewPassengerAndDriver);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_DropOldPassengerTakeNewPassenger_NewPassengerAndDriverSameEndNewArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = DropOldPassengerTakeNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.NewPassengerTooLate, OnePassengerAlreadySameEnd.NewPassengerAndDriver);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_DropOldPassengerTakeNewPassenger_NewPassengerAndDriverSameEndTooLong()
        {
            userServiceImpl.EmptyUsersList();
            var users = DropOldPassengerTakeNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.TooLongTravel, OnePassengerAlreadySameEnd.NewPassengerAndDriver);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }


        [Test]
        public async Task ShouldMatch_DropOldPassengerTakeNewPassenger_NoneSameEnd()
        {
            userServiceImpl.EmptyUsersList();
            var users = DropOldPassengerTakeNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.Matching, OnePassengerAlreadySameEnd.None);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            Assert.AreEqual(1, passengers.Count);
            Assert.AreEqual("Passager 2, potentiel nouveau passager", passengers[0].PassengerId);
            
            var d = users[0] as Driver;
            var p1 = users[1] as Passenger;
            var p2 = users[2] as Passenger;
            CollectionAssert.AreEqual(ImmutableList.Create(d.Start,p1.Start,p1.End,p2.Start,p2.End,d.End),passengers[0].LatLngs);
        }

        [Test]
        public async Task ShouldNotMatch_DropOldPassengerTakeNewPassenger_NoneSameEndOldArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = DropOldPassengerTakeNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.OldPassengerTooLate, OnePassengerAlreadySameEnd.None);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_DropOldPassengerTakeNewPassenger_NoneSameEndNewArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = DropOldPassengerTakeNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.NewPassengerTooLate, OnePassengerAlreadySameEnd.None);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_DropOldPassengerTakeNewPassenger_NoneSameEndTooLong()
        {
            userServiceImpl.EmptyUsersList();
            var users = DropOldPassengerTakeNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.TooLongTravel, OnePassengerAlreadySameEnd.None);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }


        [Test]
        public async Task ShouldMatch_TakeNewPassengerDropNewPassenger_OldPassengerAndDriverSameEnd()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.Matching, OnePassengerAlreadySameEnd.OldPassengerAndDriver);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            Assert.AreEqual(1, passengers.Count);
            Assert.AreEqual("Passager 2, potentiel nouveau passager", passengers[0].PassengerId);

            var d = users[0] as Driver;
            var p1 = users[1] as Passenger;
            var p2 = users[2] as Passenger;
            CollectionAssert.AreEqual(ImmutableList.Create(d.Start,p1.Start,p2.Start,p2.End,d.End),passengers[0].LatLngs);

        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropNewPassenger_OldPassengerAndDriverSameEndOldArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.OldPassengerTooLate, OnePassengerAlreadySameEnd.OldPassengerAndDriver);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropNewPassenger_OldPassengerAndDriverSameEndNewArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.NewPassengerTooLate, OnePassengerAlreadySameEnd.OldPassengerAndDriver);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropNewPassenger_OldPassengerAndDriverSameEndTooLong()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.TooLongTravel, OnePassengerAlreadySameEnd.OldPassengerAndDriver);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldMatch_TakeNewPassengerDropNewPassenger_NoneSameEnd()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.Matching, OnePassengerAlreadySameEnd.None);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            Assert.AreEqual(1, passengers.Count);
            Assert.AreEqual("Passager 2, potentiel nouveau passager", passengers[0].PassengerId);
            
            var d = users[0] as Driver;
            var p1 = users[1] as Passenger;
            var p2 = users[2] as Passenger;
            CollectionAssert.AreEqual(ImmutableList.Create(d.Start,p1.Start,p2.Start,p2.End,p1.End,d.End),passengers[0].LatLngs);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropNewPassenger_NoneSameEndOldArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.OldPassengerTooLate, OnePassengerAlreadySameEnd.None);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropNewPassenger_NoneSameEndNewArriveLate()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.NewPassengerTooLate, OnePassengerAlreadySameEnd.None);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }

        [Test]
        public async Task ShouldNotMatch_TakeNewPassengerDropNewPassenger_NoneSameEndTooLong()
        {
            userServiceImpl.EmptyUsersList();
            var users = TakeNewPassengerDropNewPassenger.GetUsers(OnePassengerAlreadyMatchingCase.TooLongTravel, OnePassengerAlreadySameEnd.None);
            userServiceImpl.AddUser("Conducteur", users[0]);
            userServiceImpl.AddUser("Passager 1, déjà pris", users[1]);
            userServiceImpl.AddUser("Passager 2, potentiel nouveau passager", users[2]);

            var passengers = await tested.SearchPassengers("Conducteur");
            CollectionAssert.IsEmpty(passengers);
        }
        
    }
}