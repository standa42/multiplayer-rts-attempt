using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NetworkObjects;
using ProtoBuf;

namespace NetworkingWrapper
{
    [ProtoContract]

    [ProtoInclude(100, typeof(SerializationObjectWrapper<List<Packet>>))]
    [ProtoInclude(101, typeof(SerializationObjectWrapper<Packet>))]
    [ProtoInclude(102, typeof(SerializationObjectWrapper<MapPacket>))]
    [ProtoInclude(103, typeof(SerializationObjectWrapper<RacePacket>))]
    [ProtoInclude(104, typeof(SerializationObjectWrapper<RandomSeedPacket>))]
    [ProtoInclude(105, typeof(SerializationObjectWrapper<ReadyPacket>))]
    [ProtoInclude(106, typeof(SerializationObjectWrapper<StartPacket>))]

    public abstract class SerializationObjectWrapper
    {
        public abstract object Value { get; }

        public byte[] Serialize()
        {
            MemoryStream stream = new MemoryStream();

            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Base128);

            return stream.ToArray();
        }

        public static SerializationObjectWrapper Deserialize(Stream stream)
        {
            int length;
            if (Serializer.TryReadLengthPrefix(stream, PrefixStyle.Base128, out length))
            {
                var buffer = new byte[length];
                stream.Read(buffer, 0, buffer.Length);

                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(buffer, 0, buffer.Length);

                    ms.Position = 0;

                    SerializationObjectWrapper wrapper = Serializer.Deserialize<SerializationObjectWrapper>(ms);

                    return wrapper;
                }
            }

            throw new ArgumentException();
        }

        public static SerializationObjectWrapper Deserialize(byte[] serializedObject)
        {
            using (var ms = new MemoryStream(serializedObject))
            {
                ms.Position = 0;

                return Deserialize(ms);
            }
        }
    }

    /// <summary>
    ///     Typed NetworkObjectWrapper.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    [ProtoContract]
    public class SerializationObjectWrapper<T> : SerializationObjectWrapper
    {
        public override object Value
        {
            get { return TypedValue; }
        }

        [ProtoMember(1)]
        public T TypedValue { get; set; }
    }
}



