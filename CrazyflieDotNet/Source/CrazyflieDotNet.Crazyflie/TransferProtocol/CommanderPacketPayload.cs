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
using System.Linq;

#endregion

namespace CrazyflieDotNet.Crazyflie.TransferProtocol
{
	/// <summary>
	///   Commander Payload Format:
	///   Name   |  Index  |  Type  |  Size (bytes)
	///   roll        0       float      4
	///   pitch       4       float      4
	///   yaw         8       float      4
	///   thurst      12      ushort     2
	///   .............................total: 14 bytes
	/// </summary>
	public class CommanderPacketPayload
		: PacketPayload, ICommanderPacketPayload
	{
		private static readonly int _floatSize = sizeof (float);
		private static readonly int _shortSize = sizeof (ushort);
		private static readonly int _commanderPayloadSize = _floatSize * 3 + _shortSize;

		/// <summary>
		///   Commander Payload Format:
		///   Name   |  Index  |  Type  |  Size (bytes)
		///   roll        0       float      4
		///   pitch       4       float      4
		///   yaw         8       float      4
		///   thurst      12      ushort     2
		///   .............................total: 14 bytes
		/// </summary>
		/// <param name="payloadBytes"> </param>
		public CommanderPacketPayload(byte[] payloadBytes)
		{
			if (payloadBytes == null)
			{
				throw new ArgumentNullException("payloadBytes");
			}

			if (payloadBytes.Length != _commanderPayloadSize)
			{
				throw new ArgumentException(string.Format("Commander packet payload size must be {0} bytes.", _commanderPayloadSize));
			}

			Roll = GetRoll(payloadBytes);
			Pitch = GetPitch(payloadBytes);
			Yaw = GetYaw(payloadBytes);
			Thurst = GetThurst(payloadBytes);
		}

		public CommanderPacketPayload(float roll, float pitch, float yaw, ushort thrust)
		{
			Roll = roll;
			Pitch = pitch;
			Yaw = yaw;
			Thurst = thrust;
		}

		#region ICommanderPacketPayload Members

		public float Roll { get; private set; }

		public float Pitch { get; private set; }

		public float Yaw { get; private set; }

		public ushort Thurst { get; private set; }

		#endregion

		protected override byte[] GetPacketPayloadBytes()
		{
			try
			{
                var rollByte = BitConverter.GetBytes(Roll); //4byte
                var pitchByte = BitConverter.GetBytes(Pitch); //4byte
                var yawByte = BitConverter.GetBytes(Yaw); //4byte1
                var thrustByte = BitConverter.GetBytes(Thurst); //2byte

				var commanderPayloadBytes = new byte[_commanderPayloadSize];

                rollByte.CopyTo(commanderPayloadBytes, 0);
                pitchByte.CopyTo(commanderPayloadBytes, _floatSize);
                yawByte.CopyTo(commanderPayloadBytes, _floatSize * 2);
                thrustByte.CopyTo(commanderPayloadBytes, _floatSize * 3);

				return commanderPayloadBytes;
			}
			catch (Exception ex)
			{
				throw new DataException("Error converting commander packet payload to bytes.", ex);
			}
		}

		private float GetRoll(byte[] payloadBytes)
		{
			if (payloadBytes == null)
			{
				throw new ArgumentNullException("payloadBytes");
			}

			try
			{
				var rollBytes = payloadBytes.Skip(0).Take(_floatSize);
				var roll = Convert.ToSingle(rollBytes);
				return roll;
			}
			catch (Exception ex)
			{
				throw new DataException("Error getting commander packet header roll value from header byte", ex);
			}
		}

		private float GetPitch(byte[] payloadBytes)
		{
			if (payloadBytes == null)
			{
				throw new ArgumentNullException("payloadBytes");
			}

			try
			{
				var pitchBytes = payloadBytes.Skip(_floatSize).Take(_floatSize);
				var pitch = Convert.ToSingle(pitchBytes);
				return pitch;
			}
			catch (Exception ex)
			{
				throw new DataException("Error getting commander packet header pitch value from header byte", ex);
			}
		}

		private float GetYaw(byte[] payloadBytes)
		{
			if (payloadBytes == null)
			{
				throw new ArgumentNullException("payloadBytes");
			}

			try
			{
				var yawBytes = payloadBytes.Skip(_floatSize * 2).Take(_floatSize);
				var yaw = Convert.ToSingle(yawBytes);
				return yaw;
			}
			catch (Exception ex)
			{
				throw new DataException("Error getting commander packet header yaw value from header byte", ex);
			}
		}

		private ushort GetThurst(byte[] payloadBytes)
		{
			if (payloadBytes == null)
			{
				throw new ArgumentNullException("payloadBytes");
			}

			try
			{
				var thrustBytes = payloadBytes.Skip(_floatSize * 3).Take(_shortSize);
				var thrust = Convert.ToUInt16(thrustBytes);
				return thrust;
			}
			catch (Exception ex)
			{
				throw new DataException("Error getting commander packet header thrust value from header byte", ex);
			}
		}
	}
}