/* 
 *						 _ _  _     
 *		       ____ ___  ___  __________(_|_)(_)____
 *		      / __ `__ \/ _ \/ ___/ ___/ / _ \/ ___/
 *		     / / / / / /  __(__  |__  ) /  __/ /    
 *		    /_/ /_/ /_/\___/____/____/_/\___/_/  
 *
 *	     Copyright 2013 - Messier/Chris Karcz - ckarcz@gmail.com
 *
 *	This Source Code Form is subject to the terms of the Mozilla Public
 *	License, v. 2.0. If a copy of the MPL was not distributed with this
 *	file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

#region Imports

using System;
using System.Data;

#endregion

namespace CrazyflieDotNet.Crazyflie.TransferProtocol
{
	public abstract class Packet<TPacketHeader>
		: Packet<TPacketHeader, IPacketPayload>
		where TPacketHeader : IPacketHeader
	{
		protected Packet(byte[] packetBytes)
			: base(packetBytes)
		{
		}

		protected Packet(TPacketHeader header)
			: base(header, null)
		{
		}

		protected override IPacketPayload ParsePayload(byte[] packetBytes)
		{
			return null;
		}
	}

	public abstract class Packet<TPacketHeader, TPacketPayload>
		: IPacket<TPacketHeader, TPacketPayload>
		where TPacketHeader : IPacketHeader
		where TPacketPayload : IPacketPayload
	{
		public static readonly byte[] EmptyPacketBytes = new byte[0];

		protected Packet(byte[] packetBytes)
		{
			if (packetBytes != null && packetBytes.Length > 0)
			{
				Header = ParseHeader(packetBytes);
				Payload = ParsePayload(packetBytes);
			}
		}

		protected Packet(TPacketHeader header, TPacketPayload payload)
		{
			Header = header;
			Payload = payload;
		}

		#region IPacket<TPacketHeader,TPacketPayload> Members

		public TPacketHeader Header { get; private set; }

		public TPacketPayload Payload { get; private set; }

		IPacketHeader IPacket.Header
		{
			get { return Header; }
		}

		IPacketPayload IPacket.Payload
		{
			get { return Payload; }
		}

		public byte[] GetBytes()
		{
			try
			{
				var packetBytes = GetPacketBytes();

				// so we never return null
				return packetBytes ?? EmptyPacketBytes;
			}
			catch (Exception ex)
			{
				throw new DataException("Error obtaining packet bytes.", ex);
			}
		}

		#endregion

		protected virtual byte[] GetPacketBytes()
		{
			var headerByte = Header != null ? Header.GetByte() : null;
			var headerByteLength = (headerByte != null ? 1 : 0);

			var payloadBytes = Payload != null ? Payload.GetBytes() : null;
			var payloadBytesLength = (payloadBytes != null ? payloadBytes.Length : 0);

			var packetBytesArraySize = headerByteLength + payloadBytesLength;
			var packetBytesArray = new byte[packetBytesArraySize];

			if (headerByte != null && headerByteLength > 0)
			{
				Array.Copy(new[] {headerByte.Value}, 0, packetBytesArray, 0, headerByteLength);
			}

			if (payloadBytes != null && payloadBytesLength > 0)
			{
				Array.Copy(payloadBytes, 0, packetBytesArray, headerByteLength, payloadBytesLength);
			}

			return packetBytesArray;
		}

		protected abstract TPacketHeader ParseHeader(byte[] packetBytes);

		protected abstract TPacketPayload ParsePayload(byte[] packetBytes);
	}
}