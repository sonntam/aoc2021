using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day16 : IAoCProgram
    {
        public Day16() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {

            var packetString = input.ReadLine();

            var packets = ElvenPacket.Parse(packetString);

            var versionSum = packets.IterateTreeBreadthFirst(x => x.GetType().GetProperty("Children") != null ? (x as ElvenOperatorPacket).Children : null).Select(x => x.Header.Version).Aggregate(0, (s, v) => s + v);

            return versionSum.ToString();
        }

        public override string RunB(StreamReader input)
        {
            var packetString = input.ReadLine();

            var packets = ElvenPacket.Parse(packetString);

            var value = (packets as ElvenOperatorPacket).Value;

            return value.ToString();
        }
    }

    public class ElvenPacket
    {
        public const int PacketHeaderBitLength = 6;
        public struct PacketHeader
        {
            public int Version;
            public PacketType PacketType;
        }

        public enum PacketType : int {
            SUM = 0,
            PRODUCT = 1,
            MINIMUM = 2,
            MAXIMUM = 3,
            GREATER_THAN = 5,
            LESS_THAN = 6,
            EQUAL_TO = 7,

            LITERAL_VALUE = 4,
        }

        //public enum 

        protected ElvenPacket() { }

        public PacketHeader Header { get; protected set; }
        public ElvenPacket Parent { get; protected set; }
     
        public BigInteger Value { get; protected set; }

        protected static PacketHeader GetHeader( IEnumerable<bool> bitstream )
        {
            var version = bitstream.Take(3).BitsToInt();
            var type = (PacketType)bitstream.Skip(3).Take(3).BitsToInt();

            return new PacketHeader() { PacketType = type, Version = version };
        }

        public static ElvenPacket Parse(string packages, ElvenPacket parent = null)
        {
            var packetStartIt = ToBitsFromHexString(packages);

            return Parse(packetStartIt, parent).Item1;
        }

        public static (ElvenPacket, IEnumerable<bool>) Parse(IEnumerable<bool> bitstream, ElvenPacket parent = null)
        {
            // Read type
            var type = (PacketType)bitstream.Skip(3).Take(3).BitsToInt();
            ElvenPacket packet;
            IEnumerable<bool> enumerator;
            switch (type)
            {
                case PacketType.LITERAL_VALUE:
                    (packet, enumerator) = ElvenLiteralPacket.Parse(bitstream);
                    break;
                default:
                    (packet, enumerator) = ElvenOperatorPacket.Parse(bitstream);
                    break;

            }

            packet.Parent = parent;

            return (packet, enumerator);
        }

        protected static IEnumerable<bool> ToBitsFromHexString(string packages)
        {
            foreach( var nibble in packages )
            {
                var intVal = Convert.ToInt32($"{nibble}", 16);

                for (int i = 0; i < 4; i++)
                {
                    var bitval = intVal & (1 << (3 - i));

                    yield return bitval > 0 ? true : false;
                }
            }
        }

        public bool GetBit(string transmission, int offset)
        {
            if (offset >= transmission.Length * 4 ) throw new ArgumentException("Offset is beyond available data.");

            var hexChar = transmission.Skip(offset / 4).Take(1).Single();
            var intVal = Convert.ToInt32($"{hexChar}", 16);

            var bitval = intVal & (1 << (3 - (offset % 4)));

            return (bitval > 0 ) ? true : false;
        }
    }

    public class ElvenOperatorPacket : ElvenPacket
    {
        
        public List<ElvenPacket> Children { get; protected set; }

        public static (ElvenOperatorPacket, IEnumerable<bool>) Parse(IEnumerable<bool> bitstream)
        {
            var header = GetHeader(bitstream);

            var packet = new ElvenOperatorPacket()
            {
                Header = header,
                Children = new List<ElvenPacket>()
            };

            var lengthTypeId = bitstream.Skip(PacketHeaderBitLength).Take(1).Single();
            var dataStream = bitstream.Skip(PacketHeaderBitLength + 1);

            if( false == lengthTypeId )
            {
                var subPacketsTotalLength = dataStream.Take(15).BitsToInt();
                dataStream = dataStream.Skip(15);

                var numBitsToGo = subPacketsTotalLength;

                while( numBitsToGo > 0 )
                {
                    ElvenPacket innerPacket;

                    var beforeBitCount = dataStream.Count();
                    (innerPacket, dataStream) = ElvenPacket.Parse(dataStream, packet);
                    var afterBitCount = dataStream.Count();

                    packet.Children.Add(innerPacket);

                    numBitsToGo -= (beforeBitCount - afterBitCount);

                    if (numBitsToGo < 0) throw new Exception("Invalid length");
                }
            } 
            else
            {
                var subPacketsTotalCount = dataStream.Take(11).BitsToInt();
                dataStream = dataStream.Skip(11);

                for( int i = 0; i < subPacketsTotalCount; i++ )
                {
                    ElvenPacket innerPacket;

                    (innerPacket, dataStream) = ElvenPacket.Parse(dataStream, packet);

                    packet.Children.Add(innerPacket);
                }
            }

            switch( packet.Header.PacketType )
            {
                case PacketType.SUM:
                    packet.Value = packet.Children.Aggregate(BigInteger.Zero, (s, v) => s + v.Value);
                    break;
                case PacketType.PRODUCT:
                    packet.Value = packet.Children.Aggregate(BigInteger.One, (s, v) => s * v.Value);
                    break;
                case PacketType.MINIMUM:
                    packet.Value = packet.Children.Min(p => p.Value);
                    break;
                case PacketType.MAXIMUM:
                    packet.Value = packet.Children.Max(p => p.Value);
                    break;
                case PacketType.GREATER_THAN:
                    if (packet.Children.Count != 2) throw new Exception($"Packet of type {packet.Header.PacketType} must have two children.");
                    packet.Value = packet.Children.First().Value > packet.Children.Last().Value ? 1 : 0;
                    break;
                case PacketType.LESS_THAN:
                    if (packet.Children.Count != 2) throw new Exception($"Packet of type {packet.Header.PacketType} must have two children.");
                    packet.Value = packet.Children.First().Value < packet.Children.Last().Value ? 1 : 0;
                    break;
                case PacketType.EQUAL_TO:
                    if (packet.Children.Count != 2) throw new Exception($"Packet of type {packet.Header.PacketType} must have two children.");
                    packet.Value = packet.Children.First().Value == packet.Children.Last().Value ? 1 : 0;
                    break;
                default:
                    throw new Exception($"Invalid packet type {packet.Header.PacketType}");
            }

            return (packet, dataStream);
        }
    }

    public class ElvenLiteralPacket : ElvenPacket
    {
        public static (ElvenLiteralPacket, IEnumerable<bool>) Parse(IEnumerable<bool> bitstream)
        {
            var header = GetHeader(bitstream);
            long value = 0;

            var dataStream = bitstream.Skip(PacketHeaderBitLength);
            bool hasNextPart = false;
            BigInteger number = BigInteger.Zero;
            do
            {
                number <<= 4;
                hasNextPart = dataStream.Take(1).Single();
                number += dataStream.Skip(1).Take(4).BitsToInt();

                // Advance
                dataStream = dataStream.Skip(5);

            } while (hasNextPart);

            var packet = new ElvenLiteralPacket()
            {
                Header = header,
                Value = number
            };

            // Advance
            

            return (packet, dataStream); 
        }
    }
}
