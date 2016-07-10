namespace SessionToken.Protobuf
{
    using System.IO;

    using Models;

    internal static class Serializer
    {
        internal static byte[] Serialize(object obj)
        {
            byte[] data;
            using (MemoryStream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, obj);
                data = stream.ToArray();
            }

            return data;
        }

        internal static Token Deserialize(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return ProtoBuf.Serializer.Deserialize<Token>(stream);
            }
        }
    }
}