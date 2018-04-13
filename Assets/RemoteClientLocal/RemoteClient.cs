/*
https://github.com/peterix/dfhack
Copyright (c) 2009-2012 Petr Mrázek (peterix@gmail.com)

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any
damages arising from the use of this software.

Permission is granted to anyone to use this software for any
purpose, including commercial applications, and to alter it and
redistribute it freely, subject to the following restrictions:

1. The origin of this software must not be misrepresented; you must
not claim that you wrote the original software. If you use this
software in a product, an acknowledgment in the product documentation
would be appreciated but is not required.

2. Altered source versions must be plainly marked as such, and
must not be misrepresented as being the original software.

3. This notice may not be removed or altered from any source
distribution.
*/

using dfproto;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using ProtoBuf;

namespace DFHack
{
    public enum DFHackReplyCode
    {
        RPC_REPLY_RESULT = -1,
        RPC_REPLY_FAIL = -2,
        RPC_REPLY_TEXT = -3,
        RPC_REQUEST_QUIT = -4
    }

    class RPCHandshakeHeader
    {
        //public string magic;
        //public int version;

        public static string REQUEST_MAGIC = "DFHack?\n";
        public static string RESPONSE_MAGIC = "DFHack!\n";
    }

    struct RPCMessageHeader
    {
        public const int MAX_MESSAGE_SIZE = 64 * 1048576;

        public Int16 id;
        public Int32 size;

        public byte[] ConvertToBtyes()
        {
            List<byte> output = new List<byte>();
            output.AddRange(BitConverter.GetBytes(id));
            output.AddRange(new byte[2]);
            output.AddRange(BitConverter.GetBytes(size));
            return output.ToArray();
        }
        string BytesToString(byte[] input)
        {
            string output = "";
            foreach (byte item in input)
            {
                if (output.Length > 0)
                    output += ",";
                output += item;
            }
            return output;
        }
    }

    public struct DFCoord
    {
        public int x, y, z;

        public DFCoord(int inx, int iny, int inz)
        {
            x = inx;
            y = iny;
            z = inz;
        }

        public override string ToString()
        {
            return string.Format("DFCoord({0},{1},{2})", x, y, z);
        }

