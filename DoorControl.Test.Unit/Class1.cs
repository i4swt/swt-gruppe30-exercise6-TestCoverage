using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;

namespace DoorControl.Test.Unit
{
    #region FakeClasses
    public class FakeDoor : IDoor
    {
        public bool DoorIsOpen { get; private set; }
        public bool DoorIsClosed { get; private set; }

        public FakeDoor()
        {
            DoorIsOpen = false;
            DoorIsClosed = false;
        }

        public void Open()
        {
            DoorIsOpen = true;
        }

        public void Close()
        {
            DoorIsClosed = true;
        }
    }

    public class FakeUserValidation : IUserValidation
    {
        public bool ReturnValue { get; private set; }

        public FakeUserValidation(bool returnValue)
        {
            ReturnValue = returnValue;
        }

        public bool ValidateEntryRequest(int id)
        {
            return ReturnValue;
        }
    }

    public class FakeEntryNotification : IEntryNotification
    {
        public string LatestLogging { get; private set; }

        public void NotifyEntryGranted()
        {
            LatestLogging = "Entry is granted";
        }

        public void NotifyEntryDenied()
        {
            LatestLogging = "Entry is denied";
        }
    }

    public class FakeAlarm : IAlarm
    {
        public bool AlarmHasBeenRaised { get; private set; }

        public FakeAlarm()
        {
            AlarmHasBeenRaised = false;
        }

        public void RaiseAlarm()
        {
            AlarmHasBeenRaised = true;
        }
    }

    #endregion

    [TestFixture]
    public class DoorControlUnitTest
    {
        #region Setup
        private DoorControl _uut;
        private IEntryNotification _entryNotification;
        private IUserValidation _userValidation;
        private IDoor _door;
        private IAlarm _alarm;

        [SetUp]
        public void SetUp()
        {
            _entryNotification = Substitute.For<IEntryNotification>();
            _userValidation = Substitute.For<IUserValidation>();
            _door = Substitute.For<IDoor>();
            _alarm = Substitute.For<IAlarm>();
;           _uut = new DoorControl(_door,_userValidation,_entryNotification,_alarm);
        }
        #endregion

        #region RequestEntry

        [Test]
        public void RequestEntry_GetValidID_DoorOpens()
        {
            _userValidation.ValidateEntryRequest(0).Returns(true);

            _uut.RequestEntry(0);

            //Assert that
            _door.Received(1).Open();
        }

        [Test]
        public void RequestEntry_GetValidID_GrantedNotificationIsSent()
        {
            _userValidation.ValidateEntryRequest(0).Returns(true);

            _uut.RequestEntry(0);
            
            //Assert that
            _entryNotification.Received(1).NotifyEntryGranted();
        }

        [Test]
        public void RequestEntry_GetInvalidID_DeniedNotificationIsSent()
        {
            _userValidation.ValidateEntryRequest(0).Returns(false);

            _uut.RequestEntry(0);
            
            //Assert that
            _entryNotification.DidNotReceive().NotifyEntryGranted();
        }

        [Test]
        public void RequestEntry_GetInValidID_DoorDoesntOpen()
        {
            _userValidation.ValidateEntryRequest(0).Returns(false);

            _uut.RequestEntry(0);

            //Assert that
            _door.DidNotReceive().Open();
        }

        #endregion

        #region DoorOpened
        [Test]
        public void DoorOpened_GetValidIdAndCallDoorOpen_DoorIsClosed()
        {
            _userValidation.ValidateEntryRequest(0).Returns(true);

            _uut.RequestEntry(0);
            _uut.DoorOpened();

            _door.Received(1).Close();
        }

        [Test]
        public void DoorOpened_GetValidIdAndCallDoorOpen_AlarmIsNotRaised()
        {
            _userValidation.ValidateEntryRequest(0).Returns(true);

            _uut.RequestEntry(0);
            _uut.DoorOpened();

            //Assert that
            _alarm.DidNotReceive().RaiseAlarm();
        }

        [Test]
        public void DoorOpened_NoIdHasBeenGivenAndCallDoorOpen_DoorIsClosed()
        {
            _userValidation.ValidateEntryRequest(0).Returns(true);

            _uut.DoorOpened();

            //Assert that
            _door.Received(1).Close();
        }

        [Test]
        public void DoorOpened_NoIdHasBeenGivenAndCallDoorOpen_AlarmIsRaised()
        {
            _userValidation.ValidateEntryRequest(0).Returns(true);

            _uut.DoorOpened();

            //Assert that
            _alarm.Received(1).RaiseAlarm();
        }

        #endregion

        #region DoorClosed
        [Test]
        public void DoorClosed_DoorClosedCalled_NothingHappens()
        {
            _uut.DoorClosed();
        }
        #endregion
    }
}
