namespace SessionToken.Models
{
    using System;

    using ProtoBuf;

    [ProtoContract]
    public class Token
    {
        [ProtoMember(1)]
        public int UserId { get; set; }

        [ProtoMember(2)]
        public string SessionId { get; set; }

        [ProtoMember(3)]
        public long ValidToTicks { get; set; }

        public bool IsValid => DateTime.UtcNow < this.ValidTo;

        // Converts the internal "ValidToTicks" expiration timestamp to a DateTime (based on UTC.Now) 
        public DateTime ValidTo => new DateTime(this.ValidToTicks);

        public override bool Equals(object obj)
        {
            Token token = obj as Token;
            if (ReferenceEquals(null, token))
            {
                return false;
            }

            return this.UserId == token.UserId &&
                   this.SessionId == token.SessionId &&
                   this.ValidToTicks == token.ValidToTicks;
        }

        public override int GetHashCode()
        {
            return this.UserId.GetHashCode();
        }
    }
}