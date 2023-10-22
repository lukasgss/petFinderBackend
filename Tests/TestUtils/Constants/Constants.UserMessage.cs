using Domain.Entities;
using Tests.EntityGenerators;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
    public static class UserMessageData
    {
        public const long Id = 1;
        public const string Content = "Content";
        public static DateTime TimeStamp = new(2020, 1, 1);
        public const bool HasBeenRead = false;
        public const bool HasBeenEdited = false;
        public const bool HasBeenDeleted = false;
        public static User Sender = UserGenerator.GenerateUserWithRandomId();
        public static Guid SenderId = Sender.Id;
        public static User Receiver = UserGenerator.GenerateUserWithRandomId();
        public static Guid ReceiverId = Receiver.Id;
    }
}