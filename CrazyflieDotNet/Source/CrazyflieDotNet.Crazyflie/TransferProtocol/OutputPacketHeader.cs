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
	/// <summary>
	///   Header Format (1 byte):
	///   8  7  6  5  4  3  2  1
	///   [  Port#  ][Res.][Ch.]
	///   Res. = reserved for transfer layer.
	///   Ch. = Channel
	/// </summary>
	public abstract class OutputPacketHeader
		: PacketHeader, IOutputPacketHeader
	{
		public const Channel DefaultChannel = Channel.Channel0;

		/// <summary>
		///   Header Format (1 byte):
		///   8  7  6  5  4  3  2  1
		///   [  Port#  ][Res.][Ch.]
		///   Res. = reserved for transfer layer.
		///   Ch. = Channel
		/// </summary>
		/// <param name="headerByte"> </param>
		protected OutputPacketHeader(byte headerByte)
		{
			Port = GetPort(headerByte);
			Channel = GetChannel(headerByte);
		}

		protected OutputPacketHeader(Port port, Channel channel = DefaultChannel)
		{
			Port = port;
			Channel = channel;
		}

		#region IOutputPacketHeader Members

		public Port Port { get; private set; }

		public Channel Channel { get; private set; }

		#endregion

		protected override byte? GetPacketHeaderByte()
		{
			try
			{
				var portByte = (byte) Port;
				var portByteAnd15 = (byte) (portByte & 0x0F);
				var portByteAnd15LeftShifted4 = (byte) (portByteAnd15 << 4);

				var reservedLeftShifted2 = (byte) (0x00 << 2);

				var channelByte = (byte) Channel;
				var channelByteAnd3 = (byte) (channelByte & 0x03);

				return (byte) (portByteAnd15LeftShifted4 | reservedLeftShifted2 | channelByteAnd3);
			}
			catch (Exception ex)
			{
				throw new DataException("Error converting output packet header to byte.", ex);
			}
		}

		protected virtual Port GetPort(byte headerByte)
		{
			try
			{
				var headerByteRightShiftedFour = (byte) (headerByte >> 4);
				var portByte = (byte) (headerByteRightShiftedFour & 0x0F);
				var port = (Port) Enum.ToObject(typeof (Port), portByte);
				return port;
			}
			catch (Exception ex)
			{
				throw new DataException("Error getting output packet header port value from header byte", ex);
			}
		}

		protected virtual Channel GetChannel(byte headerByte)
		{
			try
			{
				var channelByte = (byte) (headerByte & 0x03);
				var channel = (Channel) Enum.ToObject(typeof (Channel), channelByte);
				return channel;
			}
			catch (Exception ex)
			{
				throw new DataException("Error getting output packet header channel value from header byte", ex);
			}
		}
	}
}