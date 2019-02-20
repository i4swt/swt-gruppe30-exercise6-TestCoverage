using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DoorControl
{
    public class DoorControl
    {
        private readonly IDoor _door;
        private readonly IUserValidation _userValidation;
        private readonly IEntryNotification _entryNotification;
        private readonly IAlarm _alarm;
        private DoorState _currentState;

        private enum DoorState
        {
            Closed,
            Closing,
            Opening,
            Breached,
        }

        public DoorControl(IDoor door,IUserValidation userValidation,IEntryNotification entryNotification,IAlarm alarm)
        {
            _door = door;
            _userValidation = userValidation;
            _entryNotification = entryNotification;
            _alarm = alarm;
            _currentState = DoorState.Closed;
        }

        public void RequestEntry(int id)
        {
            if (_userValidation.ValidateEntryRequest(id) && _currentState == DoorState.Closed)
            {
                _currentState = DoorState.Opening;
                _door.Open();
                _entryNotification.NotifyEntryGranted();
            }
            else
            {
                _entryNotification.NotifyEntryDenied();
            }
        }

        public void DoorOpened()
        {
            if (_currentState != DoorState.Closed)
            {
                _door.Close();
                _currentState = DoorState.Closing;
            }
            else
            {
                _door.Close();
                _alarm.RaiseAlarm();
                _currentState = DoorState.Breached;
            }
        }

        public void DoorClosed()
        {
            _currentState = DoorState.Closed;
        }
    }
}
