// SPDX-FileCopyrightText: 2022 Demerzel Solutions Limited
// SPDX-License-Identifier: LGPL-3.0-only

using System;
using DotNetty.Buffers;
using Nethermind.Serialization.Rlp;

namespace Nethermind.Network.P2P
{
    public class NettyRlpStream : RlpStream
    {
        private readonly IByteBuffer _buffer;

        private readonly int _initialPosition;

        public NettyRlpStream(IByteBuffer buffer)
        {
            _buffer = buffer;
            _initialPosition = buffer.ReaderIndex;
        }

        public override void Write(Span<byte> bytesToWrite)
        {
            _buffer.EnsureWritable(bytesToWrite.Length, true);

            Span<byte> target =
                _buffer.Array.AsSpan(_buffer.ArrayOffset + _buffer.WriterIndex, bytesToWrite.Length);
            bytesToWrite.CopyTo(target);
            int newWriterIndex = _buffer.WriterIndex + bytesToWrite.Length;

            _buffer.SetWriterIndex(newWriterIndex);
        }

        public override void WriteByte(byte byteToWrite)
        {
            _buffer.EnsureWritable(1, true);
            _buffer.WriteByte(byteToWrite);
        }

        protected override void WriteZero(int length)
        {
            _buffer.EnsureWritable(length, true);
            _buffer.WriteZero(length);
        }

        public override byte ReadByte()
        {
            return _buffer.ReadByte();
        }

        public override Span<byte> Read(int length)
        {
            Span<byte> span = _buffer.Array.AsSpan(_buffer.ArrayOffset + _buffer.ReaderIndex, length);
            _buffer.SkipBytes(span.Length);
            return span;
        }

        public override byte PeekByte()
        {
            byte result = _buffer.ReadByte();
            _buffer.SetReaderIndex(_buffer.ReaderIndex - 1);
            return result;
        }

        protected override byte PeekByte(int offset)
        {
            _buffer.MarkReaderIndex();
            _buffer.SkipBytes(offset);
            byte result = _buffer.ReadByte();
            _buffer.ResetReaderIndex();
            return result;
        }

        protected override void SkipBytes(int length)
        {
            _buffer.SkipBytes(length);
        }

        public override int Position
        {
            get => _buffer.ReaderIndex - _initialPosition;
            set => _buffer.SetReaderIndex(_initialPosition + value);
        }

        public override int Length => _buffer.ReadableBytes + (_buffer.ReaderIndex - _initialPosition);

        public override bool HasBeenRead => _buffer.ReadableBytes > 0;

        protected override string Description => "|NettyRlpStream|description missing|";
    }
}
