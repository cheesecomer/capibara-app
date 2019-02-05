using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Capibara.Domain.Models
{
    public class Room : ModelBase<Room>
    {
        private bool isConnected;

        private int id;

        private string name;

        private int capacity;

        private int numberOfParticipants;

        public virtual bool IsConnected
        {
            get => this.isConnected;
            set => this.SetProperty(ref this.isConnected, value);
        }

        public ObservableCollection<Message> Messages { get; }
            = new ObservableCollection<Message>();

        public ObservableCollection<User> Participants { get; }
            = new ObservableCollection<User>();

        public int Id
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }

        public string Name
        {
            get => this.name;
            set => this.SetProperty(ref this.name, value);
        }

        public int Capacity
        {
            get => this.capacity;
            set => this.SetProperty(ref this.capacity, value);
        }

        public int NumberOfParticipants
        {
            get => this.numberOfParticipants;
            set => this.SetProperty(ref this.numberOfParticipants, value);
        }

        public override Room Restore(Room other)
        {
            this.Id = other.Id;
            this.Name = other.Name;
            this.Capacity = other.Capacity;
            this.IsConnected = other.IsConnected;
            this.NumberOfParticipants = other.NumberOfParticipants;

            // 差分を追加
            other.Participants
                ?.Where(x => this.Participants.All(v => v.Id != x.Id))
                ?.ToList()
                ?.ForEach(x => this.Participants.Add(x));

            // 差分を削除
            this.Participants
                ?.Where(x => other.Participants.All(v => v.Id != x.Id))
                ?.ToList()
                ?.ForEach(x => this.Participants.Remove(x));

            // 差分を追加
            other.Messages
                ?.Where(x => this.Messages.All(v => v.Id != x.Id))
                ?.ToList()
                ?.ForEach(x => this.Messages.Add(x));

            // 差分を削除
            this.Messages
                ?.Where(x => other.Messages.All(v => v.Id != x.Id))
                ?.ToList()
                ?.ForEach(x => this.Messages.Remove(x));

            return this;
        }

        public override bool Equals(object obj)
        {
            if (obj is Room room)
            {
                if (room.Id != this.Id) return false;
                if (room.Name != this.Name) return false;
                if (room.Capacity != this.Capacity) return false;
                if (room.NumberOfParticipants != this.NumberOfParticipants) return false;

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{ Id: {this.Id}, Name: {this.Name}, Capacity: {this.Capacity}, NumberOfParticipants: {this.NumberOfParticipants} }}";
        }
    }
}