        public static bool operator <(DFCoord a, DFCoord b)
        {
            if (a.x != b.x) return (a.x < b.x);
            if (a.y != b.y) return (a.y < b.y);
            return a.z < b.z;
        }
        public static bool operator >(DFCoord a, DFCoord b)
        {
            if (a.x != b.x) return (a.x > b.x);
            if (a.y != b.y) return (a.y > b.y);
            return a.z > b.z;
        }
        public static DFCoord operator +(DFCoord a, DFCoord b)
        {
            return new DFCoord(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static DFCoord operator -(DFCoord a, DFCoord b)
        {
            return new DFCoord(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static DFCoord operator /(DFCoord a, int number)
        {
            return new DFCoord((a.x < 0 ? a.x - number : a.x) / number, (a.y < 0 ? a.y - number : a.y) / number, a.z);
        }
        public static DFCoord operator *(DFCoord a, int number)
        {
            return new DFCoord(a.x * number, a.y * number, a.z);
        }
        public static DFCoord operator %(DFCoord a, int number)
        {
            return new DFCoord((a.x + number) % number, (a.y + number) % number, a.z);
        }
        public static DFCoord operator -(DFCoord a, int number)
        {
            return new DFCoord(a.x, a.y, a.z - number);
        }
        public static DFCoord operator +(DFCoord a, int number)
        {
            return new DFCoord(a.x, a.y, a.z + number);
        }
        public static bool operator ==(DFCoord a, DFCoord b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
        public static bool operator !=(DFCoord a, DFCoord b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }
        public override int GetHashCode()
        {
            return (((x * 499) + y) * 613) + z;
        }
        public override bool Equals(object obj)
        {
            return this == (DFCoord)obj;
        }
        public static implicit operator RemoteFortressReader.Coord(DFCoord a)
        {
            RemoteFortressReader.Coord b = new RemoteFortressReader.Coord();
            b.x = a.x;
            b.y = a.y;
            b.z = a.z;
            return b;
        }
        public static implicit operator DFCoord(RemoteFortressReader.Coord a)
        {
            return new DFCoord(a.x, a.y, a.z);
        }

        public int Max
        {
            get
            {
                return Math.Max(Math.Max(Math.Abs(x), Math.Abs(y)), Math.Abs(z));
            }
        }

        public float Magnitude
        {
            get
            {
                return (float)(Math.Sqrt(x * x + y * y + z * z));
            }
        }

    }
    public struct DFCoord2d
    {
        public int x;
        public int y;

        public DFCoord2d(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public bool isValid()
        {
            return x != -30000;
        }
        public void clear()
        {
            x = y = -30000;
        }

        public static bool operator <(DFCoord2d a, DFCoord2d b)
        {
            if (a.x != b.x) return (a.x < b.x);
            return a.y < b.y;
        }
        public static bool operator >(DFCoord2d a, DFCoord2d b)
        {
            if (a.x != b.x) return (a.x > b.x);
            return a.y > b.y;
        }

        public static DFCoord2d operator +(DFCoord2d a, DFCoord2d b)
        {
            return new DFCoord2d(a.x + b.x, a.y + b.y);
        }
        public static DFCoord2d operator -(DFCoord2d a, DFCoord2d b)
        {
            return new DFCoord2d(a.x - b.x, a.y - b.y);
        }

        public static DFCoord2d operator /(DFCoord2d a, int number)
        {
            return new DFCoord2d((a.x < 0 ? a.x - number : a.x) / number, (a.y < 0 ? a.y - number : a.y) / number);
        }
        public static DFCoord2d operator *(DFCoord2d a, int number)
        {
            return new DFCoord2d(a.x * number, a.y * number);
        }
        public static DFCoord2d operator %(DFCoord2d a, int number)
        {
            return new DFCoord2d((a.x + number) % number, (a.y + number) % number);
        }
        public static DFCoord2d operator &(DFCoord2d a, int number)
        {
            return new DFCoord2d(a.x & number, a.y & number);
        }

        public override string ToString()
        {
            return string.Format("DFCoord({0},{1})", x, y);
        }

        public override int GetHashCode()
        {
            return (x * 499) + y;
        }

        public int Max
        {
            get
            {
                return Math.Max(x, y);
            }
        }

        public float Magnitude
        {
            get
            {
                return (float)(Math.Sqrt(x * x + y * y));
            }
        }

    }
    // Coordinates of a MapBlock.
    // Like DFCoord, but can only reference block corners.
    // Use when you're expecting the coordinates of a block.
    public struct BlockCoord
    {
        public int x, y, z;

        public BlockCoord(int inx, int iny, int inz)
        {
            x = inx;
            y = iny;
            z = inz;
        }

        public static BlockCoord FromDFCoord(DFCoord coord)
        {
            if (coord.x % GameMap.blockSize != 0 || coord.y % GameMap.blockSize != 0)
            {
                throw new InvalidOperationException("Can't make a block coord from a non-block-corner");
            }
            return new BlockCoord(coord.x / GameMap.blockSize, coord.y / GameMap.blockSize, coord.z);
        }

        public DFCoord ToDFCoord()
        {
            return new DFCoord(x * GameMap.blockSize, y * GameMap.blockSize, z);
        }

        public override string ToString()
        {
            return string.Format("BlockCoord({0}[{1}],{2}[{3}],{4})", x, x * GameMap.blockSize, y, y * GameMap.blockSize, z);
        }

        public static bool operator <(BlockCoord a, BlockCoord b)
        {
            if (a.x != b.x) return (a.x < b.x);
            if (a.y != b.y) return (a.y < b.y);
            return a.z < b.z;
        }
        public static bool operator >(BlockCoord a, BlockCoord b)
        {
            if (a.x != b.x) return (a.x > b.x);
            if (a.y != b.y) return (a.y > b.y);
            return a.z > b.z;
        }
        public static BlockCoord operator +(BlockCoord a, BlockCoord b)
        {
            return new BlockCoord(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static BlockCoord operator -(BlockCoord a, BlockCoord b)
        {
            return new BlockCoord(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static BlockCoord operator /(BlockCoord a, int number)
        {
            return new BlockCoord((a.x < 0 ? a.x - number : a.x) / number, (a.y < 0 ? a.y - number : a.y) / number, a.z);
        }
        public static BlockCoord operator *(BlockCoord a, int number)
        {
            return new BlockCoord(a.x * number, a.y * number, a.z);
        }
        public static BlockCoord operator %(BlockCoord a, int number)
        {
            return new BlockCoord((a.x + number) % number, (a.y + number) % number, a.z);
        }
        public static BlockCoord operator -(BlockCoord a, int number)
        {
            return new BlockCoord(a.x, a.y, a.z - number);
        }
        public static BlockCoord operator +(BlockCoord a, int number)
        {
            return new BlockCoord(a.x, a.y, a.z + number);
        }
        public static bool operator ==(BlockCoord a, BlockCoord b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
        public static bool operator !=(BlockCoord a, BlockCoord b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return this == (BlockCoord)obj;
        }

        public struct Range
        {
            public readonly BlockCoord Min;
            public readonly BlockCoord Max;

            public Range(BlockCoord min, BlockCoord max)
            {
                Min = min;
                Max = max;
            }

            public override string ToString()
            {
                return string.Format("BlockCoord.Range({0},{1})", Min, Max);
            }
        }
    }

    /* Protocol description:
     *
     * 1. Handshake
     *
     *   Client initiates connection by sending the handshake
     *   request header. The server responds with the response
     *   magic. Currently both versions must be 1.
     *
     * 2. Interaction
     *
     *   Requests are done by exchanging messages between the
     *   client and the server. Messages consist of a serialized
     *   protobuf message preceeded by RPCMessageHeader. The size
     *   field specifies the length of the protobuf part.
     *
     *   NOTE: As a special exception, RPC_REPLY_FAIL uses the size
     *         field to hold the error code directly.
     *
     *   Every callable function is assigned a non-negative id by
     *   the server. Id 0 is reserved for BindMethod, which can be
     *   used to request any other id by function name. Id 1 is
     *   RunCommand, used to call console commands remotely.
     *
     *   The client initiates every call by sending a message with
     *   appropriate function id and input arguments. The server
     *   responds with zero or more RPC_REPLY_TEXT:CoreTextNotification
     *   messages, followed by RPC_REPLY_RESULT containing the output
     *   of the function if it succeeded, or RPC_REPLY_FAIL with the
     *   error code if it did not.
     *
     * 3. Disconnect
     *
     *   The client terminates the connection by sending an
     *   RPC_REQUEST_QUIT header with zero size and immediately
     *   closing the socket.
     */


    public class TimedRemoteFunction<TInput, TOutput>
        where TInput : class, IExtensible, new()
        where TOutput : class, IExtensible, new()
    {
        private RemoteFunction<TInput, TOutput> function;
        private float interval;

        Stopwatch watch = null;

        public TimedRemoteFunction(float interval, RemoteFunction<TInput, TOutput> function)
        {
            this.function = function;
            this.interval = interval;
        }

        public TOutput Execute(TInput input)
        {
            TOutput output = null;
            if(watch == null)
            {
                output = function.Execute(input);
                watch = new Stopwatch();
                watch.Start();
            }
            else if(watch.ElapsedMilliseconds > interval)
            {
                output = function.Execute(input);
                watch.Reset();
                watch.Start();
            }
            return output;
        }
    }
    public class TimedRemoteFunction<Input>
    where Input : class, IExtensible, new()
    {
        private RemoteFunction<Input> function;
        private double interval;

        Stopwatch watch = null;

        public TimedRemoteFunction(double interval, RemoteFunction<Input> function)
        {
            this.function = function;
            this.interval = interval;
        }

        public void Execute(Input input)
        {
            if (watch == null)
            {
                function.TryExecute(input);
                watch = new Stopwatch();
                watch.Start();
            }
            else if (watch.ElapsedMilliseconds > interval)
            {
                function.TryExecute(input);
                watch.Reset();
                watch.Start();
            }
            return;
        }
    }

}
